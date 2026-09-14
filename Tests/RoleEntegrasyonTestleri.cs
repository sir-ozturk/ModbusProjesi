using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

internal static class RoleEntegrasyonTestleri
{
    private static int kontrolSayisi;

    private static void Kontrol(bool sonuc, string ad)
    {
        if (!sonuc) throw new Exception(ad);
        kontrolSayisi++;
        Console.WriteLine("OK: " + ad);
    }

    private static void Ayar(string anahtar, string deger)
    {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.AppSettings.Settings.Remove(anahtar);
        config.AppSettings.Settings.Add(anahtar, deger);
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
    }

    private static async Task YanitTesti(Makineler makine, bool baslat, string durum, string beklenenHata, int gecikme)
    {
        TcpListener sunucu = new TcpListener(IPAddress.Loopback, 0);
        sunucu.Start();
        Ayar("RoleCihazAdresi", "http://127.0.0.1:" + ((IPEndPoint)sunucu.LocalEndpoint).Port + "/");
        Task<string> istek = Task.Run(async () =>
        {
            using (TcpClient baglanti = await sunucu.AcceptTcpClientAsync())
            using (NetworkStream akis = baglanti.GetStream())
            using (StreamReader okuyucu = new StreamReader(akis))
            {
                string ilkSatir = await okuyucu.ReadLineAsync();
                while (!string.IsNullOrEmpty(await okuyucu.ReadLineAsync())) { }
                if (gecikme > 0) await Task.Delay(gecikme);
                else
                {
                    byte[] cevap = Encoding.ASCII.GetBytes("HTTP/1.1 " + durum + "\r\nLocation: /55\r\nContent-Length: 0\r\nConnection: close\r\n\r\n");
                    if (durum == "invalid") cevap = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked\r\nConnection: close\r\n\r\nINVALID\r\n");
                    await akis.WriteAsync(cevap, 0, cevap.Length);
                }
                return ilkSatir;
            }
        });
        try
        {
            Stopwatch sure = Stopwatch.StartNew();
            string hata = baslat ? await MakineRoleIslemleri.MakineBaslatAsync(makine)
                : await MakineRoleIslemleri.MakineDurdurAsync(makine);
            Kontrol(hata == beklenenHata, "HTTP " + durum + " / hata sonucu");
            if (gecikme > 0) Kontrol(sure.Elapsed.TotalSeconds >= 3 && sure.Elapsed.TotalSeconds < 5.5, "4 saniye zaman asimi");
            Kontrol(await istek == "GET /" + (baslat ? "00" : "01") + " HTTP/1.1", "Yalnizca IO1 GET komutu");
        }
        finally { sunucu.Stop(); }
    }

    private static async Task DurumOkumaTesti(string govde, string durum, bool basarili)
    {
        TcpListener sunucu = new TcpListener(IPAddress.Loopback, 0);
        sunucu.Start();
        Ayar("RoleCihazAdresi", "http://127.0.0.1:" + ((IPEndPoint)sunucu.LocalEndpoint).Port + "/");
        Task<string> istek = Task.Run(async () =>
        {
            using (TcpClient istemci = await sunucu.AcceptTcpClientAsync())
            using (NetworkStream akis = istemci.GetStream())
            using (StreamReader okuyucu = new StreamReader(akis))
            {
                string yol = await okuyucu.ReadLineAsync();
                while (!string.IsNullOrEmpty(await okuyucu.ReadLineAsync())) { }
                byte[] cevap = Encoding.ASCII.GetBytes("HTTP/1.1 " + durum + "\r\nLocation: /00\r\nContent-Length: " + Encoding.ASCII.GetByteCount(govde) + "\r\nConnection: close\r\n\r\n" + govde);
                await akis.WriteAsync(cevap, 0, cevap.Length);
                return yol;
            }
        });
        try
        {
            bool okundu = false;
            string sonuc = null;
            try { sonuc = await MakineRoleIslemleri.DurumOkuAsync(); okundu = true; }
            catch (ArgumentException) { }
            catch (System.Net.Http.HttpRequestException) { }
            Kontrol(okundu == basarili, "Durum okuma: " + durum + "/" + govde);
            if (basarili) Kontrol(sonuc == govde.Trim(), "IO yanıtı korunur");
            Kontrol(await istek == "GET /98 HTTP/1.1", "Okuma yalnızca /98 kullanır");
        }
        finally { sunucu.Stop(); }
    }

    private static async Task Calistir()
    {
        await DurumOkumaTesti("0000000000000000", "200 OK", true);
        await DurumOkumaTesti(" 0000000000000001\r\n", "200 OK", true);
        await DurumOkumaTesti("1111111111111111", "200 OK", true);
        await DurumOkumaTesti("000", "200 OK", false);
        await DurumOkumaTesti("000000000000000x", "200 OK", false);
        await DurumOkumaTesti("0000000000000000", "500 Error", false);
        await DurumOkumaTesti("0000000000000000", "302 Found", false);
        for (int kanal = 1; kanal <= 16; kanal++)
        {
            char[] bitler = new string('0', 16).ToCharArray();
            bitler[16 - kanal] = '1';
            for (int okunan = 1; okunan <= 16; okunan++)
                Kontrol(MakineRoleIslemleri.DurumdanDuruyorMu(new string(bitler), okunan) == (kanal == okunan), "IO bit sırası " + kanal + "/" + okunan);
        }
        foreach (string bozuk in new[] { "", "0", "00000000000000000", "000000000000000x", "<html>hata</html>", null })
        {
            bool reddedildi = false;
            try { MakineRoleIslemleri.DurumdanDuruyorMu(bozuk, 1); }
            catch (ArgumentException) { reddedildi = true; }
            Kontrol(reddedildi, "Bozuk durum reddedilir");
        }
        foreach (int kanal in new[] { 0, 17, -1 })
        {
            bool reddedildi = false;
            try { MakineRoleIslemleri.DurumdanDuruyorMu("0000000000000000", kanal); }
            catch (ArgumentException) { reddedildi = true; }
            Kontrol(reddedildi, "Geçersiz kanal reddedilir");
        }
        Ayar("RoleBirMakineNo", "18");
        Makineler makine = new Makineler(null) { MakineNo = "18", AktifMi = true, RelayChannel = 1 };
        await YanitTesti(makine, false, "200 OK", null, 0);
        await YanitTesti(makine, true, "200 OK", null, 0);
        await YanitTesti(makine, false, "500 Error", Mesajlar.RoleCihazYanitiBasarisiz, 0);
        await YanitTesti(makine, false, "302 Found", Mesajlar.RoleCihazYanitiBasarisiz, 0);
        await YanitTesti(makine, false, "timeout", Mesajlar.RoleCihazZamanAsimi, 4600);
        // Gecersiz HTTP govdesi HttpRequestException uretir.
        await YanitTesti(makine, false, "invalid", Mesajlar.RoleCihazinaUlasilamadi, 0);
        makine.MakineNo = "1";
        Kontrol(await MakineRoleIslemleri.MakineDurdurAsync(makine) == Mesajlar.MakineRoleAtamasiYok, "Makine no 1 reddedilir");
        makine.MakineNo = "18";
        makine.AktifMi = false;
        Kontrol(await MakineRoleIslemleri.MakineBaslatAsync(makine) == Mesajlar.MakineRoleAtamasiYok, "Pasif makine reddedilir");
        makine.AktifMi = true;
        Ayar("RoleCihazAdresi", "http://127.0.0.1/55");
        Kontrol(await MakineRoleIslemleri.MakineDurdurAsync(makine) == Mesajlar.RoleCihazAyariGecersiz, "Komut iceren cihaz adresi reddedilir");
        Ayar("RoleBirMakineNo", "");
        Kontrol(await MakineRoleIslemleri.MakineBaslatAsync(makine) == Mesajlar.MakineRoleAtamasiYok, "Eksik eslesme reddedilir");
    }

    private static int Main()
    {
        try
        {
            Calistir().GetAwaiter().GetResult();
            Console.WriteLine(kontrolSayisi + " kontrol basarili.");
            return 0;
        }
        catch (Exception hata)
        {
            Console.Error.WriteLine(hata);
            return 1;
        }
    }
}
