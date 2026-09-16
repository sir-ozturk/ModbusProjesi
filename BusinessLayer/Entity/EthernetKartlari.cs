using System;
using System.Data;

public class EthernetKartlari : OrtakAlanlar, IOrtakMetotlar
{
    public EthernetKartlari(VeritabaniIslemleri veritabaniIslemleri)
    {
        VeritabaniIslem = veritabaniIslemleri;
    }

    #region SABİTLER

    public const string C_Tablo = "dbo.EthernetKartlari";

    public const string C_Sp_Ekle = "dbo.SP_EthernetKartlari_EKLE";
    public const string C_Sp_Guncelle = "dbo.SP_EthernetKartlari_GUNCELLE";
    public const string C_Sp_Sil = "dbo.SP_EthernetKartlari_SIL";
    public const string C_Sp_Doldur = "dbo.SP_EthernetKartlari_DOLDUR";
    public const string C_Sp_TumunuGetir = "dbo.SP_EthernetKartlari_TUMUNU_GETIR";

    public const string C_Sutun_kart_adi = "kart_adi";
    public const string C_Sutun_model = "model";
    public const string C_Sutun_ip = "ip";
    public const string C_Sutun_http_port = "http_port";

    #endregion

    #region NESNELER

    private string kartAdi;
    public string KartAdi
    {
        get
        {
            return kartAdi;
        }
        set
        {
            kartAdi = value;
        }
    }

    private string model;
    public string Model
    {
        get
        {
            return model;
        }
        set
        {
            model = value;
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

    private int httpPort;
    public int HttpPort
    {
        get
        {
            return httpPort;
        }
        set
        {
            httpPort = value;
        }
    }

    #endregion

    #region METOTLAR

    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_kart_adi, KartAdi);
        VeritabaniIslem.ParametreEkle(C_Sutun_model, Model);
        VeritabaniIslem.ParametreEkle(C_Sutun_ip, Ip);
        VeritabaniIslem.ParametreEkle(C_Sutun_http_port, HttpPort);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);

        return VeritabaniIslem.Calistir();
    }

    public bool Guncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Guncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_kart_adi, KartAdi);
        VeritabaniIslem.ParametreEkle(C_Sutun_model, Model);
        VeritabaniIslem.ParametreEkle(C_Sutun_ip, Ip);
        VeritabaniIslem.ParametreEkle(C_Sutun_http_port, HttpPort);
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

    public void TumunuGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_TumunuGetir;
        VeriTablosu = VeritabaniIslem.TabloGetir();
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

        KartAdi = SonucKayit[C_Sutun_kart_adi].ToString();
        Model = SonucKayit[C_Sutun_model].ToString();
        Ip = SonucKayit[C_Sutun_ip].ToString();
        HttpPort = Convert.ToInt32(SonucKayit[C_Sutun_http_port]);
        AktifMi = Convert.ToBoolean(SonucKayit[C_Sutun_aktif_mi]);

        return true;
    }

    #endregion
}
