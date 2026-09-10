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

    #region ROL MESAJLARI

    public static string RolSilinemedi
    {
        get { return "Rol silinemedi. Rolü kullanan kullanıcılar bulunuyor olabilir."; }
    }

    public static string RolYetkileriSilinemedi
    {
        get { return "Role ait yetki kayıtları silinemedi."; }
    }

    public static string RolYetkisiKaydedilemedi
    {
        get { return "Rol yetkisi kaydedilemedi."; }
    }

    public static string RolYetkisiGuncellenemedi
    {
        get { return "Rol yetkisi güncellenemedi."; }
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

    public static string MakineSiralamasiGuncellendi
    {
        get { return "Makine sıralaması başarıyla güncellendi."; }
    }

    public static string MakineSiralamasiGuncellenemedi
    {
        get { return "Makine sıralaması güncellenemedi."; }
    }

    public static string MakineBasariylaDurduruldu
    {
        get { return "Makine başarıyla durduruldu."; }
    }

    public static string MakineDurdurulamadi
    {
        get { return "Makine durdurulamadı."; }
    }

    public static string MakineZatenDuruyor
    {
        get { return "Makine zaten duruyor."; }
    }

    public static string DurusNedeniSeciniz
    {
        get { return "Lütfen bir duruş nedeni seçiniz veya özel neden giriniz."; }
    }

    public static string MakineSimulasyondaCalistirildi
    {
        get { return "Makine simülasyonda yeniden çalıştırıldı."; }
    }

    public static string MakineCalistirilamadi
    {
        get { return "Makine çalıştırma kaydı kapatılamadı."; }
    }

    public static string MakineZatenCalisiyor
    {
        get { return "Makine zaten çalışıyor."; }
    }

    public static string MakineRoleAtamasiYok
    {
        get { return "Bu makine 1 numaralı röleye atanmamış veya aktif değil."; }
    }

    public static string RoleCihazAyariGecersiz
    {
        get { return "Makine kontrol cihazının adres ayarı geçersiz."; }
    }

    public static string RoleCihazinaUlasilamadi
    {
        get { return "Makine kontrol cihazına ulaşılamadı. Komut sonucu doğrulanamadı; röle durumunu kontrol ediniz."; }
    }

    public static string RoleCihazZamanAsimi
    {
        get { return "Makine kontrol cihazı 4 saniye içinde yanıt vermedi. Komut uygulanmış olabilir; röle durumunu kontrol ediniz."; }
    }

    public static string RoleCihazYanitiBasarisiz
    {
        get { return "Makine kontrol cihazından başarılı yanıt alınamadı. Röle durumunu kontrol ediniz."; }
    }

    public static string RoleKomutuKaydedilemedi
    {
        get { return "Röle komutu cihaz tarafından kabul edildi ancak veritabanı kaydı tamamlanamadı. Ekrandaki durum güncel olmayabilir; röle durumunu kontrol ediniz."; }
    }

    public static string RoleKomutuIsleniyor
    {
        get { return "Röle için başka bir işlem devam ediyor. Tamamlandıktan sonra tekrar deneyiniz."; }
    }

    public static string RoleDurdurmaKomutuGonderildi
    {
        get { return "1 numaralı röleye durdurma komutu gönderildi."; }
    }

    public static string RoleBaslatmaKomutuGonderildi
    {
        get { return "1 numaralı röleye başlatma komutu gönderildi."; }
    }

    #endregion
}

