using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed class RelayPulseIo : IRelayPulseIo
{
    public Task<MakineRoleIslemleri.KanalDurumSonucu> Set(MakineRoleBaglantilari c, bool on)
    { return on ? MakineRoleIslemleri.SetRelayOn(c) : MakineRoleIslemleri.SetRelayOff(c); }
    public Task<MakineRoleIslemleri.KanalDurumSonucu> Read(MakineRoleBaglantilari c)
    { return MakineRoleIslemleri.ReadRelayStatus(c); }
}

public sealed class RelayPulseService
{
    public static readonly RelayPulseService Default = new RelayPulseService(new RelayPulseStore(), new RelayPulseIo(), () => RelayPulseClock.Now);
    private readonly IRelayPulseStore store;
    private readonly IRelayPulseIo io;
    private readonly Func<DateTime> turkeyNow;
    private readonly ConcurrentDictionary<long, byte> running = new ConcurrentDictionary<long, byte>();
    public string WorkerError { get; private set; }

    public RelayPulseService(IRelayPulseStore store, IRelayPulseIo io, Func<DateTime> turkeyNow)
    { this.store = store; this.io = io; this.turkeyNow = turkeyNow; }

    // Çağıran işlem, bağlantı bilgileri alınırken mevcut ayar/cihaz kilidini korur.
    // Bağımsız kalıcı kayıt, sayfa işlemi geri alınsa da korunur.
    public async Task<string> TriggerStopPulse(MakineRoleBaglantilari connection)
    {
        using (var lease = store.TryLock(connection))
        {
            if (lease == null || store.HasActive(connection.MakineId))
                return "Bu röle cihazında başka bir işlem veya makinede aktif pulse var; yeni OFF gönderilmedi. Kısa süre sonra tekrar deneyiniz.";
            var now = turkeyNow();
            var pulse = new RelayPulse { MakineId=connection.MakineId, ControllerIp=connection.Ip,
                HttpPort=connection.HttpPort, RelayChannel=connection.KanalNo, PulseAktif=true,
                PulseBaslangic=now, PulseBitis=now.AddSeconds(10), SonrakiDeneme=now.AddSeconds(10) };
            // Bu kayıt eklenemezse OFF kesinlikle gönderilmez.
            lease.Create(pulse);
            try
            {
                lease.Log(pulse, "OFF hazırlık", null, true, null);
                // Geciken istek, pulse bitişinden sonra OFF gönderemez.
                if (turkeyNow() >= pulse.PulseBitis) throw new InvalidOperationException("OFF gönderilmeden pulse süresi doldu.");
                var command = await io.Set(connection, false).ConfigureAwait(false);
                LogIo(lease, pulse, Endpoint(pulse, false), command);
                // Zaman aşımındaki komut da cihaza ulaşmış olabilir; her durumda /98 okunur.
                var status = await VerifyRelayState(lease, pulse, false).ConfigureAwait(false);
                pulse.OffDogrulandi = status.Basarili && !status.IoOn;
                if (!pulse.OffDogrulandi)
                {
                    pulse.SonHata = "OFF doğrulanamadı. " + UserError(status.Hata);
                    // IO belirsizse beklemeden ON kurtarması denenir.
                    pulse.SonrakiDeneme = turkeyNow();
                    lease.Save(pulse);
                    await Release(lease, pulse, "OFF doğrulama hatası").ConfigureAwait(false);
                    return "OFF doğrulanamadı; durdurma başarılı kabul edilmedi. " + (pulse.SonHata ?? "Röle ON durumuna geri alındı.");
                }
                lease.Save(pulse);
                return null;
            }
            catch (Exception ex)
            {
                PersistFailure(lease, pulse, ex);
                // Veritabanı veya log hatası, olası OFF sonrasındaki acil ON denemesini engellemez.
                await EmergencyOn(pulse, ex).ConfigureAwait(false);
                throw;
            }
        }
    }

    public bool HasActivePulse(int machineId) { return store.HasActive(machineId); }
    public string GetWarnings()
    {
        string warnings = store.GetWarnings();
        return string.IsNullOrEmpty(WorkerError) ? warnings : WorkerError + " | " + warnings;
    }

    private static string Endpoint(RelayPulse p, bool on)
    { return (on ? "ON /" : "OFF /") + ((p.RelayChannel-1)*2+(on ? 1 : 0)).ToString("D2"); }

    private static string Error(MakineRoleIslemleri.KanalDurumSonucu result)
    { return (result.Hata ?? "Beklenen IO biti okunamadı.") + " " + result.HataDetayi; }

    // Teknik hata ayrıntıları RelayPulseEvents içinde kalır; eski ekran mesajları da sadeleştirilir.
    public static string UserError(string error)
    {
        if (string.IsNullOrWhiteSpace(error)) return "Beklenen IO durumu doğrulanamadı.";
        int details = error.IndexOf("System.", StringComparison.Ordinal);
        if (details >= 0) error = error.Substring(0, details);
        return error.Trim();
    }

    private static void LogIo(IRelayPulseLease lease, RelayPulse p, string command, MakineRoleIslemleri.KanalDurumSonucu result)
    { lease.Log(p, command, result.HamCevap, result.Basarili, result.Basarili ? null : Error(result)); }

    private async Task<MakineRoleIslemleri.KanalDurumSonucu> VerifyRelayState(IRelayPulseLease lease, RelayPulse p, bool expectedOn)
    {
        var status = await io.Read(p.Baglanti()).ConfigureAwait(false);
        bool matched = status.Basarili && status.IoOn == expectedOn;
        lease.Log(p, "/98 beklenen " + (expectedOn ? "ON" : "OFF"), status.HamCevap, matched, matched ? null : Error(status));
        return status;
    }

    public Task ReleaseExpiredPulses() { return Dispatch(false); }
    public Task RecoverPendingPulses() { return Dispatch(true); }

    private Task Dispatch(bool recovery)
    {
        var tasks = new List<Task>();
        foreach (var pulse in store.GetDue(recovery))
        {
            if (!running.TryAdd(pulse.Id, 0)) continue;
            // Senkron veritabanı işlemleri ayrı görevde yürütülerek tarama döngüsünün beklemesi önlenir.
            tasks.Add(Task.Run(() => Process(pulse, recovery)));
        }
        return Task.WhenAll(tasks);
    }

    private async Task Process(RelayPulse candidate, bool recovery)
    {
        try
        {
            using (var lease = store.TryLock(candidate.Baglanti()))
            {
                if (lease == null) return;
                // Kayıt süreçler arası cihaz kilidi altında yeniden okunur; eski bilgiyle işlem yapılmaz.
                var pulse = lease.Get(candidate.Id);
                bool safetyThreshold = pulse != null && !pulse.KritikHata && turkeyNow() >= pulse.PulseBaslangic.AddSeconds(12);
                if (pulse == null || !pulse.PulseAktif || (turkeyNow() < pulse.PulseBitis && pulse.ReleaseDenemeSayisi == 0)
                    || (!recovery && !safetyThreshold && turkeyNow() < pulse.SonrakiDeneme)) return;
                try { await Release(lease, pulse, recovery ? "Recovery" : "Watchdog").ConfigureAwait(false); }
                catch (Exception ex) { PersistFailure(lease, pulse, ex); await EmergencyOn(pulse, ex).ConfigureAwait(false); throw; }
            }
        }
        catch (Exception ex) { ReportWorkerError(ex); }
        finally { byte ignored; running.TryRemove(candidate.Id, out ignored); }
    }

    private async Task Release(IRelayPulseLease lease, RelayPulse pulse, string source)
    {
        if (!pulse.KritikHata && turkeyNow() >= pulse.PulseBaslangic.AddSeconds(12))
        {
            pulse.KritikHata = true;
            pulse.SonHata = "KRİTİK: 12 saniye içinde ON doğrulanmadı; watchdog beklemeden ON deniyor.";
            lease.Save(pulse);
            lease.Log(pulse, "12 saniye güvenlik eşiği", null, false, pulse.SonHata);
            RelayPulseDiagnostics.Error(Context(pulse) + " " + pulse.SonHata);
        }
        // İlk bırakma girişimi 10. saniyede, öncesinde durum sorgusu beklenmeden ON gönderir.
        // Sınırlı tekrarlar bittikten sonra watchdog bağımsız kurtarma girişimlerini sürdürür.
        bool watchdogRescue = pulse.ReleaseDenemeSayisi >= 4;
        if (watchdogRescue)
        {
            var observed = await VerifyRelayState(lease, pulse, true).ConfigureAwait(false);
            if (observed.Basarili && observed.IoOn) { Complete(lease, pulse); return; }
        }
        pulse.ReleaseDenemeSayisi++;
        // Deneme sayısı komuttan önce kalıcılaştırılır; yeniden başlatmada kaybolmaz.
        pulse.SonrakiDeneme = turkeyNow();
        lease.Save(pulse);
        lease.Log(pulse, source + " " + Endpoint(pulse, true) + " hazırlık", null, true, null);
        var command = await io.Set(pulse.Baglanti(), true).ConfigureAwait(false);
        LogIo(lease, pulse, Endpoint(pulse, true), command);
        var status = await VerifyRelayState(lease, pulse, true).ConfigureAwait(false);
        if (status.Basarili && status.IoOn) { Complete(lease, pulse); return; }

        bool abnormal = turkeyNow() >= pulse.PulseBaslangic.AddSeconds(12);
        pulse.KritikHata = pulse.KritikHata || abnormal || pulse.ReleaseDenemeSayisi >= 4;
        pulse.SonHata = (pulse.KritikHata ? "KRİTİK: " : "") + "ON doğrulanamadı. "
            + (abnormal ? "12 saniyelik güvenlik eşiği aşıldı. " : "")
            + (pulse.ReleaseDenemeSayisi >= 4 ? "İlk ON ve üç tekrar başarısız; watchdog kurtarmayı sürdürüyor. " : "")
            + UserError(status.Hata);
        pulse.SonrakiDeneme = turkeyNow().AddSeconds(pulse.ReleaseDenemeSayisi < 4 ? pulse.ReleaseDenemeSayisi : 5);
        lease.Save(pulse);
        lease.Log(pulse, "ON başarısız", status.HamCevap, false, pulse.SonHata + " " + Error(status));
        if (pulse.KritikHata) RelayPulseDiagnostics.Error(Context(pulse) + " " + pulse.SonHata);
    }

    private void Complete(IRelayPulseLease lease, RelayPulse pulse)
    {
        pulse.PulseAktif = false;
        pulse.GercekOnZamani = turkeyNow();
        pulse.SonHata = null;
        lease.Save(pulse);
        lease.Log(pulse, "ON doğrulandı / pulse tamamlandı", null, true, null);
        // MakineLoglari.Kapat() çağrılmaz; IO ON olması makinenin çalıştığını göstermez.
    }

    private async Task EmergencyOn(RelayPulse pulse, Exception cause)
    {
        RelayPulseDiagnostics.Error(Context(pulse) + " KRİTİK: " + cause);
        try
        {
            var command = await io.Set(pulse.Baglanti(), true).ConfigureAwait(false);
            var state = await io.Read(pulse.Baglanti()).ConfigureAwait(false);
            RelayPulseDiagnostics.Error(Context(pulse) + " Acil ON: " + command.Hata + " /98=" + state.HamCevap + " " + state.Hata);
            // Kalıcı doğrulama kaydı olmadan pulse kapatılmaz; sonraki watchdog yeniden doğrular.
        }
        catch (Exception ex) { RelayPulseDiagnostics.Error(Context(pulse) + " Acil ON başarısız: " + ex); }
    }

    private void PersistFailure(IRelayPulseLease lease, RelayPulse pulse, Exception ex)
    {
        pulse.KritikHata = true;
        pulse.SonHata = "KRİTİK: Pulse işlemi tamamlanamadı. " + ex.Message;
        pulse.SonrakiDeneme = turkeyNow().AddSeconds(5);
        try { lease.Save(pulse); lease.Log(pulse, "Exception", null, false, ex.ToString()); }
        catch (Exception storageError) { RelayPulseDiagnostics.Error(Context(pulse) + " Hata DB'ye de yazılamadı: " + storageError); }
    }

    private static string Context(RelayPulse p)
    { return string.Format("Pulse={0} Makine={1} Cihaz={2}:{3} Kanal={4} OFF={5:o} ON hedef={6:o} Deneme={7}",
        p.Id,p.MakineId,p.ControllerIp,p.HttpPort,p.RelayChannel,p.PulseBaslangic,p.PulseBitis,p.ReleaseDenemeSayisi); }

    public void ReportWorkerError(Exception ex)
    {
        WorkerError = "KRİTİK: Röle güvenlik takibi hata verdi. " + ex.Message;
        RelayPulseDiagnostics.Error("Röle watchdog: " + ex);
    }

    // Süreler, denemeler ve bekleyen işlemler SQL'de tutulur; bu bekleme yalnız tarama aralığıdır.
    public async Task RunWatchdog(CancellationToken stopping)
    {
        var pending = new List<Task>();
        try
        {
            try { pending.Add(RecoverPendingPulses()); }
            catch (Exception ex) { ReportWorkerError(ex); }
            while (!stopping.IsCancellationRequested)
            {
                try { pending.Add(ReleaseExpiredPulses()); }
                catch (Exception ex) { ReportWorkerError(ex); }
                pending.RemoveAll(t => t.Status == TaskStatus.RanToCompletion);
                await Task.Delay(250, stopping).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) { }
        finally { await Task.WhenAll(pending).ConfigureAwait(false); }
    }
}
