using System;
using System.Data;

public class Makineler : OrtakAlanlar, IOrtakMetotlar
{
    public Makineler(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    ~Makineler()
    {
        SonucKayit = null;
        VeriTablosu = null;
    }

    #region SABİTLER

    public const string C_Tablo = "dbo.Makineler";

    public const string C_Sp_Ekle = "dbo.SP_Makineler_EKLE";
    public const string C_Sp_Guncelle = "dbo.SP_Makineler_GUNCELLE";
    public const string C_Sp_Sil = "dbo.SP_Makineler_SIL";
    public const string C_Sp_Doldur = "dbo.SP_Makineler_DOLDUR";
    public const string C_Sp_TumunuGetir = "dbo.SP_Makineler_TUMUNU_GETIR";
    public const string C_Sp_DashboardGetir = "dbo.SP_Makineler_DASHBOARD_GETIR";
    public const string C_Sp_SiralamayiGuncelle = "dbo.SP_Makineler_SIRALAMAYI_GUNCELLE";
    public const string C_Sp_KayitVarMi = "dbo.SP_Makineler_KAYIT_VAR_MI";

    public const string C_Sutun_model_ad = "model_ad";
    public const string C_Sutun_entegrasyon_kod = "entegrasyon_kod";
    public const string C_Sutun_gg_no = "gg_no";
    public const string C_Sutun_makine_no = "makine_no";
    public const string C_Sutun_makine_adi = "makine_adi";
    public const string C_Sutun_sira_no = "sira_no";
    public const string C_Sutun_band_no = "band_no";
    public const string C_Sutun_ip = "ip";
    public const string C_Sutun_mfg = "mfg";

    #endregion

    #region NESNELER

    private int? relayChannel;
    public int? RelayChannel
    {
        get
        {
            return relayChannel;
        }
        set
        {
            relayChannel = value;
        }
    }

    private string modelAd;
    public string ModelAd
    {
        get 
        { 
            return modelAd; 
        }
        set 
        { 
            modelAd = value; 
        }
    }

    private string entegrasyonKod;
    public string EntegrasyonKod
    {
        get
        {
            return entegrasyonKod;
        }
        set
        {
            entegrasyonKod = value;
        }
    }

    private string ggNo;
    public string GgNo
    {
        get
        {
            return ggNo;
        }
        set
        {
            ggNo = value;
        }
    }

    private string makineNo;
    public string MakineNo
    {
        get
        {
            return makineNo;
        }
        set
        {
            makineNo = value;
        }
    }

    private string makineAdi;
    public string MakineAdi
    {
        get
        {
            return makineAdi;
        }
        set
        {
            makineAdi = value;
        }
    }

    private string bandNo;
    public string BandNo
    {
        get 
        { 
            return bandNo; 
        }
        set 
        { 
            bandNo = value; 
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

    private string ip;
    public string Ip
    {
        get
        {
            return ip;
        }
        set
        {
            ip = value;
        }
    }

    private string mfg;
    public string Mfg
    {
        get
        {
            return mfg;
        }
        set
        {
            mfg = value;
        }
    }

    #endregion

    #region METOTLAR

    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;

        VeritabaniIslem.ParametreEkle(C_Sutun_model_ad, ModelAd);
        VeritabaniIslem.ParametreEkle(C_Sutun_entegrasyon_kod, EntegrasyonKod);
        VeritabaniIslem.ParametreEkle(C_Sutun_gg_no, GgNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_makine_no, MakineNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_makine_adi, MakineAdi);
        VeritabaniIslem.ParametreEkle(C_Sutun_band_no, BandNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_ip, Ip);
        VeritabaniIslem.ParametreEkle(C_Sutun_mfg, Mfg);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);

        return VeritabaniIslem.Calistir();
    }

    public bool Guncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Guncelle;

        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_model_ad, ModelAd);
        VeritabaniIslem.ParametreEkle(C_Sutun_entegrasyon_kod, EntegrasyonKod);
        VeritabaniIslem.ParametreEkle(C_Sutun_gg_no, GgNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_makine_no, MakineNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_makine_adi, MakineAdi);
        VeritabaniIslem.ParametreEkle(C_Sutun_band_no, BandNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_ip, Ip);
        VeritabaniIslem.ParametreEkle(C_Sutun_mfg, Mfg);
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

        SonucKayit = VeritabaniIslem.SatirGetir();

        if (SonucKayit == null)
        {
            return false;
        }

        ModelAd = SonucKayit[C_Sutun_model_ad].ToString();
        EntegrasyonKod = SonucKayit[C_Sutun_entegrasyon_kod].ToString();
        GgNo = SonucKayit[C_Sutun_gg_no].ToString();
        MakineNo = SonucKayit[C_Sutun_makine_no].ToString();
        RelayChannel = SonucKayit["relay_channel"] == DBNull.Value ? (int?)null : Convert.ToInt32(SonucKayit["relay_channel"]);
        MakineAdi = SonucKayit[C_Sutun_makine_adi].ToString();
        SiraNo = Convert.ToInt32(SonucKayit[C_Sutun_sira_no]);
        BandNo = SonucKayit[C_Sutun_band_no].ToString();
        Ip = SonucKayit[C_Sutun_ip].ToString();
        Mfg = SonucKayit[C_Sutun_mfg].ToString();
        AktifMi = Convert.ToBoolean(SonucKayit[C_Sutun_aktif_mi]);

        return true;
    }

    public void TumunuGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_TumunuGetir;

        VeriTablosu = VeritabaniIslem.TabloGetir();
    }

    public void DashboardGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_DashboardGetir;

        VeriTablosu = VeritabaniIslem.TabloGetir();
    }

    public bool SiralamayiGuncelle(string siralamaXml)
    {
        VeritabaniIslem.SpAdi = C_Sp_SiralamayiGuncelle;

        VeritabaniIslem.ParametreEkle("siralama_xml", siralamaXml);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);

        return VeritabaniIslem.Calistir();
    }

    public bool KayitVarMi()
    {
        VeritabaniIslem.SpAdi = C_Sp_KayitVarMi;

        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_makine_no, MakineNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_ip, Ip);
        VeritabaniIslem.ParametreEkle(C_Sutun_mfg, Mfg);

        object sonuc = VeritabaniIslem.DegerGetir();

        return Convert.ToInt32(sonuc) > 0;
    }

    #endregion
}
