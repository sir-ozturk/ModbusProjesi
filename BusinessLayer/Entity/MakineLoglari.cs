using System;
using System.Data;

public class MakineLoglari : OrtakAlanlar
{
    public MakineLoglari(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    ~MakineLoglari()
    {
        SonucKayit = null;
        VeriTablosu = null;
    }

    #region SABİTLER

    public const string C_Tablo = "dbo.MakineLoglari";

    public const string C_Sp_Ekle = "dbo.SP_MakineLoglari_EKLE";
    public const string C_Sp_AcikKayitGetir = "dbo.SP_MakineLoglari_ACIK_KAYIT_GETIR";
    public const string C_Sp_Kapat = "dbo.SP_MakineLoglari_KAPAT";

    public const string C_Sutun_makine_id = "makine_id";
    public const string C_Sutun_islem_tipi = "islem_tipi";
    public const string C_Sutun_islem_nedeni = "islem_nedeni";
    public const string C_Sutun_islem_baslangic_tarih = "islem_baslangic_tarih";
    public const string C_Sutun_islem_bitis_tarih = "islem_bitis_tarih";
    public const string C_Sutun_devam_ediyor_mu = "devam_ediyor_mu";
    public const string C_Sutun_basarili_mi = "basarili_mi";
    public const string C_Sutun_hata_mesaji = "hata_mesaji";

    public const string C_IslemTipi_Durdur = "DURDUR";

    #endregion

    #region NESNELER

    private int makineId;
    public int MakineId
    {
        get
        {
            return makineId;
        }
        set
        {
            makineId = value;
        }
    }

    private string islemTipi;
    public string IslemTipi
    {
        get
        {
            return islemTipi;
        }
        set
        {
            islemTipi = value;
        }
    }

    private string islemNedeni;
    public string IslemNedeni
    {
        get
        {
            return islemNedeni;
        }
        set
        {
            islemNedeni = value;
        }
    }

    private DateTime islemBaslangicTarih;
    public DateTime IslemBaslangicTarih
    {
        get
        {
            return islemBaslangicTarih;
        }
        set
        {
            islemBaslangicTarih = value;
        }
    }

    private DateTime? islemBitisTarih;
    public DateTime? IslemBitisTarih
    {
        get
        {
            return islemBitisTarih;
        }
        set
        {
            islemBitisTarih = value;
        }
    }

    private bool devamEdiyorMu;
    public bool DevamEdiyorMu
    {
        get
        {
            return devamEdiyorMu;
        }
        set
        {
            devamEdiyorMu = value;
        }
    }

    private bool basariliMi;
    public bool BasariliMi
    {
        get
        {
            return basariliMi;
        }
        set
        {
            basariliMi = value;
        }
    }

    private string hataMesaji;
    public string HataMesaji
    {
        get
        {
            return hataMesaji;
        }
        set
        {
            hataMesaji = value;
        }
    }

    #endregion

    #region METOTLAR

    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;

        VeritabaniIslem.ParametreEkle(C_Sutun_makine_id, MakineId);
        VeritabaniIslem.ParametreEkle(C_Sutun_islem_tipi, IslemTipi);
        VeritabaniIslem.ParametreEkle(C_Sutun_islem_nedeni, IslemNedeni);
        VeritabaniIslem.ParametreEkle(C_Sutun_devam_ediyor_mu, DevamEdiyorMu);
        VeritabaniIslem.ParametreEkle(C_Sutun_basarili_mi, BasariliMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_hata_mesaji, HataMesaji);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);

        return VeritabaniIslem.Calistir();
    }

    public bool AcikKayitGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_AcikKayitGetir;
        VeritabaniIslem.ParametreEkle(C_Sutun_makine_id, MakineId);

        SonucKayit = VeritabaniIslem.SatirGetir();

        if (SonucKayit == null)
        {
            return false;
        }

        Id = Convert.ToInt32(SonucKayit[C_Sutun_id]);
        MakineId = Convert.ToInt32(SonucKayit[C_Sutun_makine_id]);
        IslemTipi = SonucKayit[C_Sutun_islem_tipi].ToString();
        IslemNedeni = SonucKayit[C_Sutun_islem_nedeni] == DBNull.Value ? "" : SonucKayit[C_Sutun_islem_nedeni].ToString();
        IslemBaslangicTarih = Convert.ToDateTime(SonucKayit[C_Sutun_islem_baslangic_tarih]);
        IslemBitisTarih = SonucKayit[C_Sutun_islem_bitis_tarih] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(SonucKayit[C_Sutun_islem_bitis_tarih]);
        DevamEdiyorMu = Convert.ToBoolean(SonucKayit[C_Sutun_devam_ediyor_mu]);
        BasariliMi = Convert.ToBoolean(SonucKayit[C_Sutun_basarili_mi]);
        HataMesaji = SonucKayit[C_Sutun_hata_mesaji] == DBNull.Value ? "" : SonucKayit[C_Sutun_hata_mesaji].ToString();
        AktifMi = Convert.ToBoolean(SonucKayit[C_Sutun_aktif_mi]);

        return true;
    }

    public bool Kapat()
    {
        VeritabaniIslem.SpAdi = C_Sp_Kapat;

        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId > 0 ? (object)GuncelleyenId : null);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, string.IsNullOrWhiteSpace(GuncelleyenIp) ? null : GuncelleyenIp);

        return VeritabaniIslem.Calistir();
    }

    #endregion
}
