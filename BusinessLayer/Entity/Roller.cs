using System;
using System.Data;

public class Roller : OrtakAlanlar, IOrtakMetotlar
{
    public Roller(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    ~Roller()
    {
        SonucKayit = null;
        VeriTablosu = null;
    }

    #region SABİTLER

    public const string C_Tablo = "dbo.Roller";

    public const string C_Sp_Ekle = "dbo.SP_Roller_EKLE";
    public const string C_Sp_Guncelle = "dbo.SP_Roller_GUNCELLE";
    public const string C_Sp_Sil = "dbo.SP_Roller_SIL";
    public const string C_Sp_Doldur = "dbo.SP_Roller_DOLDUR";
    public const string C_Sp_TumunuGetir = "dbo.SP_Roller_TUMUNU_GETIR";
    public const string C_Sp_MaxIdGetir = "dbo.SP_Roller_MAX_ID_GETIR";

    public const string C_Sutun_rol_kodu = "rol_kodu";
    public const string C_Sutun_adi = "adi";
    public const string C_Sutun_aciklama = "aciklama";

    #endregion

    #region NESNELER

    private string rolKodu;
    public string RolKodu
    {
        get
        {
            return rolKodu;
        }
        set
        {
            rolKodu = value;
        }
    }

    private string adi;
    public string Adi
    {
        get
        {
            return adi;
        }
        set
        {
            adi = value;
        }
    }

    private string aciklama;
    public string Aciklama
    {
        get
        {
            return aciklama;
        }
        set
        {
            aciklama = value;
        }
    }

    #endregion

    #region METOTLAR

    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;

        VeritabaniIslem.ParametreEkle(C_Sutun_rol_kodu, RolKodu);
        VeritabaniIslem.ParametreEkle(C_Sutun_adi, Adi);
        VeritabaniIslem.ParametreEkle(C_Sutun_aciklama, Aciklama);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);

        return VeritabaniIslem.Calistir();
    }
    public bool Guncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Guncelle;

        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_adi, Adi);
        VeritabaniIslem.ParametreEkle(C_Sutun_aciklama, Aciklama);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
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

        DataRow veriSatiri = VeritabaniIslem.SatirGetir();

        if (veriSatiri == null)
        {
            return false;
        }

        RolKodu = veriSatiri[C_Sutun_rol_kodu] == DBNull.Value ? "" : veriSatiri[C_Sutun_rol_kodu].ToString();
        Adi = veriSatiri[C_Sutun_adi] == DBNull.Value ? "" : veriSatiri[C_Sutun_adi].ToString();
        Aciklama = veriSatiri[C_Sutun_aciklama] == DBNull.Value ? "" : veriSatiri[C_Sutun_aciklama].ToString();
        AktifMi = veriSatiri[C_Sutun_aktif_mi] != DBNull.Value && Convert.ToBoolean(veriSatiri[C_Sutun_aktif_mi]);

        return true;
    }

    public void TumunuGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_TumunuGetir;

        VeriTablosu = VeritabaniIslem.TabloGetir();
    }

    public int MaxIdGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_MaxIdGetir;

        object deger = VeritabaniIslem.DegerGetir();

        if (deger == null || deger == DBNull.Value)
        {
            return 0;
        }

        return Convert.ToInt32(deger);
    }

    #endregion
}


