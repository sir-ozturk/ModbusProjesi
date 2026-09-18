using System;
using System.Data;

public class RoleKartlari : OrtakAlanlar, IOrtakMetotlar
{
    public RoleKartlari(VeritabaniIslemleri veritabaniIslemleri)
    {
        VeritabaniIslem = veritabaniIslemleri;
    }

    #region SABİTLER

    public const string C_Tablo = "dbo.RoleKartlari";

    public const string C_Sp_Ekle = "dbo.SP_RoleKartlari_EKLE";
    public const string C_Sp_Guncelle = "dbo.SP_RoleKartlari_GUNCELLE";
    public const string C_Sp_Sil = "dbo.SP_RoleKartlari_SIL";
    public const string C_Sp_Doldur = "dbo.SP_RoleKartlari_DOLDUR";
    public const string C_Sp_TumunuGetir = "dbo.SP_RoleKartlari_TUMUNU_GETIR";

    public const string C_Sutun_role_adi = "role_adi";
    public const string C_Sutun_ethernet_kart_id = "ethernet_kart_id";

    #endregion

    #region NESNELER

    private string roleAdi;
    public string RoleAdi
    {
        get
        {
            return roleAdi;
        }
        set
        {
            roleAdi = value;
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
        if (!kontrol.RoleKontrol(this, DonanimKontrolleri.Islem.EKLE))
        {
            return false;
        }

        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_role_adi, RoleAdi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ethernet_kart_id, EthernetKartId);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);

        return VeritabaniIslem.Calistir();
    }

    public bool Guncelle()
    {
        DonanimKontrolleri kontrol = new DonanimKontrolleri(VeritabaniIslem);
        if (!kontrol.RoleKontrol(this, DonanimKontrolleri.Islem.GUNCELLE))
        {
            return false;
        }

        VeritabaniIslem.SpAdi = C_Sp_Guncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_role_adi, RoleAdi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ethernet_kart_id, EthernetKartId);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);

        return VeritabaniIslem.Calistir();
    }

    public bool Sil()
    {
        DonanimKontrolleri kontrol = new DonanimKontrolleri(VeritabaniIslem);
        if (!kontrol.RoleKontrol(this, DonanimKontrolleri.Islem.SIL))
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

        RoleAdi = SonucKayit[C_Sutun_role_adi].ToString();
        EthernetKartId = Convert.ToInt32(SonucKayit[C_Sutun_ethernet_kart_id]);
        AktifMi = Convert.ToBoolean(SonucKayit[C_Sutun_aktif_mi]);

        return true;
    }

    #endregion
}
