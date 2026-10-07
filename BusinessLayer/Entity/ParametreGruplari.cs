using System;
using System.Data;

public class ParametreGruplari : OrtakAlanlar
{
    public ParametreGruplari(VeritabaniIslemleri veritabaniIslemleri)
    {
        VeritabaniIslem = veritabaniIslemleri;
    }

    #region SABİTLER

    public const string C_Tablo = "dbo.ParametreGruplari";
    public const string C_Sp_Listele = "dbo.SP_ParametreGruplari_LISTELE";
    public const string C_Grup_MakineDurusNedeni = "MAKINE_DURUS_NEDENI";
    public const string C_Grup_MakineModeli = "MAKINE_MODELI";
    public const string C_Sutun_kod = "kod";
    public const string C_Sutun_adi = "adi";
    public const string C_Sutun_sira_no = "sira_no";

    #endregion

    #region NESNELER

    private string kod;
    public string Kod
    {
        get
        {
            return kod;
        }
        set
        {
            kod = value;
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

    private int siraNo;
    public int SiraNo
    {
        get
        {
            return siraNo;
        }
        set
        {
            siraNo = value;
        }
    }

    #endregion

    #region METOTLAR

    public void TumunuGetir()
    {
        Listele();
    }

    public DataTable Listele(bool? aktifMi = null)
    {
        VeritabaniIslem.SpAdi = C_Sp_Listele;
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, aktifMi);
        VeriTablosu = VeritabaniIslem.TabloGetir();
        return VeriTablosu;
    }

    #endregion
}
