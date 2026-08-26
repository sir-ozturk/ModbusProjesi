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

    public static string RolBasariylaGuncellendi
    {
        get { return "Rol başarıyla güncellendi."; }
    }

    public static string RolBasariylaEklendi
    {
        get { return "Rol başarıyla eklendi."; }
    }
    public static string SilmeBasarili
    {
        get { return "Silme işlemi başarılı."; }
    }

    public static string KayitBasarili
    {
        get { return "Kayıt işlemi başarılı."; }
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

    public static string KayitGetirmeHatasi
    {
        get { return "Kayıtlar getirilemedi."; }
    }

    public static string KayitBulunamadi
    {
        get { return "Kayıtlar bulunamadı."; }
    }

    public static string YetkinizYok
    {
        get { return "Yetkisiz işlem."; }
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

    public static string RolAlanlarBos
    {
        get { return "Lütfen rol adı ve aktiflik durumunu seçiniz."; }
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

    #region DOĞRULAMA MESAJLARI

    public static string BaslangicTarihiBitisTarihindenBuyukOlamaz
    {
        get { return "Başlangıç tarihi bitiş tarihinden büyük olamaz."; }
    }

    #endregion

    #region PROFİL DÜZENLEME MESAJLARI

    public static string ProfilAlanlariBos
    {
        get { return "Lütfen ad, soyad, telefon ve e-posta alanlarını eksiksiz doldurunuz."; }
    }

    public static string GecersizEPostaAdresi
    {
        get { return "Lütfen geçerli bir e-posta adresi giriniz."; }
    }

    public static string SifreAlanlariBos
    {
        get { return "Şifre değişikliği için mevcut şifreyi, yeni şifreyi ve tekrarını giriniz."; }
    }

    public static string YeniSifrelerEslesmiyor
    {
        get { return "Yeni şifre ile tekrarı aynı değil."; }
    }

    public static string MevcutSifreHatali
    {
        get { return "Mevcut şifreniz hatalı."; }
    }

    public static string YeniSifreMevcutSifreyleAyni
    {
        get { return "Yeni şifreniz mevcut şifrenizle aynı olamaz."; }
    }

    public static string ProfilBasariylaGuncellendi
    {
        get { return "Profil bilgileriniz başarıyla güncellendi."; }
    }

    #endregion

    #region MAKİNE MESAJLARI

    public static string MakineBasariylaEklendi
    {
        get { return "Makine başarıyla eklendi."; }
    }

    public static string MakineBasariylaGuncellendi
    {
        get { return "Makine başarıyla güncellendi."; }
    }

    public static string MakineBasariylaSilindi
    {
        get { return "Makine başarıyla silindi."; }
    }

    public static string MakineBilgileriAlinamadi
    {
        get { return "Makine bilgileri alınamadı."; }
    }

    public static string MakineEklemeHatasi
    {
        get { return "Makine ekleme işlemi başarısız."; }
    }

    public static string MakineGuncellemeHatasi
    {
        get { return "Makine güncelleme işlemi başarısız."; }
    }

    public static string MakineSilmeHatasi
    {
        get { return "Makine silme işlemi başarısız."; }
    }

    public static string MakineAlanlarBos
    {
        get { return "Lütfen makine bilgilerini eksiksiz doldurunuz."; }
    }

    public static string MakineKayitli
    {
        get { return "Aynı makine numarası, IP veya MFG bilgisine sahip bir makine zaten kayıtlı."; }
    }

    public static string GecersizIpAdresi
    {
        get { return "Lütfen geçerli bir IP adresi giriniz."; }
    }

    public static string MakineSayisalAlanHatasi
    {
        get { return "GG No, Makine No ve MFG alanlarına yalnızca rakam girilebilir."; }
    }

    #endregion
}

