using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using System.Text.RegularExpressions;

public static class MakineRoleIslemleri
{
    private static readonly HttpClient httpClient = new HttpClient(new HttpClientHandler
    {
        AllowAutoRedirect = false,
        UseProxy = false
    })
    {
        Timeout = TimeSpan.FromSeconds(3)
    };

    public sealed class KanalDurumSonucu
    {
        public bool Basarili { get; set; }
        public bool DuruyorMu { get; set; }
        public string HamCevap { get; set; }
        public string Hata { get; set; }
    }

    public static Task<string> MakineDurdurAsync(MakineRoleBaglantilari baglanti)
    {
        // Active-low: IO ON röleyi bırakır, COM-NO açılır.
        return KomutGonderAsync(baglanti, true);
    }

    public static Task<string> MakineBaslatAsync(MakineRoleBaglantilari baglanti)
    {
        // Active-low: IO OFF röleyi çeker, COM-NO kapanır.
        return KomutGonderAsync(baglanti, false);
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
                if (!yanit.IsSuccessStatusCode) { sonuc.Hata = Mesajlar.RoleCihazYanitiBasarisiz; return sonuc; }
                Match eslesen = Regex.Match(sonuc.HamCevap ?? string.Empty, "[01]{16}");
                if (!eslesen.Success) { sonuc.Hata = Mesajlar.RoleDurumuOkunamadi; return sonuc; }
                int indeks = 16 - baglanti.KanalNo;
                sonuc.DuruyorMu = eslesen.Value[indeks] == '1';
                sonuc.Basarili = true;
            }
        }
        catch (TaskCanceledException) { sonuc.Hata = Mesajlar.RoleCihazZamanAsimi; }
        catch (HttpRequestException) { sonuc.Hata = Mesajlar.RoleCihazinaUlasilamadi; }
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

    private static async Task<string> KomutGonderAsync(MakineRoleBaglantilari baglanti, bool durdur)
    {
        if (baglanti == null || !baglanti.AktifMi || baglanti.KanalNo<1 || baglanti.KanalNo>16)
        {
            return Mesajlar.MakineRoleAtamasiYok;
        }

        IPAddress ip;
        if (!IPAddress.TryParse(baglanti.Ip, out ip) || ip.AddressFamily!=AddressFamily.InterNetwork
            || baglanti.HttpPort<1 || baglanti.HttpPort>65535)
        {
            return Mesajlar.RoleCihazAyariGecersiz;
        }
        // Her kanalın OFF komutu çift, ON komutu bir sonraki tek sayıdır.
        int komutNo=(baglanti.KanalNo-1)*2+(durdur ? 1 : 0);
        Uri komutAdresi=new UriBuilder(Uri.UriSchemeHttp,ip.ToString(),baglanti.HttpPort,
            komutNo.ToString("D2",System.Globalization.CultureInfo.InvariantCulture)).Uri;

        try
        {
            using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(ConfigInt("Hw584TimeoutSeconds", 3))))
            using (HttpResponseMessage yanit = await httpClient.GetAsync(komutAdresi, cts.Token).ConfigureAwait(false))
            {
                return yanit.IsSuccessStatusCode ? null : Mesajlar.RoleCihazYanitiBasarisiz;
            }
        }
        catch (TaskCanceledException)
        {
            return Mesajlar.RoleCihazZamanAsimi;
        }
        catch (HttpRequestException)
        {
            return Mesajlar.RoleCihazinaUlasilamadi;
        }
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
        return int.TryParse(ConfigurationManager.AppSettings[anahtar], out deger) && deger > 0 ? deger : varsayilan;
    }
}
