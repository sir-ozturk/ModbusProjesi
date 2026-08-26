using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class RolYetkiler : OrtakAlanlar, IOrtakMetotlar
{
    public RolYetkiler(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    ~RolYetkiler()
    {
        SonucKayit = null;
        VeriTablosu = null;
    }

    #region SABİTLER

    public const string C_Tablo = "dbo.RolYetkiler";

    public const string C_Sp_Ekle = "dbo.SP_RolYetkiler_EKLE";
    public const string C_Sp_Guncelle = "dbo.SP_RolYetkiler_GUNCELLE";
    public const string C_Sp_Sil = "dbo.SP_RolYetkiler_SIL";
    public const string C_Sp_Doldur = "dbo.SP_RolYetkiler_DOLDUR";
    public const string C_Sp_TumunuGetir = "dbo.SP_RolYetkiler_TUMUNU_GETIR";
    public const string C_Sp_YetkiVarmi = "dbo.SP_RolYetkiler_YETKI_VAR_MI";
    public const string C_Sp_RoleGoreGetir = "dbo.SP_RolYetkiler_ROLE_GORE_GETIR";
    public const string C_Sp_RoleGoreSil = "dbo.SP_RolYetkiler_ROLE_GORE_SIL";

    public const string C_Sutun_rol_id = "rol_id";
    public const string C_Sutun_ekran = "ekran";
    public const string C_Sutun_goruntuleme = "goruntuleme";
    public const string C_Sutun_ekleme = "ekleme";
    public const string C_Sutun_guncelleme = "guncelleme";
    public const string C_Sutun_silme = "silme";
    public const string C_Sutun_yazdirma = "yazdirma";

    #endregion

    #region NESNELER

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

    private string ekran;
    public string Ekran
    {
        get
        {
            return ekran;
        }
        set
        {
            ekran = value;
        }
    }

    private bool goruntuleme;
    public bool Goruntuleme
    {
        get
        {
            return goruntuleme;
        }
        set
        {
            goruntuleme = value;
        }
    }

    private bool ekleme;
    public bool Ekleme
    {
        get
        {
            return ekleme;
        }
        set
        {
            ekleme = value;
        }
    }

    private bool guncelleme;
    public bool Guncelleme
    {
        get
        {
            return guncelleme;
        }
        set
        {
            guncelleme = value;
        }
    }

    private bool silme;
    public bool Silme
    {
        get
        {
            return silme;
        }
        set
        {
            silme = value;
        }
    }

    private bool yazdirma;
    public bool Yazdirma
    {
        get
        {
            return yazdirma;
        }
        set
        {
            yazdirma = value;
        }
    }

    #endregion

    #region METOTLAR

    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;

        VeritabaniIslem.ParametreEkle(C_Sutun_rol_id, RolId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekran, Ekran);
        VeritabaniIslem.ParametreEkle(C_Sutun_goruntuleme, Goruntuleme);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleme, Ekleme);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleme, Guncelleme);
        VeritabaniIslem.ParametreEkle(C_Sutun_silme, Silme);
        VeritabaniIslem.ParametreEkle(C_Sutun_yazdirma, Yazdirma);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);

        return VeritabaniIslem.Calistir();
    }

    public bool Guncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Guncelle;

        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_rol_id, RolId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekran, Ekran);
        VeritabaniIslem.ParametreEkle(C_Sutun_goruntuleme, Goruntuleme);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleme, Ekleme);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleme, Guncelleme);
        VeritabaniIslem.ParametreEkle(C_Sutun_silme, Silme);
        VeritabaniIslem.ParametreEkle(C_Sutun_yazdirma, Yazdirma);
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

        RolId = veriSatiri[C_Sutun_rol_id] == DBNull.Value ? 0 : Convert.ToInt32(veriSatiri[C_Sutun_rol_id]);
        Ekran = veriSatiri[C_Sutun_ekran] == DBNull.Value ? "" : veriSatiri[C_Sutun_ekran].ToString();
        Goruntuleme = veriSatiri[C_Sutun_goruntuleme] != DBNull.Value && Convert.ToBoolean(veriSatiri[C_Sutun_goruntuleme]);
        Ekleme = veriSatiri[C_Sutun_ekleme] != DBNull.Value && Convert.ToBoolean(veriSatiri[C_Sutun_ekleme]);
        Guncelleme = veriSatiri[C_Sutun_guncelleme] != DBNull.Value && Convert.ToBoolean(veriSatiri[C_Sutun_guncelleme]);
        Silme = veriSatiri[C_Sutun_silme] != DBNull.Value && Convert.ToBoolean(veriSatiri[C_Sutun_silme]);
        Yazdirma = veriSatiri[C_Sutun_yazdirma] != DBNull.Value && Convert.ToBoolean(veriSatiri[C_Sutun_yazdirma]);
        AktifMi = veriSatiri[C_Sutun_aktif_mi] != DBNull.Value && Convert.ToBoolean(veriSatiri[C_Sutun_aktif_mi]);

        return true;
    }

    public void TumunuGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_TumunuGetir;

        VeriTablosu = VeritabaniIslem.TabloGetir();
    }

    public bool YetkiVarmi(int rolId, string ekran, string islemTuru)
    {
        VeritabaniIslem.SpAdi = C_Sp_YetkiVarmi;

        VeritabaniIslem.ParametreEkle(C_Sutun_rol_id, rolId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekran, ekran);
        VeritabaniIslem.ParametreEkle("islem_turu", islemTuru);

        object sonuc = VeritabaniIslem.DegerGetir();

        if (sonuc == null || sonuc == DBNull.Value)
        {
            return false;
        }

        return Convert.ToBoolean(sonuc);
    }

    public void RoleGoreGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_RoleGoreGetir;
        VeritabaniIslem.ParametreEkle(C_Sutun_rol_id, RolId);

        VeriTablosu = VeritabaniIslem.TabloGetir();
    }

    public bool RoleGoreSil()
    {
        VeritabaniIslem.SpAdi = C_Sp_RoleGoreSil;
        VeritabaniIslem.ParametreEkle(C_Sutun_rol_id, RolId);

        return VeritabaniIslem.Calistir();
    }

    #endregion
}

