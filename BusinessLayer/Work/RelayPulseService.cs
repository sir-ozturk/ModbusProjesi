using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public sealed class RelayPulseResult
{
    public bool OnDogrulandi { get; set; }
    public bool OffDogrulandi { get; set; }
    public bool KomutGonderildi { get; set; }
    public string Hata { get; set; }
}

// Çağrı içinde ON -> 3 saniye -> OFF. Web veya talimat işleyicisi çağırabilir.
public sealed class RelayPulseService
{
    public static readonly RelayPulseService Default = new RelayPulseService();
    private static readonly ConcurrentDictionary<string, byte> busy = new ConcurrentDictionary<string, byte>();
    private readonly Func<MakineRoleBaglantilari, bool, Task<MakineRoleIslemleri.KanalDurumSonucu>> set;
    private readonly Func<MakineRoleBaglantilari, Task<MakineRoleIslemleri.KanalDurumSonucu>> read;
    private readonly Func<int, Task> delay;

    public RelayPulseService()
        : this((c, on) => on ? MakineRoleIslemleri.SetRelayOn(c) : MakineRoleIslemleri.SetRelayOff(c),
            MakineRoleIslemleri.ReadRelayStatus, ms => Task.Delay(ms)) { }

    // Testler gerçek röle yerine sahte çağrılar ve bekleme kullanabilir.
    public RelayPulseService(
        Func<MakineRoleBaglantilari, bool, Task<MakineRoleIslemleri.KanalDurumSonucu>> set,
        Func<MakineRoleBaglantilari, Task<MakineRoleIslemleri.KanalDurumSonucu>> read,
        Func<int, Task> delay)
    { this.set = set; this.read = read; this.delay = delay; }

    public async Task<RelayPulseResult> TriggerStopPulse(MakineRoleBaglantilari connection)
    {
        var result = new RelayPulseResult();
        if (connection == null || !connection.AktifMi || connection.KanalNo < 1 || connection.KanalNo > 16)
        { result.Hata = Mesajlar.MakineRoleAtamasiYok; return result; }
        string key = connection.Ip + ":" + connection.HttpPort;
        if (!busy.TryAdd(key, 0))
        { result.Hata = "Bu röle cihazında başka bir işlem devam ediyor. Tekrar deneyiniz."; return result; }
        bool returnOff = false;
        try
        {
            // Yanıt kaybolsa da komut ulaşmış olabilir; durum okunana kadar OFF dönüşü gerekir.
            returnOff = true;
            result.KomutGonderildi = true;
            await set(connection, true).ConfigureAwait(false);
            var state = await read(connection).ConfigureAwait(false);
            if (state.Basarili && !state.IoOn)
            {
                returnOff = false;
                result.OffDogrulandi = true;
                result.Hata = "ON doğrulanamadı; röle OFF durumunda. Durdurma yapılmadı.";
            }
            else if (!state.Basarili)
                result.Hata = "ON durumu okunamadı; durdurma doğrulanamadı. Tedbir olarak OFF dönüşü denendi.";
            else
            {
                result.OnDogrulandi = true;
                await delay(3000).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Log(connection, ex.ToString());
            result.Hata = "Röle işlemi sırasında hata oluştu; OFF dönüşü denendi.";
        }
        finally
        {
            try
            {
                if (returnOff)
                {
                    result.OffDogrulandi = await ReturnOff(connection).ConfigureAwait(false);
                    if (!result.OffDogrulandi)
                        result.Hata = "Rölenin OFF dönüşü 3 denemede doğrulanamadı. Cihazı kontrol ediniz.";
                }
            }
            finally { byte ignored; busy.TryRemove(key, out ignored); }
        }
        if (result.Hata != null) Log(connection, result.Hata);
        return result;
    }

    // Açılışta yarım kalan talimat için sadece OFF dönüşü; asla ON göndermez.
    public async Task<bool> GuvenliOffAsync(MakineRoleBaglantilari connection)
    {
        if (connection == null || !connection.AktifMi || connection.KanalNo < 1 || connection.KanalNo > 16)
            return false;
        string key = connection.Ip + ":" + connection.HttpPort;
        if (!busy.TryAdd(key, 0)) return false;
        try { return await ReturnOff(connection).ConfigureAwait(false); }
        finally { byte ignored; busy.TryRemove(key, out ignored); }
    }

    private async Task<bool> ReturnOff(MakineRoleBaglantilari connection)
    {
        for (int attempt = 1; attempt <= 3; attempt++)
        {
            // Komut yanıtı alınamasa da /98 doğrulaması yapılır.
            try { await set(connection, false).ConfigureAwait(false); }
            catch (Exception ex) { Log(connection, "OFF " + attempt + ": " + ex); }
            try
            {
                var state = await read(connection).ConfigureAwait(false);
                if (state.Basarili && !state.IoOn) return true;
                Log(connection, "OFF " + attempt + " doğrulanamadı: " + state.Hata);
            }
            catch (Exception ex) { Log(connection, "OFF durum okuma: " + ex); }
            if (attempt < 3)
            {
                try { await delay(500).ConfigureAwait(false); }
                catch (Exception ex) { Log(connection, "Tekrar beklemesi: " + ex); }
            }
        }
        return false;
    }

    private static void Log(MakineRoleBaglantilari c, string message)
    { LogError("Makine " + c.MakineId + " / " + c.Ip + ":" + c.HttpPort + " kanal " + c.KanalNo + ": " + message); }

    private static readonly object logGate = new object();

    public static void LogError(string message)
    {
        // Log hatası OFF dönüşünü veya kullanıcıya hata gösterilmesini engellemez.
        try { Trace.TraceError(message); } catch { }
        try
        {
            lock (logGate)
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(3));
                string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
                Directory.CreateDirectory(directory);
                File.AppendAllText(Path.Combine(directory, "RelayPulseCritical-" + now.ToString("yyyyMMdd") + ".log"),
                    now.ToString("o") + " " + message + Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            try { Trace.TraceError("Röle hata kaydı yazılamadı: " + ex); } catch { }
        }
    }
}
