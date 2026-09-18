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

    private static async Task YanitTesti(MakineRoleBaglantilari baglanti, bool baslat, string durum, string beklenenHata, int gecikme)
    {
        TcpListener sunucu = new TcpListener(IPAddress.Loopback, 0);
        sunucu.Start();
        baglanti.Ip="127.0.0.1";
        baglanti.HttpPort=((IPEndPoint)sunucu.LocalEndpoint).Port;
        Task<string> istek = Task.Run(async () =>
        {
            using (TcpClient tcpBaglanti = await sunucu.AcceptTcpClientAsync())
            using (NetworkStream akis = tcpBaglanti.GetStream())
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
            string hata = baslat ? await MakineRoleIslemleri.MakineBaslatAsync(baglanti)
                : await MakineRoleIslemleri.MakineDurdurAsync(baglanti);
            Kontrol(hata == beklenenHata, "HTTP " + durum + " / hata sonucu");
            if (gecikme > 0) Kontrol(sure.Elapsed.TotalSeconds >= 3 && sure.Elapsed.TotalSeconds < 5.5, "3 saniye zaman asimi");
            string[] offYollari = { "00", "02", "04", "06", "08", "10", "12", "14", "16", "18", "20", "22", "24", "26", "28", "30" };
            string[] onYollari = { "01", "03", "05", "07", "09", "11", "13", "15", "17", "19", "21", "23", "25", "27", "29", "31" };
            string beklenenYol = (baslat ? onYollari : offYollari)[baglanti.KanalNo - 1];
            Kontrol(await istek == "GET /" + beklenenYol + " HTTP/1.1", "Kanal " + baglanti.KanalNo + (baslat ? " Baslat ON /" : " Durdur OFF /") + beklenenYol);
        }
        finally { sunucu.Stop(); }
    }

    private static async Task DurumOkumaTesti(string govde, string durum, bool basarili)
    {
        TcpListener sunucu = new TcpListener(IPAddress.Loopback, 0);
        sunucu.Start();
        MakineRoleBaglantilari baglanti = new MakineRoleBaglantilari(null) { AktifMi = true, KanalNo = 1, Ip = "127.0.0.1", HttpPort = ((IPEndPoint)sunucu.LocalEndpoint).Port };
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
            bool duruyorMu = false;
            string sonuc = null;
            try { var cevap = await MakineRoleIslemleri.KanalDurumunuGetirAsync(baglanti); sonuc = cevap.HamCevap == null ? null : cevap.HamCevap.Trim(); okundu = cevap.Basarili; duruyorMu = cevap.DuruyorMu; }
            catch (ArgumentException) { }
            catch (System.Net.Http.HttpRequestException) { }
            Kontrol(okundu == basarili, "Durum okuma: " + durum + "/" + govde);
            if (basarili) Kontrol(sonuc == govde.Trim(), "IO yanıtı korunur");
            if (basarili) Kontrol(duruyorMu == (govde.Trim()[15] == '0'), "IO yorumu: OFF tetik, ON normal");
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
                Kontrol(MakineRoleIslemleri.DurumdanDuruyorMu(new string(bitler), okunan) == (kanal != okunan), "IO bit sırası: yalnız belirtilen kanal ON " + kanal + "/" + okunan);
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

        MakineRoleBaglantilari baglanti = new MakineRoleBaglantilari(null) { KanalNo=1, AktifMi=true };
        for (int kanal = 1; kanal <= 16; kanal++)
        {
            baglanti.KanalNo = kanal;
            await YanitTesti(baglanti, false, "200 OK", null, 0);
            await YanitTesti(baglanti, true, "200 OK", null, 0);
        }
        await YanitTesti(baglanti, false, "500 Error", Mesajlar.RoleCihazYanitiBasarisiz, 0);
        await YanitTesti(baglanti, false, "302 Found", Mesajlar.RoleCihazYanitiBasarisiz, 0);
        await YanitTesti(baglanti, false, "timeout", Mesajlar.RoleCihazZamanAsimi, 4600);
        await YanitTesti(baglanti, false, "invalid", Mesajlar.RoleCihazinaUlasilamadi, 0);
        baglanti.KanalNo=0;
        Kontrol(await MakineRoleIslemleri.MakineDurdurAsync(baglanti)==Mesajlar.MakineRoleAtamasiYok,"Kanal 0 reddedilir");
        baglanti.KanalNo=17;
        Kontrol(await MakineRoleIslemleri.MakineDurdurAsync(baglanti)==Mesajlar.MakineRoleAtamasiYok,"Kanal 17 reddedilir");
        baglanti.KanalNo=1; baglanti.AktifMi=false;
        Kontrol(await MakineRoleIslemleri.MakineBaslatAsync(baglanti)==Mesajlar.MakineRoleAtamasiYok,"Pasif baglanti reddedilir");
        baglanti.AktifMi=true; baglanti.HttpPort=65536;
        Kontrol(await MakineRoleIslemleri.MakineBaslatAsync(baglanti)==Mesajlar.RoleCihazAyariGecersiz,"Gecersiz HTTP portu");
        baglanti.HttpPort=8080; baglanti.Ip="hatali";
        Kontrol(await MakineRoleIslemleri.MakineBaslatAsync(baglanti)==Mesajlar.RoleCihazAyariGecersiz,"Gecersiz IP");

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
