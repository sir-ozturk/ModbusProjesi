using System;
using System.Data;

public class Roller : OrtakAlanlar, IOrtakMetotlar
{
    public Roller(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    #region SABİTLER

    public const string C_Sp_TumunuGetir = "dbo.SP_Roller_TUMUNU_GETIR";
    public const string C_Sutun_rol_adi = "rol_adi";
    public const string C_Sutun_aciklama = "aciklama";

    #endregion

    #region NESNELER

    private string rolAdi;
    public string RolAdi
    {
        get
        {
            return rolAdi;
        }
        set
        {
            rolAdi = value;
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
        return true;
    }
    public bool Guncelle()
    {
        return true;
    }
    public bool Sil()
    {
        return true;
    }

    public bool Doldur()
    {
        return true;
    }

    public void TumunuGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_TumunuGetir;

        VeriTablosu = VeritabaniIslem.TabloGetir();
    }

    #endregion
}


