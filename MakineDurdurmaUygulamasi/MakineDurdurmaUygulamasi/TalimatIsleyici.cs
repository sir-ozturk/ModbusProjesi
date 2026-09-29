using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MakineDurdurmaUygulamasi
{
    public sealed class TalimatIsleyici
    {
        private readonly Func<VeritabaniIslemleri> baglantiAc;
        private readonly RelayPulseService role;
        private readonly int gecerlilikSaniye;
        private int calisiyor;

        public bool Calisiyor { get { return Volatile.Read(ref calisiyor) == 1; } }

        private readonly int kullaniciId;
        private readonly string ip;

        public TalimatIsleyici(string baglantiMetni, int kullaniciId, string ip, int gecerlilikSaniye)
            : this(() =>
            {
                VeritabaniIslemleri veritabani = new VeritabaniIslemleri();
                veritabani.OturumBaslat(baglantiMetni);
                return veritabani;
            }, kullaniciId, ip, gecerlilikSaniye, RelayPulseService.Default) { }

        // Testler aynı akışı sahte veritabanı ve röle ile çalıştırır.
        internal TalimatIsleyici(Func<VeritabaniIslemleri> baglantiAc, int kullaniciId, string ip,
            int gecerlilikSaniye, RelayPulseService role)
        {
            if (baglantiAc == null) throw new ArgumentNullException("baglantiAc");
            if (kullaniciId <= 0 || string.IsNullOrWhiteSpace(ip) || ip.Length > 50)
                throw new ArgumentException("İşleyici kullanıcı kimliği ve IP bilgisi gereklidir.");
            if (gecerlilikSaniye < 1 || gecerlilikSaniye > 86400)
                throw new ArgumentOutOfRangeException("gecerlilikSaniye");
            this.baglantiAc = baglantiAc;
            this.kullaniciId = kullaniciId;
            this.ip = ip;
            this.gecerlilikSaniye = gecerlilikSaniye;
            this.role = role ?? RelayPulseService.Default;
        }

        // Form task'ı saklayıp STOP/EXIT'te token'ı iptal eder ve task'ın bitmesini bekler.
        // Token cihaz çağrılarına verilmez: alınmış talimat ve OFF dönüşü yarıda kesilmez.
        public Task CalistirAsync(CancellationToken durdur, IProgress<string> durum = null)
        {
            if (Interlocked.CompareExchange(ref calisiyor, 1, 0) != 0)
                throw new InvalidOperationException("Talimat işleyicisi zaten çalışıyor.");
            return Task.Run(async () =>
            {
                try
                {
                    VeritabaniIslemleri veritabani = await BaglantiAcAsync(durdur, durum).ConfigureAwait(false);
                    if (veritabani == null) return;
                    try
                    {
                        veritabani.OturumKilidiAl("ModbusTalimatIsleyici", false);
                        MakineDurdurmaTalimatlari talimatlar = new MakineDurdurmaTalimatlari(veritabani)
                        { GuncelleyenId = kullaniciId, GuncelleyenIp = ip };
                        var yarim = talimatlar.YarimKalanlariGetir();
                        foreach (var talimat in yarim)
                        {
                            await YarimKalaniKapat(talimatlar, talimat).ConfigureAwait(false);
                        }
                        if (yarim.Count > 0)
                            throw new InvalidOperationException("Yarım kalan talimatlar Kontrol Gerekli olarak kaydedildi. Kontrol sonrası yeniden RUN kullanınız.");

                        while (!durdur.IsCancellationRequested)
                        {
                            // Alma yanıtı kaybolduysa kayıt sahiplenilmiş olabilir: hata halinde yeniden alma yok.
                            var talimat = talimatlar.SiradakiniAl(gecerlilikSaniye);
                            if (talimat == null)
                            {
                                Bildir(durum, "Çalışıyor — bekleyen talimat yok.");
                                await Task.Delay(1000, durdur).ConfigureAwait(false);
                                continue;
                            }
                            Bildir(durum, "Talimat işleniyor: " + talimat.Id);
                            await Isle(talimatlar, talimat).ConfigureAwait(false);
                            Bildir(durum, "Talimat sonuçlandı: " + talimat.Id);
                        }
                    }
                    finally { veritabani.Bitir(); }
                }
                catch (OperationCanceledException) when (durdur.IsCancellationRequested) { }
                catch (Exception ex)
                {
                    RelayPulseService.LogError("Talimat işleyicisi durdu: " + ex);
                    Bildir(durum, "Durduruldu: " + ex.Message);
                    throw;
                }
                finally { Interlocked.Exchange(ref calisiyor, 0); }
            });
        }

        private async Task<VeritabaniIslemleri> BaglantiAcAsync(CancellationToken token, IProgress<string> durum)
        {
            while (!token.IsCancellationRequested)
            {
                try { return baglantiAc(); }
                // Yalnızca henüz oturum/iş alınmadan bağlantı açma hatası yeniden denenir.
                catch (SqlException ex)
                {
                    RelayPulseService.LogError("Talimat veritabanı bağlantısı: " + ex.Message);
                    Bildir(durum, "Veritabanına bağlanılamadı; yeniden denenecek.");
                    await Task.Delay(1000, token).ConfigureAwait(false);
                }
            }
            return null;
        }

        private async Task Isle(MakineDurdurmaTalimatlari talimatlar, MakineDurdurmaTalimatlari talimat)
        {
            RelayPulseResult pulse = null;
            try
            {
                MakineRoleBaglantilari baglanti;
                try { baglanti = talimatlar.BaglantiyiKilitle(talimat.MakineId); }
                catch (DonanimIslemHatasi ex)
                {
                    talimatlar.Sonuclandir(talimat, TalimatDurumu.Hatali, ex.Message, false, false);
                    return;
                }
                string hata = AdresKontrol(talimat, baglanti) ?? talimatlar.KomutOncesiKontrol(talimat, gecerlilikSaniye);
                if (hata != null)
                {
                    talimatlar.Sonuclandir(talimat, TalimatDurumu.Hatali, hata, false, false);
                    return;
                }
                pulse = await role.TriggerStopPulse(baglanti).ConfigureAwait(false);
                bool tamam = pulse.OnDogrulandi && pulse.OffDogrulandi && pulse.Hata == null;
                var durum = tamam ? TalimatDurumu.Tamamlandi
                    : pulse.KomutGonderildi ? TalimatDurumu.KontrolGerekli : TalimatDurumu.Hatali;
                talimatlar.Sonuclandir(talimat, durum,
                    tamam ? "ON ve OFF doğrulandı; durdurma sinyali tamamlandı." : pulse.Hata ?? "Röle sonucu belirsiz.",
                    pulse.OnDogrulandi, pulse.OffDogrulandi);
                if (durum == TalimatDurumu.KontrolGerekli)
                    throw new InvalidOperationException("Talimat " + talimat.Id + " kontrol gerektiriyor. Yeni iş alınmadı.");
            }
            catch (Exception ex)
            {
                // DB kaydı başarısız olsa bile kanıt yerel hata kaydında kalsın.
                RelayPulseService.LogError("Talimat " + talimat.Id + " makine " + talimat.MakineId
                    + " ON=" + (pulse != null && pulse.OnDogrulandi)
                    + " OFF=" + (pulse != null && pulse.OffDogrulandi) + ": " + ex);
                throw;
            }
            finally { talimatlar.KomutKilitleriniBirak(); }
        }

        private async Task YarimKalaniKapat(MakineDurdurmaTalimatlari talimatlar, MakineDurdurmaTalimatlari talimat)
        {
            try
            {
                var baglanti = talimatlar.BaglantiyiKilitle(talimat.MakineId);
                string hata = AdresKontrol(talimat, baglanti);
                bool off = hata == null && await role.GuvenliOffAsync(baglanti).ConfigureAwait(false);
                talimatlar.Sonuclandir(talimat, TalimatDurumu.KontrolGerekli,
                    "Yarım kalan talimat; önceki ON sonucu bilinmiyor. "
                    + (hata ?? (off ? "OFF doğrulandı." : "OFF doğrulanamadı.")), false, off);
            }
            finally { talimatlar.KomutKilitleriniBirak(); }
        }

        public static string AdresKontrol(MakineDurdurmaTalimatlari talimat, MakineRoleBaglantilari baglanti)
        {
            IPAddress ip;
            if (talimat == null || baglanti == null || !baglanti.AktifMi || baglanti.MakineId != talimat.MakineId
                || baglanti.KanalNo < 1 || baglanti.KanalNo > 16 || baglanti.HttpPort < 1 || baglanti.HttpPort > 65535
                || !IPAddress.TryParse(baglanti.Ip, out ip) || ip.AddressFamily != AddressFamily.InterNetwork)
                return "Aktif ve geçerli makine röle bağlantısı bulunamadı.";
            Uri url;
            if (!Uri.TryCreate(talimat.Url, UriKind.Absolute, out url) || url.Scheme != Uri.UriSchemeHttp
                || url.UserInfo.Length != 0 || url.Query.Length != 0 || url.Fragment.Length != 0
                || url.Host != ip.ToString() || url.Port != baglanti.HttpPort
                || url.AbsolutePath != "/" + ((baglanti.KanalNo - 1) * 2 + 1).ToString("D2", System.Globalization.CultureInfo.InvariantCulture))
                return "Talimat URL'si makinenin güncel röle bağlantısıyla uyuşmuyor.";
            return null;
        }

        private static void Bildir(IProgress<string> durum, string mesaj)
        {
            // Görünüm hatası cihaz akışını kesmemeli. Form Progress<string> kullanabilir.
            try { if (durum != null) durum.Report(mesaj); }
            catch (Exception ex) { RelayPulseService.LogError("Durum gösterilemedi: " + ex.Message); }
        }
    }
}
