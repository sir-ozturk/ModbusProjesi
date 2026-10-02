using System;
using System.Data;

public class Parametreler : OrtakAlanlar, IOrtakMetotlar
{
    public Parametreler(VeritabaniIslemleri veritabaniIslemleri)
    {
        VeritabaniIslem = veritabaniIslemleri;
    }

#region SABİTLER
    public const string C_Tablo = "dbo.Parametreler";
    public const string C_Sp_Ekle = "dbo.SP_Parametreler_EKLE";
    public const string C_Sp_Guncelle = "dbo.SP_Parametreler_GUNCELLE";
    public const string C_Sp_Listele = "dbo.SP_Parametreler_LISTELE";
    public const string C_Sp_Doldur = "dbo.SP_Parametreler_DOLDUR";
    public const string C_Sp_Sil = "dbo.SP_Parametreler_SIL";
    public const string C_Sp_KullanimKayitlariGetir = "dbo.SP_Parametreler_KULLANIM_KAYITLARI_GETIR";
    public const string C_Sp_GrubaGoreGetir = "dbo.SP_Parametreler_GRUBA_GORE_GETIR";
    public const string C_Sutun_grup_id = "grup_id";
    public const string C_Sutun_kod = "kod";
    public const string C_Sutun_adi = "adi";
    public const string C_Sutun_aciklama = "aciklama";
    public const string C_Sutun_sira_no = "sira_no";
    public const string C_Sutun_aciklama_zorunlu_mu = "aciklama_zorunlu_mu";
#endregion
#region NESNELER
    private int grupId;
    public int GrupId
    {
        get
        {
            return grupId;
        }

        set
        {
            grupId = value;
        }
    }

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

    private bool aciklamaZorunluMu;
    public bool AciklamaZorunluMu
    {
        get
        {
            return aciklamaZorunluMu;
        }

        set
        {
            aciklamaZorunluMu = value;
        }
    }

#endregion
#region METOTLAR
    public bool Ekle()
    {
        ParametreKontrolleri kontrol = new ParametreKontrolleri(VeritabaniIslem);
        if (!kontrol.ParametreKontrol(this, ParametreKontrolleri.Islem.EKLE))
        {
            return false;
        }

        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_grup_id, GrupId);
        VeritabaniIslem.ParametreEkle(C_Sutun_kod, Kod);
        VeritabaniIslem.ParametreEkle(C_Sutun_adi, Adi);
        VeritabaniIslem.ParametreEkle(C_Sutun_aciklama, Aciklama);
        VeritabaniIslem.ParametreEkle(C_Sutun_sira_no, SiraNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_aciklama_zorunlu_mu, AciklamaZorunluMu);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);
        return VeritabaniIslem.Calistir();
    }

    public bool Guncelle()
    {
        ParametreKontrolleri kontrol = new ParametreKontrolleri(VeritabaniIslem);
        if (!kontrol.ParametreKontrol(this, ParametreKontrolleri.Islem.GUNCELLE))
        {
            return false;
        }

        VeritabaniIslem.SpAdi = C_Sp_Guncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_adi, Adi);
        VeritabaniIslem.ParametreEkle(C_Sutun_aciklama, Aciklama);
        VeritabaniIslem.ParametreEkle(C_Sutun_sira_no, SiraNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_aciklama_zorunlu_mu, AciklamaZorunluMu);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);
        return VeritabaniIslem.Calistir();
    }

    public bool Sil()
    {
        ParametreKontrolleri kontrol = new ParametreKontrolleri(VeritabaniIslem);
        if (!kontrol.SilmeKontrol(Id))
        {
            return false;
        }

        VeritabaniIslem.SpAdi = C_Sp_Sil;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        return VeritabaniIslem.Calistir();
    }

    public void TumunuGetir()
    {
        Listele();
    }

    public DataTable Listele(int? grupId = null, bool? aktifMi = null, string arama = null)
    {
        if (string.IsNullOrWhiteSpace(arama))
        {
            arama = null;
        }
        else
        {
            arama = arama.Trim();
        }

        if (arama != null && arama.Length > 150)
        {
            throw new ArgumentException("Arama metni 150 karakteri aşamaz.", "arama");
        }

        VeritabaniIslem.SpAdi = C_Sp_Listele;
        VeritabaniIslem.ParametreEkle(C_Sutun_grup_id, grupId);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, aktifMi);
        VeritabaniIslem.ParametreEkle("arama", arama);
        VeriTablosu = VeritabaniIslem.TabloGetir();
        return VeriTablosu;
    }

    public DataTable GrubaGoreGetir(string grupKodu)
    {
        if (string.IsNullOrWhiteSpace(grupKodu) || grupKodu.Trim().Length > 50)
        {
            throw new ArgumentException("Geçerli bir grup kodu gereklidir.", "grupKodu");
        }

        VeritabaniIslem.SpAdi = C_Sp_GrubaGoreGetir;
        VeritabaniIslem.ParametreEkle("grup_kodu", grupKodu.Trim().ToUpperInvariant());
        VeriTablosu = VeritabaniIslem.TabloGetir();
        return VeriTablosu;
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

        GrupId = Convert.ToInt32(SonucKayit[C_Sutun_grup_id]);
        Kod = SonucKayit[C_Sutun_kod].ToString();
        Adi = SonucKayit[C_Sutun_adi].ToString();
        Aciklama = SonucKayit[C_Sutun_aciklama].ToString();
        SiraNo = Convert.ToInt32(SonucKayit[C_Sutun_sira_no]);
        AciklamaZorunluMu = Convert.ToBoolean(SonucKayit[C_Sutun_aciklama_zorunlu_mu]);
        AktifMi = Convert.ToBoolean(SonucKayit[C_Sutun_aktif_mi]);
        return true;
    }
#endregion
}
