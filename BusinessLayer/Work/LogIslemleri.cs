using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class LogIslemleri
{
    public static void IslemKaydet()
    {
        IslemNesnesi islemNesnesi = new IslemNesnesi();

        islemNesnesi.URL = "";

        try
        {
            islemNesnesi.URL = HttpContext.Current.Request.Url.ToString();
        }
        catch
        {

        }

        islemNesnesi.IpAdresi = Utility.IpNoGetir();
        islemNesnesi.IslemAdi = "Sayfa Görüntüleme " + islemNesnesi.URL;
        islemNesnesi.IslemIcerik = "";
        islemNesnesi.Tarih = DateTime.Now;

        islemNesnesi.KullaniciID = 0;

        try
        {
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

            if (currentInfo != null)
            {
                islemNesnesi.KullaniciID = currentInfo.KullaniciId;
            }
        }
        catch
        {

        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Loglar loglar = new Loglar(veritabaniIslemleri);

            loglar.Kullanici_id = islemNesnesi.KullaniciID;
            loglar.Url = islemNesnesi.URL;
            loglar.Tablo_adi = "";
            loglar.Islem_adi = islemNesnesi.IslemAdi;
            loglar.Islem_tipi = "S";
            loglar.Detay = islemNesnesi.IslemIcerik;
            loglar.Ip_adres = islemNesnesi.IpAdresi;
            loglar.Islem_tarihi = islemNesnesi.Tarih;

            veritabaniIslemleri.LogYasak = true;

            loglar.Ekle();
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    static public void HataKaydet(Exception exception, string hataEtiketi)
    {
        LogIslemleri logIslemleri = new LogIslemleri();
        logIslemleri.LogKaydet(exception.Message, exception.Message, exception.GetType().Name, hataEtiketi);
    }

    private void LogKaydet(string hataMetni, string baslik, string hataTip, string hataEtiketi)
    {
        HataNesnesi hataNesnesi = new HataNesnesi();

        hataNesnesi.HataBaslik = baslik;
        hataNesnesi.HataEtiketi = hataEtiketi;
        hataNesnesi.HataMetni = hataMetni;
        hataNesnesi.HataTipi = hataTip;
        hataNesnesi.IpAdresi = Utility.IpNoGetir();
        hataNesnesi.Tarih = DateTime.Now;
        hataNesnesi.URL = "";

        try
        {
            hataNesnesi.URL = HttpContext.Current.Request.Url.ToString();
        }
        catch
        {
        }

        hataNesnesi.KullaniciID = 0;

        try
        {
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

            hataNesnesi.KullaniciID = currentInfo.KullaniciId;
        }
        catch
        {
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Loglar loglar = new Loglar(veritabaniIslemleri);

            loglar.Kullanici_id = hataNesnesi.KullaniciID;
            loglar.Url = hataNesnesi.URL;
            loglar.Tablo_adi = hataNesnesi.HataEtiketi;
            loglar.Islem_adi = hataNesnesi.HataBaslik;
            loglar.Islem_tipi = "H";
            loglar.Detay = "Hata Tipi: " + hataNesnesi.HataTipi + Environment.NewLine + "Hata Metni: " + hataNesnesi.HataMetni;
            loglar.Ip_adres = hataNesnesi.IpAdresi;
            loglar.Islem_tarihi = hataNesnesi.Tarih;

            loglar.Ekle();
        }
        catch
        {
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    public static void OlayKaydet(string islemAdi, string islemEtiketi, string detay)
    {
        string url = "";

        try
        {
            url = HttpContext.Current.Request.Url.ToString();
        }
        catch
        {
        }

        int kullaniciId = 0;

        try
        {
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

            if (currentInfo != null)
            {
                kullaniciId = currentInfo.KullaniciId;
            }
        }
        catch
        {
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Loglar loglar = new Loglar(veritabaniIslemleri);

            loglar.Kullanici_id = kullaniciId;
            loglar.Url = url;
            loglar.Tablo_adi = islemEtiketi;
            loglar.Islem_adi = islemAdi;
            loglar.Islem_tipi = "S";
            loglar.Detay = detay;
            loglar.Ip_adres = Utility.IpNoGetir();
            loglar.Islem_tarihi = DateTime.Now;

            veritabaniIslemleri.LogYasak = true;

            loglar.Ekle();
        }
        catch
        {
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }
}

