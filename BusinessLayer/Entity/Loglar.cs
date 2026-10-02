using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

public class Loglar : OrtakAlanlar
{
    public ArrayList eskiDetay;
    private ArrayList yeniDetay;
    public Loglar(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
        eskiDetay = new ArrayList();
        yeniDetay = new ArrayList();
    }

#region SABİTLER
    public const string C_Tablo = "dbo.Loglar";
    public const string C_Sp_Ekle = "dbo.SP_Loglar_EKLE";
    public const string C_Sp_Doldur = "dbo.SP_Loglar_DOLDUR";
    public const string C_Sp_TumunuGetir = "dbo.SP_Loglar_TUMUNU_GETIR";
    public const string C_Sp_IslemAdlar = "dbo.SP_Loglar_ISLEM_ADLAR";
    public const string C_Sp_TabloAdlar = "dbo.SP_Loglar_TABLO_ADLAR";
    public const string C_Sp_FiltreliGetir = "dbo.SP_Loglar_FILTRELI_GETIR";
    public const string C_Sutun_kullanici_id = "kullanici_id";
    public const string C_Parameter_kullanici_adi = "kullanici_adi";
    public const string C_Sutun_url = "url";
    public const string C_Sutun_tablo_adi = "tablo_adi";
    public const string C_Sutun_islem_adi = "islem_adi";
    public const string C_Sutun_islem_tipi = "islem_tipi";
    public const string C_Sutun_detay = "detay";
    public const string C_Sutun_ip_adres = "ip_adres";
    public const string C_Sutun_islem_tarihi = "islem_tarihi";
    public const string C_Parameter_baslangic_tarih = "baslangic_tarih";
    public const string C_Parameter_bitis_tarih = "bitis_tarih";
#endregion
#region NESNELER
    private int kullanici_id;
    public int Kullanici_id
    {
        get
        {
            return kullanici_id;
        }

        set
        {
            kullanici_id = value;
        }
    }

    private string url;
    public string Url
    {
        get
        {
            return url;
        }

        set
        {
            url = value;
        }
    }

    private string ip_adres;
    public string Ip_adres
    {
        get
        {
            return ip_adres;
        }

        set
        {
            ip_adres = value;
        }
    }

    private string tablo_adi;
    public string Tablo_adi
    {
        get
        {
            return tablo_adi;
        }

        set
        {
            tablo_adi = value;
        }
    }

    private string islem_adi;
    public string Islem_adi
    {
        get
        {
            return islem_adi;
        }

        set
        {
            islem_adi = value;
        }
    }

    private string islem_tipi;
    public string Islem_tipi
    {
        get
        {
            return islem_tipi;
        }

        set
        {
            islem_tipi = value;
        }
    }

    private string detay;
    public string Detay
    {
        get
        {
            return detay;
        }

        set
        {
            detay = value;
        }
    }

    private DateTime islem_tarihi;
    public DateTime Islem_tarihi
    {
        get
        {
            return islem_tarihi;
        }

        set
        {
            islem_tarihi = value;
        }
    }

#endregion
#region METOTLAR
    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_kullanici_id, Kullanici_id);
        VeritabaniIslem.ParametreEkle(C_Sutun_url, Url);
        VeritabaniIslem.ParametreEkle(C_Sutun_tablo_adi, Tablo_adi);
        VeritabaniIslem.ParametreEkle(C_Sutun_islem_adi, Islem_adi);
        VeritabaniIslem.ParametreEkle(C_Sutun_islem_tipi, Islem_tipi);
        if (Detay == null)
        {
            Detay = DetayGetir();
        }

        VeritabaniIslem.ParametreEkle(C_Sutun_detay, Detay);
        VeritabaniIslem.ParametreEkle(C_Sutun_ip_adres, Ip_adres);
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

        if (veriSatiri[C_Sutun_kullanici_id] == DBNull.Value)
        {
            Kullanici_id = 0;
        }
        else
        {
            Kullanici_id = Convert.ToInt32(veriSatiri[C_Sutun_kullanici_id]);
        }

        if (veriSatiri[C_Sutun_url] == DBNull.Value)
        {
            Url = "";
        }
        else
        {
            Url = veriSatiri[C_Sutun_url].ToString();
        }

        if (veriSatiri[C_Sutun_tablo_adi] == DBNull.Value)
        {
            Tablo_adi = "";
        }
        else
        {
            Tablo_adi = veriSatiri[C_Sutun_tablo_adi].ToString();
        }

        if (veriSatiri[C_Sutun_islem_adi] == DBNull.Value)
        {
            Islem_adi = "";
        }
        else
        {
            Islem_adi = veriSatiri[C_Sutun_islem_adi].ToString();
        }

        if (veriSatiri[C_Sutun_islem_tipi] == DBNull.Value)
        {
            Islem_tipi = "";
        }
        else
        {
            Islem_tipi = veriSatiri[C_Sutun_islem_tipi].ToString();
        }

        if (veriSatiri[C_Sutun_detay] == DBNull.Value)
        {
            Detay = "";
        }
        else
        {
            Detay = veriSatiri[C_Sutun_detay].ToString();
        }

        if (veriSatiri[C_Sutun_ip_adres] == DBNull.Value)
        {
            Ip_adres = "";
        }
        else
        {
            Ip_adres = veriSatiri[C_Sutun_ip_adres].ToString();
        }

        if (veriSatiri[C_Sutun_islem_tarihi] == DBNull.Value)
        {
            Islem_tarihi = DateTime.MinValue;
        }
        else
        {
            Islem_tarihi = Convert.ToDateTime(veriSatiri[C_Sutun_islem_tarihi]);
        }

        return true;
    }

    public void TumunuGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_TumunuGetir;
        VeriTablosu = VeritabaniIslem.TabloGetir();
    }

    public void IslemAdlarGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_IslemAdlar;
        VeriTablosu = VeritabaniIslem.TabloGetir();
    }

    public void TabloAdlarGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_TabloAdlar;
        VeriTablosu = VeritabaniIslem.TabloGetir();
    }

    public void YeniDetayEkle(string baslik, object icerik)
    {
        eskiDetay.Add(baslik.Replace("@", ""));
        yeniDetay.Add(icerik.ToString());
    }

    public string DetayGetir()
    {
        detay = "<NewDataSet><YeniKayitlar>";
        for (int i = 0; i < yeniDetay.Count; i++)
        {
            try
            {
                detay += "<" + eskiDetay[i].ToString() + ">" + System.Security.SecurityElement.Escape(yeniDetay[i].ToString()) + "</" + eskiDetay[i].ToString() + ">";
            }
            catch
            {
            }
        }

        detay += "</YeniKayitlar></NewDataSet>";
        return detay;
    }

    public void FiltreliGetir(string kullaniciAdi, string tabloAdi, string islemAdi, string islemTipi, DateTime? baslangicTarih, DateTime? bitisTarih)
    {
        VeritabaniIslem.SpAdi = C_Sp_FiltreliGetir;
        if (string.IsNullOrEmpty(kullaniciAdi))
        {
            VeritabaniIslem.ParametreEkle(C_Parameter_kullanici_adi, null);
        }
        else
        {
            VeritabaniIslem.ParametreEkle(C_Parameter_kullanici_adi, kullaniciAdi);
        }

        if (string.IsNullOrEmpty(tabloAdi))
        {
            VeritabaniIslem.ParametreEkle(C_Sutun_tablo_adi, null);
        }
        else
        {
            VeritabaniIslem.ParametreEkle(C_Sutun_tablo_adi, tabloAdi);
        }

        if (string.IsNullOrEmpty(islemAdi))
        {
            VeritabaniIslem.ParametreEkle(C_Sutun_islem_adi, null);
        }
        else
        {
            VeritabaniIslem.ParametreEkle(C_Sutun_islem_adi, islemAdi);
        }

        if (string.IsNullOrEmpty(islemTipi))
        {
            VeritabaniIslem.ParametreEkle(C_Sutun_islem_tipi, null);
        }
        else
        {
            VeritabaniIslem.ParametreEkle(C_Sutun_islem_tipi, islemTipi);
        }

        if (baslangicTarih.HasValue)
        {
            VeritabaniIslem.ParametreEkle(C_Parameter_baslangic_tarih, (object)baslangicTarih.Value);
        }
        else
        {
            VeritabaniIslem.ParametreEkle(C_Parameter_baslangic_tarih, null);
        }

        if (bitisTarih.HasValue)
        {
            VeritabaniIslem.ParametreEkle(C_Parameter_bitis_tarih, (object)bitisTarih.Value);
        }
        else
        {
            VeritabaniIslem.ParametreEkle(C_Parameter_bitis_tarih, null);
        }

        VeriTablosu = VeritabaniIslem.TabloGetir();
    }
#endregion
}
