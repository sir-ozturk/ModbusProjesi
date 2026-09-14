using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

public static class MakineRoleIslemleri
{
    public static bool DurumdanDuruyorMu(string durum, int relayChannel)
    {
        if (durum == null || durum.Length != 16 || relayChannel < 1 || relayChannel > 16)
            throw new ArgumentException("Geçersiz IO durumu veya röle kanalı.");
        foreach (char bit in durum)
            if (bit != '0' && bit != '1')
                throw new ArgumentException("IO yanıtı yalnızca 16 adet 0/1 içermelidir.");
        return durum[16 - relayChannel] == '1';
    }

    public static async Task<string> DurumOkuAsync()
    {
        Uri adres;
        if (!Uri.TryCreate(ConfigurationManager.AppSettings["RoleCihazAdresi"], UriKind.Absolute, out adres)
            || adres.Scheme != Uri.UriSchemeHttp || adres.AbsolutePath != "/"
            || !string.IsNullOrEmpty(adres.Query) || !string.IsNullOrEmpty(adres.Fragment)
            || !string.IsNullOrEmpty(adres.UserInfo))
            throw new InvalidOperationException("Röle cihaz adresi geçersiz.");
        using (HttpResponseMessage yanit = await httpClient.GetAsync(new Uri(adres, "98")).ConfigureAwait(false))
        {
            yanit.EnsureSuccessStatusCode();
            string durum = (await yanit.Content.ReadAsStringAsync().ConfigureAwait(false)).Trim();
            DurumdanDuruyorMu(durum, 1);
            return durum;
        }
    }

    private static readonly HttpClient httpClient = new HttpClient(new HttpClientHandler
    {
        AllowAutoRedirect = false,
        UseProxy = false
    })
    {
        Timeout = TimeSpan.FromSeconds(4)
    };

    public static bool RoleBirMakinesiMi(string makineNo)
    {
        string atananMakineNo = ConfigurationManager.AppSettings["RoleBirMakineNo"];
        return !string.IsNullOrWhiteSpace(atananMakineNo)
            && string.Equals(atananMakineNo.Trim(), (makineNo ?? "").Trim(), StringComparison.Ordinal);
    }

    public static Task<string> MakineDurdurAsync(Makineler makine)
    {
        // Active-low: IO1 ON röleyi bırakır, COM-NO açılır.
        return KomutGonderAsync(makine, "01");
    }

    public static Task<string> MakineBaslatAsync(Makineler makine)
    {
        // Active-low: IO1 OFF röleyi çeker, COM-NO kapanır.
        return KomutGonderAsync(makine, "00");
    }

    private static async Task<string> KomutGonderAsync(Makineler makine, string komut)
    {
        if (makine == null || !makine.AktifMi || makine.RelayChannel != 1 || !RoleBirMakinesiMi(makine.MakineNo))
        {
            return Mesajlar.MakineRoleAtamasiYok;
        }

        Uri cihazAdresi;
        if (!Uri.TryCreate(ConfigurationManager.AppSettings["RoleCihazAdresi"], UriKind.Absolute, out cihazAdresi)
            || cihazAdresi.Scheme != Uri.UriSchemeHttp
            || cihazAdresi.AbsolutePath != "/"
            || !string.IsNullOrEmpty(cihazAdresi.Query)
            || !string.IsNullOrEmpty(cihazAdresi.Fragment)
            || !string.IsNullOrEmpty(cihazAdresi.UserInfo))
        {
            return Mesajlar.RoleCihazAyariGecersiz;
        }

        try
        {
            using (HttpResponseMessage yanit = await httpClient.GetAsync(new Uri(cihazAdresi, komut)).ConfigureAwait(false))
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
}
