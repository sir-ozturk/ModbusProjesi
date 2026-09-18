using System;
using System.Data;

public class MakineRoleBaglantilari : OrtakAlanlar, IOrtakMetotlar
{
    public MakineRoleBaglantilari(VeritabaniIslemleri veritabaniIslemleri)
    {
        VeritabaniIslem = veritabaniIslemleri;
    }

    #region SABİTLER

    public const string C_Tablo = "dbo.MakineRoleBaglantilari";

    public const string C_Sp_Ekle = "dbo.SP_MakineRoleBaglantilari_EKLE";
    public const string C_Sp_Guncelle = "dbo.SP_MakineRoleBaglantilari_GUNCELLE";
    public const string C_Sp_Sil = "dbo.SP_MakineRoleBaglantilari_SIL";
    public const string C_Sp_Doldur = "dbo.SP_MakineRoleBaglantilari_DOLDUR";
    public const string C_Sp_TumunuGetir = "dbo.SP_MakineRoleBaglantilari_TUMUNU_GETIR";
    public const string C_Sp_KomutGetir = "dbo.SP_MakineRoleBaglantilari_KOMUT_GETIR";

    public const string C_Sutun_role_kart_id = "role_kart_id";
    public const string C_Sutun_kanal_no = "kanal_no";
    public const string C_Sutun_makine_id = "makine_id";
    public const string C_Sutun_ip = "ip";
    public const string C_Sutun_http_port = "http_port";
    public const string C_Sutun_ethernet_kart_id = "ethernet_kart_id";

    #endregion

    #region NESNELER

    private int roleKartId;
    public int RoleKartId
    {
        get
        {
            return roleKartId;
        }
        set
        {
            roleKartId = value;
        }
    }

    private int kanalNo;
    public int KanalNo
    {
        get
        {
            return kanalNo;
        }
        set
        {
            kanalNo = value;
        }
    }

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

    private int ethernetKartId;
    public int EthernetKartId
    {
        get
        {
            return ethernetKartId;
        }
        set
        {
            ethernetKartId = value;
        }
    }

    #endregion

    #region METOTLAR

    public bool Ekle()
    {
        DonanimKontrolleri kontrol = new DonanimKontrolleri(VeritabaniIslem);
        if (!kontrol.BaglantiKontrol(this, DonanimKontrolleri.Islem.EKLE))
        {
            return false;
        }

        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_role_kart_id, RoleKartId);
        VeritabaniIslem.ParametreEkle(C_Sutun_kanal_no, KanalNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_makine_id, MakineId);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);

        return VeritabaniIslem.Calistir();
    }

    public bool Guncelle()
    {
        DonanimKontrolleri kontrol = new DonanimKontrolleri(VeritabaniIslem);
        if (!kontrol.BaglantiKontrol(this, DonanimKontrolleri.Islem.GUNCELLE))
        {
            return false;
        }

        VeritabaniIslem.SpAdi = C_Sp_Guncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_role_kart_id, RoleKartId);
        VeritabaniIslem.ParametreEkle(C_Sutun_kanal_no, KanalNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_makine_id, MakineId);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);

        return VeritabaniIslem.Calistir();
    }

    public bool Sil()
    {
        DonanimKontrolleri kontrol = new DonanimKontrolleri(VeritabaniIslem);
        if (!kontrol.BaglantiKontrol(this, DonanimKontrolleri.Islem.SIL))
        {
            return false;
        }

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

        RoleKartId = Convert.ToInt32(SonucKayit[C_Sutun_role_kart_id]);
        KanalNo = Convert.ToInt32(SonucKayit[C_Sutun_kanal_no]);
        MakineId = Convert.ToInt32(SonucKayit[C_Sutun_makine_id]);
        AktifMi = Convert.ToBoolean(SonucKayit[C_Sutun_aktif_mi]);

        return true;
    }

    // Transaction içindeki SQL kilitleri komut tamamlanana kadar korunur.
    public bool KomutBaglantisiniGetir()
    {
        DonanimKontrolleri kontrol = new DonanimKontrolleri(VeritabaniIslem);
        if (!kontrol.KomutAyarKilidiAl())
        {
            throw new DonanimIslemHatasi(VeritabaniIslem.SonHataMesaji);
        }

        VeritabaniIslem.SpAdi = C_Sp_KomutGetir;
        VeritabaniIslem.ParametreEkle(C_Sutun_makine_id, MakineId);

        SonucKayit = VeritabaniIslem.SatirGetir();

        if (SonucKayit == null)
        {
            return false;
        }

        Id = Convert.ToInt32(SonucKayit[C_Sutun_id]);
        RoleKartId = Convert.ToInt32(SonucKayit[C_Sutun_role_kart_id]);
        KanalNo = Convert.ToInt32(SonucKayit[C_Sutun_kanal_no]);
        Ip = SonucKayit[C_Sutun_ip].ToString();
        HttpPort = Convert.ToInt32(SonucKayit[C_Sutun_http_port]);
        EthernetKartId = Convert.ToInt32(SonucKayit[C_Sutun_ethernet_kart_id]);
        if (!kontrol.CihazKilidiAl(EthernetKartId))
        {
            throw new DonanimIslemHatasi(VeritabaniIslem.SonHataMesaji);
        }

        AktifMi = true;

        return true;
    }

    #endregion
}
