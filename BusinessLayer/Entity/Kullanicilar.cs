using System;
using System.Data;


public class Kullanicilar : OrtakAlanlar, IOrtakMetotlar
{
    public Kullanicilar(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    #region SABİTLER

    public const string C_Tablo = "dbo.Kullanicilar";

    public const string C_Sp_Ekle = "dbo.SP_Kullanicilar_EKLE";
    public const string C_Sp_Sil = "dbo.SP_Kullanicilar_SIL";
    public const string C_Sp_Guncelle = "dbo.SP_Kullanicilar_GUNCELLE";
    public const string C_Sp_Doldur = "dbo.SP_Kullanicilar_DOLDUR";
    public const string C_Sp_TumunuGetir = "dbo.SP_Kullanicilar_TUMUNU_GETIR";
    public const string C_Sp_Giris = "dbo.SP_Kullanicilar_GIRIS";
    public const string C_Sp_SifreKontrol = "dbo.SP_Kullanicilar_SIFRE_KONTROL";
    public const string C_Sp_SifreGuncelle = "dbo.SP_Kullanicilar_SIFRE_GUNCELLE";
    public const string C_Sp_KayitVarMi = "dbo.SP_Kullanicilar_KAYIT_VAR_MI";
    public const string C_Sp_KullaniciAdiGuncelle = "dbo.SP_Kullanicilar_KULLANICI_ADI_GUNCELLE";
    public const string C_Sp_MaxIdGetir = "dbo.SP_Kullanicilar_MAX_ID_GETIR";


    public const string C_Sutun_kullanici_adi = "kullanici_adi";
    public const string C_Sutun_sifre = "sifre";
    public const string C_Sutun_ad = "ad";
    public const string C_Sutun_soyad = "soyad";
    public const string C_Sutun_telefon = "telefon";
    public const string C_Sutun_mail = "mail";
    public const string C_Sutun_rol_id = "rol_id";
    public const string C_Sutun_profil_resim = "profil_resim";

    #endregion

    #region NESNELER

    private string kullaniciAdi;
    public string KullaniciAdi
    {
        get
        {
            return kullaniciAdi;
        }
        set
        {
            kullaniciAdi = value;
        }
    }

    private string sifre;
    public string Sifre
    {
        get
        {
            return sifre;
        }
        set
        {
            sifre = value;
        }
    }

    private string ad;
    public string Ad
    {
        get
        {
            return ad;
        }
        set
        {
            ad = value;
        }
    }

    private string soyad;
    public string Soyad
    {
        get
        {
            return soyad;
        }
        set
        {
            soyad = value;
        }
    }

    private string telefon;

    public string Telefon
    {
        get
        {
            return telefon;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                telefon = value;
            }
            else
            {
                telefon = value.Replace("-", "").Replace(" ", "");
            }
        }
    }

    private string mail;
    public string Mail
    {
        get
        {
            return mail;
        }
        set
        {
            mail = value;
        }
    }

    private int rolId;
    public int RolId
    {
        get
        {
            return rolId;
        }
        set
        {
            rolId = value;
        }
    }

    private string profilResim;
    public string ProfilResim
    {
        get
        {
            return profilResim;
        }
        set
        {
            profilResim = value;
        }
    }

    #endregion

    #region METOTLAR

    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;

        VeritabaniIslem.ParametreEkle(C_Sutun_kullanici_adi, KullaniciAdi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ad, Ad);
        VeritabaniIslem.ParametreEkle(C_Sutun_soyad, Soyad);
        VeritabaniIslem.ParametreEkle(C_Sutun_sifre, Sifre);
        VeritabaniIslem.ParametreEkle(C_Sutun_telefon, Telefon);
        VeritabaniIslem.ParametreEkle(C_Sutun_mail, Mail);
        VeritabaniIslem.ParametreEkle(C_Sutun_rol_id, RolId);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_profil_resim, ProfilResim);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);

        return VeritabaniIslem.Calistir();
    }

    public bool Guncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Guncelle;

        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_kullanici_adi, KullaniciAdi);

        if (string.IsNullOrEmpty(Sifre))
        {
            VeritabaniIslem.ParametreEkle(C_Sutun_sifre, null);
        }
        else
        {
            VeritabaniIslem.ParametreEkle(C_Sutun_sifre, Sifre);
        }

        VeritabaniIslem.ParametreEkle(C_Sutun_ad, Ad);
        VeritabaniIslem.ParametreEkle(C_Sutun_soyad, Soyad);
        VeritabaniIslem.ParametreEkle(C_Sutun_telefon, Telefon);
        VeritabaniIslem.ParametreEkle(C_Sutun_mail, Mail);
        VeritabaniIslem.ParametreEkle(C_Sutun_rol_id, RolId);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);

        if (string.IsNullOrEmpty(ProfilResim))
        {
            VeritabaniIslem.ParametreEkle(C_Sutun_profil_resim, null);
        }
        else
        {
            VeritabaniIslem.ParametreEkle(C_Sutun_profil_resim, ProfilResim);
        }

        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);

        return VeritabaniIslem.Calistir();
    }

    public bool Sil()
    {
        VeritabaniIslem.SpAdi = C_Sp_Sil;

        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);

        return VeritabaniIslem.Calistir();
    }

    public bool Doldur()
    {
        VeritabaniIslem.SpAdi = C_Sp_Doldur;

        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);

        SonucKayit = VeritabaniIslem.SatirGetir();

        if (SonucKayit == null)
        {
            return false;
        }

        Id = Convert.ToInt32(SonucKayit[C_Sutun_id]);
        KullaniciAdi = SonucKayit[C_Sutun_kullanici_adi].ToString();
        Sifre = SonucKayit[C_Sutun_sifre].ToString();
        Ad = SonucKayit[C_Sutun_ad].ToString();
        Soyad = SonucKayit[C_Sutun_soyad].ToString();
        Telefon = SonucKayit[C_Sutun_telefon].ToString();
        Mail = SonucKayit[C_Sutun_mail].ToString();
        RolId = Convert.ToInt32(SonucKayit[C_Sutun_rol_id]);
        AktifMi = Convert.ToBoolean(SonucKayit[C_Sutun_aktif_mi]);
        ProfilResim = SonucKayit[C_Sutun_profil_resim].ToString();

        return true;
    }

    public void TumunuGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_TumunuGetir;

        VeriTablosu = VeritabaniIslem.TabloGetir();
    }

    public bool Giris()
    {
        VeritabaniIslem.SpAdi = C_Sp_Giris;

        VeritabaniIslem.ParametreEkle(C_Sutun_kullanici_adi, KullaniciAdi);
        VeritabaniIslem.ParametreEkle(C_Sutun_sifre, Sifre);

        SonucKayit = VeritabaniIslem.SatirGetir();

        if (SonucKayit == null)
        {
            return false;
        }

        Id = Convert.ToInt32(SonucKayit[C_Sutun_id]);
        Ad = SonucKayit[C_Sutun_ad].ToString();
        Soyad = SonucKayit[C_Sutun_soyad].ToString();
        RolId = Convert.ToInt32(SonucKayit[C_Sutun_rol_id]);
        AktifMi = Convert.ToBoolean(SonucKayit[C_Sutun_aktif_mi]);
        ProfilResim = SonucKayit[C_Sutun_profil_resim].ToString();

        return true;
    }

    public bool SifreKontrol()
    {
        VeritabaniIslem.SpAdi = C_Sp_SifreKontrol;

        VeritabaniIslem.ParametreEkle(C_Sutun_kullanici_adi, KullaniciAdi);
        VeritabaniIslem.ParametreEkle(C_Sutun_mail, Mail);

        SonucKayit = VeritabaniIslem.SatirGetir();

        if (SonucKayit == null)
        {
            return false;
        }

        Ad = SonucKayit[C_Sutun_ad].ToString();
        Id = Convert.ToInt32(SonucKayit[C_Sutun_id]);

        return true;
    }

    public bool SifreGuncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_SifreGuncelle;

        VeritabaniIslem.ParametreEkle(C_Sutun_kullanici_adi, KullaniciAdi);
        VeritabaniIslem.ParametreEkle(C_Sutun_sifre, Sifre);
        VeritabaniIslem.ParametreEkle(C_Sutun_mail, Mail);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);

        return VeritabaniIslem.Calistir();
    }

    public bool KayitVarMi()
    {
        VeritabaniIslem.SpAdi = C_Sp_KayitVarMi;

        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_telefon, Telefon);
        VeritabaniIslem.ParametreEkle(C_Sutun_mail, Mail);

        object sonuc = VeritabaniIslem.DegerGetir();

        int kayitSayisi = Convert.ToInt32(sonuc);

        return kayitSayisi > 0;
    }

    public bool KullaniciAdiGuncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_KullaniciAdiGuncelle;

        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_kullanici_adi, KullaniciAdi);

        return VeritabaniIslem.Calistir();
    }

    public int MaxIdGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_MaxIdGetir;

        object sonuc = VeritabaniIslem.DegerGetir();

        return Convert.ToInt32(sonuc);
    }

    #endregion

}

