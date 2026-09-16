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
            if (gecikme > 0) Kontrol(sure.Elapsed.TotalSeconds >= 3 && sure.Elapsed.TotalSeconds < 5.5, "4 saniye zaman asimi");
            Kontrol(await istek == "GET /" + (((baglanti.KanalNo-1)*2)+(baslat ? 0 : 1)).ToString("D2") + " HTTP/1.1", "Secilen kanalin GET komutu");
        }
        finally { sunucu.Stop(); }
    }

    private static async Task Calistir()
    {
        MakineRoleBaglantilari baglanti = new MakineRoleBaglantilari(null) { KanalNo=1, AktifMi=true };
        await YanitTesti(baglanti, false, "200 OK", null, 0);
        await YanitTesti(baglanti, true, "200 OK", null, 0);
        baglanti.KanalNo=16;
        await YanitTesti(baglanti, false, "200 OK", null, 0);
        await YanitTesti(baglanti, true, "200 OK", null, 0);
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
