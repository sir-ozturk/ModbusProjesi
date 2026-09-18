using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;


public static class MakineRoleIslemleri
{
    // true = ON (normal / enerjisiz). Makinenin çalışma durumunu bildirmez.
    public static bool GetRelayChannelState(string status, int channel)
    {
        return !DurumdanDuruyorMu(status, channel);
    }
    public static bool DurumdanDuruyorMu(string durum, int relayChannel)
    {
        if (durum == null || durum.Length != 16 || relayChannel < 1 || relayChannel > 16)
            throw new ArgumentException("Geçersiz IO durumu veya röle kanalı.");
        foreach (char bit in durum)
            if (bit != '0' && bit != '1')
                throw new ArgumentException("IO yanıtı yalnızca 16 adet 0/1 içermelidir.");
        // COM-NO hedef bağlantı: IO 0 = duruş, IO 1 = röle kaynaklı hata kaldırılmış.
        // Kanal 1 en sağdaki bittir; bu bilgi motor hareketinin geri bildirimi değildir.
        return durum[16 - relayChannel] == '0';
    }

    private static readonly HttpClient httpClient = new HttpClient(new HttpClientHandler
    {
        AllowAutoRedirect = false,
        UseProxy = false,
        // Tek cihaz 16 kanala hizmet verir; eşzamanlı bağlantılarla cihaz yükü artırılmaz.
        MaxConnectionsPerServer = 1
    })
    {
        Timeout = System.Threading.Timeout.InfiniteTimeSpan,
        MaxResponseContentBufferSize = 4096
    };

    static MakineRoleIslemleri()
    {
        httpClient.DefaultRequestHeaders.ConnectionClose = true;
    }

    public sealed class KanalDurumSonucu
    {
        public bool Basarili { get; set; }
        public bool DuruyorMu { get; set; }
        public string HamCevap { get; set; }
        public string Hata { get; set; }
        public string HataDetayi { get; set; }
        public bool IoOn { get { return !DuruyorMu; } }
    }

    public static Task<string> MakineDurdurAsync(MakineRoleBaglantilari baglanti)
    {
        // COM-NO bağlantı için OFF = makineyi durdur.
        return KomutGonderAsync(baglanti, false);
    }

    public static Task<string> MakineBaslatAsync(MakineRoleBaglantilari baglanti)
    {
        // ON = normal / röle bırakılmış. Makine duruş kaydını değiştirmez.
        // Bu komut motorun fiziksel olarak çalıştığını doğrulamaz.
        return KomutGonderAsync(baglanti, true);
    }

    public static async Task<KanalDurumSonucu> KanalDurumunuGetirAsync(MakineRoleBaglantilari baglanti)
    {
        KanalDurumSonucu sonuc = new KanalDurumSonucu();
        Uri adres;
        string hata = CihazAdresiniDogrula(baglanti, "98", out adres);
        if (hata != null) { sonuc.Hata = hata; return sonuc; }
        try
        {
            using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(ConfigInt("Hw584TimeoutSeconds", 3))))
            using (HttpResponseMessage yanit = await httpClient.GetAsync(adres, cts.Token).ConfigureAwait(false))
            {
                sonuc.HamCevap = await yanit.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!yanit.IsSuccessStatusCode) { sonuc.Hata = Mesajlar.RoleCihazYanitiBasarisiz; sonuc.HataDetayi = "HTTP " + (int)yanit.StatusCode; return sonuc; }
                try { sonuc.DuruyorMu = DurumdanDuruyorMu((sonuc.HamCevap ?? string.Empty).Trim(), baglanti.KanalNo); }
                catch (ArgumentException) { sonuc.Hata = Mesajlar.RoleDurumuOkunamadi; return sonuc; }
                sonuc.Basarili = true;
            }
        }
        catch (TaskCanceledException ex) { sonuc.Hata = Mesajlar.RoleCihazZamanAsimi; sonuc.HataDetayi = ex.ToString(); }
        catch (HttpRequestException ex) { sonuc.Hata = Mesajlar.RoleCihazinaUlasilamadi; sonuc.HataDetayi = ex.ToString(); }
        return sonuc;
    }

    public static async Task<string> KanalDurumunuDogrulaAsync(MakineRoleBaglantilari baglanti, bool beklenenDuruyorMu)
    {
        int deneme = ConfigInt("RelayVerificationAttempts", 4);
        int bekleme = ConfigInt("RelayVerificationWaitMilliseconds", 150);
        for (int i = 0; i < deneme; i++)
        {
            if (i > 0) await Task.Delay(bekleme).ConfigureAwait(false);
            KanalDurumSonucu durum = await KanalDurumunuGetirAsync(baglanti).ConfigureAwait(false);
            if (!durum.Basarili) return durum.Hata;
            if (durum.DuruyorMu == beklenenDuruyorMu) return null;
        }
        return Mesajlar.RoleDurumuDogrulanamadi;
    }

    private static async Task<string> KomutGonderAsync(MakineRoleBaglantilari baglanti, bool ioOn)
    {
        return (await SetRelayState(baglanti, ioOn).ConfigureAwait(false)).Hata;
    }

    public static Task<KanalDurumSonucu> SetRelayOn(MakineRoleBaglantilari connection) { return SetRelayState(connection, true); }
    public static Task<KanalDurumSonucu> SetRelayOff(MakineRoleBaglantilari connection) { return SetRelayState(connection, false); }
    public static Task<KanalDurumSonucu> ReadRelayStatus(MakineRoleBaglantilari connection) { return KanalDurumunuGetirAsync(connection); }

    private static async Task<KanalDurumSonucu> SetRelayState(MakineRoleBaglantilari baglanti, bool ioOn)
    {
        var sonuc = new KanalDurumSonucu();
        if (baglanti == null || !baglanti.AktifMi || baglanti.KanalNo<1 || baglanti.KanalNo>16)
        {
            sonuc.Hata = Mesajlar.MakineRoleAtamasiYok; return sonuc;
        }

        IPAddress ip;
        if (!IPAddress.TryParse(baglanti.Ip, out ip) || ip.AddressFamily!=AddressFamily.InterNetwork
            || baglanti.HttpPort<1 || baglanti.HttpPort>65535)
        {
            sonuc.Hata = Mesajlar.RoleCihazAyariGecersiz; return sonuc;
        }
        // Her kanalın OFF komutu çift, ON komutu bir sonraki tek sayıdır.
        int komutNo=(baglanti.KanalNo-1)*2+(ioOn ? 1 : 0);
        Uri komutAdresi=new UriBuilder(Uri.UriSchemeHttp,ip.ToString(),baglanti.HttpPort,
            komutNo.ToString("D2",System.Globalization.CultureInfo.InvariantCulture)).Uri;

        try
        {
            using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(ConfigInt("Hw584TimeoutSeconds", 3))))
            using (HttpResponseMessage yanit = await httpClient.GetAsync(komutAdresi, cts.Token).ConfigureAwait(false))
            {
                sonuc.HamCevap = await yanit.Content.ReadAsStringAsync().ConfigureAwait(false);
                sonuc.Basarili = yanit.IsSuccessStatusCode;
                sonuc.Hata = sonuc.Basarili ? null : Mesajlar.RoleCihazYanitiBasarisiz;
                if (!sonuc.Basarili) sonuc.HataDetayi = "HTTP " + (int)yanit.StatusCode;
            }
        }
        catch (TaskCanceledException ex)
        {
            sonuc.Hata = Mesajlar.RoleCihazZamanAsimi; sonuc.HataDetayi = ex.ToString();
        }
        catch (HttpRequestException ex)
        {
            sonuc.Hata = Mesajlar.RoleCihazinaUlasilamadi; sonuc.HataDetayi = ex.ToString();
        }
        return sonuc;
    }

    private static string CihazAdresiniDogrula(MakineRoleBaglantilari baglanti, string yol, out Uri adres)
    {
        adres = null;
        if (baglanti == null || !baglanti.AktifMi || baglanti.KanalNo < 1 || baglanti.KanalNo > 16) return Mesajlar.MakineRoleAtamasiYok;
        IPAddress ip;
        if (!IPAddress.TryParse(baglanti.Ip, out ip) || ip.AddressFamily != AddressFamily.InterNetwork || baglanti.HttpPort < 1 || baglanti.HttpPort > 65535) return Mesajlar.RoleCihazAyariGecersiz;
        adres = new UriBuilder(Uri.UriSchemeHttp, ip.ToString(), baglanti.HttpPort, yol).Uri;
        return null;
    }

    private static int ConfigInt(string anahtar, int varsayilan)
    {
        int deger;
        deger = int.TryParse(ConfigurationManager.AppSettings[anahtar], out deger) && deger > 0 ? deger : varsayilan;
        return anahtar == "Hw584TimeoutSeconds" ? Math.Min(3, deger) : deger;
    }
}
