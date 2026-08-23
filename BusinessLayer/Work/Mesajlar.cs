using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Mesajlar
{
    #region BAŞARI MESAJLARI

    public static string KullaniciBasariylaGuncellendi
    {
        get { return "Kullanıcı başarıyla güncellendi."; }
    }

    public static string KullaniciBasariylaEklendi
    {
        get { return "Kullanıcı başarıyla eklendi."; }
    }

    #endregion

    #region HATA MESAJLARI

    public static string KullaniciKoduVeyaMailHatali
    {
        get { return "Kullanıcı kodu veya mail hatalı!"; }
    }

    public static string KullaniciAdiVeyaSifreHatali
    {
        get { return "Kullanıcı adı veya şifre hatalı!"; }
    }

    public static string IslemGerceklestirilemedi
    {
        get { return "İşlem gerçekleştirilemedi."; }
    }

    public static string KayitZatenMevcut
    {
        get { return "Bu mail adresi veya telefon numarası daha önce kullanılmıştır."; }
    }

    #endregion

    #region DOĞRULAMA MESAJLARI

    public static string EmailVeKullaniciKoduDoldurunuz
    {
        get { return "Lütfen email ve kullanıcı kodu alanlarını doldurunuz!"; }
    }

    public static string RobotDogrulamasiYapiniz
    {
        get { return "Lütfen robot olmadığınızı doğrulayınız!"; }
    }

    public static string KullaniciAdiVeSifreGiriniz
    {
        get { return "Lütfen kullanıcı adı ve şifre giriniz!"; }
    }

    public static string TumAlanlariDoldurunuz
    {
        get { return "Lütfen tüm alanları eksiksiz doldurunuz, rol ve aktiflik durumu seçiniz!"; }
    }

    public static string GecerliProfilResmiSeciniz
    {
        get { return "Lütfen sadece .jpg, .jpeg veya .png uzantılı fotoğraflar seçiniz."; }
    }

    #endregion

    #region HATA BAŞLIKLARI

    public static string SifreSifirlamaHatasi
    {
        get { return "Şifre Sıfırlama Hatası: "; }
    }

    public static string GirisHatasi
    {
        get { return "Giriş Hatası: "; }
    }

    public static string ListelemeHatasi
    {
        get { return "Listeleme Hatası: "; }
    }

    public static string SilmeHatasi
    {
        get { return "Silme Hatası: "; }
    }

    public static string VeriYuklemeHatasi
    {
        get { return "Veriler yüklenirken hata oluştu: "; }
    }

    public static string GenelHata
    {
        get { return "Hata Oluştu: "; }
    }

    #endregion

    #region BİLGİ MESAJLARI

    public static string KullaniciAdiBaslik
    {
        get { return "Kullanıcı Adı: "; }
    }

    public static string GeciciSifreBaslik
    {
        get { return "Geçici Şifre: "; }
    }

    #endregion
}

