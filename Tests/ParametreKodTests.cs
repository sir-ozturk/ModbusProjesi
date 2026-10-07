using System;
using System.Data;

public class VeritabaniIslemleri
{
    public string SpAdi;
    public string SonHataMesaji;
    public bool KilitAlindi;
    public bool KilitVerilsin = true;
    public DataTable Gruplar = new DataTable();
    public DataTable Kayitlar = new DataTable();
    public DataTable Kullanimlar = new DataTable();
    public Parametreler EskiKayit;

    public VeritabaniIslemleri()
    {
        Gruplar.Columns.Add("id", typeof(int));
        Gruplar.Columns.Add("kod", typeof(string));
        Kayitlar.Columns.Add("kod", typeof(string));
        Kayitlar.Columns.Add("aktif_mi", typeof(bool)).DefaultValue = true;
        Kullanimlar.Columns.Add("durus_nedeni_parametre_id", typeof(int));
        Kullanimlar.Columns.Add("islem_nedeni", typeof(string));
    }

    public bool HataBildir(string mesaj) { SonHataMesaji = mesaj; return false; }
    public bool UygulamaKilidiAl(string kaynak, bool paylasimli) { KilitAlindi = KilitVerilsin; return KilitAlindi; }
    public void ParametreEkle(string ad, object deger) { }
    public DataTable TabloGetir() { return Kullanimlar.Copy(); }
}

public class OrtakAlanlar { public const string C_Sutun_id = "id"; }

public class ParametreGruplari
{
    public const string C_Grup_MakineModeli = "MAKINE_MODELI";
    public const string C_Grup_MakineDurusNedeni = "MAKINE_DURUS_NEDENI";
    public const string C_Sutun_kod = "kod";
    private VeritabaniIslemleri db;
    public ParametreGruplari(VeritabaniIslemleri veritabani) { db = veritabani; }
    public DataTable Listele(bool aktifMi) { return db.Gruplar.Copy(); }
}

public class Parametreler
{
    public const string C_Sutun_kod = "kod";
    public const string C_Sp_KullanimKayitlariGetir = "KULLANIM";
    private VeritabaniIslemleri db;
    public int Id;
    public int GrupId;
    public string Kod;
    public string Adi;
    public string Aciklama;
    public int SiraNo;
    public bool AktifMi = true;
    public bool AciklamaZorunluMu;
    public string EkleyenIp;
    public string GuncelleyenIp;
    public DataRow SonucKayit;
    public Parametreler(VeritabaniIslemleri veritabani) { db = veritabani; }
    public DataTable Listele(int grupId)
    {
        if (!db.KilitAlindi) throw new Exception("Kodlar kilit alınmadan okundu.");
        return db.Kayitlar.Copy();
    }
    public bool Doldur()
    {
        if (db.EskiKayit == null) return false;
        GrupId = db.EskiKayit.GrupId;
        Kod = db.EskiKayit.Kod;
        Adi = db.EskiKayit.Adi;
        SonucKayit = db.EskiKayit.SonucKayit;
        return true;
    }
}

public static class ParametreKodTests
{
    private static int sayac;
    private static void Kontrol(bool sonuc, string ad)
    {
        if (!sonuc) throw new Exception(ad);
        sayac++;
        Console.WriteLine("OK: " + ad);
    }

    private static VeritabaniIslemleri Hazirla(string grupKodu)
    {
        VeritabaniIslemleri db = new VeritabaniIslemleri();
        db.Gruplar.Rows.Add(1, grupKodu);
        return db;
    }

    private static Parametreler YeniKayit(VeritabaniIslemleri db)
    {
        Parametreler kayit = new Parametreler(db);
        kayit.GrupId = 1;
        kayit.Adi = "Yeni seçenek";
        return kayit;
    }

    private static void KodBekle(VeritabaniIslemleri db, string beklenen, string ad)
    {
        Parametreler kayit = YeniKayit(db);
        // Elle gönderilen kod dikkate alınmamalı.
        kayit.Kod = "ELLE_GONDERILEN_KOD";
        bool sonuc = new ParametreKontrolleri(db).ParametreKontrol(kayit, ParametreKontrolleri.Islem.EKLE);
        Kontrol(sonuc && kayit.Kod == beklenen, ad);
    }

    public static int Main()
    {
        try
        {
            VeritabaniIslemleri db = Hazirla("MAKINE_MODELI");
            KodBekle(db, "MAKINE_MODELI_001", "Boş grupta 001 ve elle kodun yok sayılması");
            db.Kayitlar.Rows.Add("MAKINE_MODELI_001");
            db.Kayitlar.Rows.Add("MAKINE_MODELI_003");
            KodBekle(db, "MAKINE_MODELI_002", "Aradaki boş numara");
            db.Kayitlar.Rows.Add("MAKINE_MODELI_002", false);
            KodBekle(db, "MAKINE_MODELI_004", "Pasif dahil mevcut kodun atlanması");
            db = Hazirla("MAKINE_DURUS_NEDENI");
            for (int i = 1; i <= 9; i++)
            {
                if (i != 8) db.Kayitlar.Rows.Add("MAKINE_DURUS_NEDENI_" + i.ToString("D3"));
            }
            KodBekle(db, "MAKINE_DURUS_NEDENI_008", "009 varken silinen 008 kullanılır");
            db.Kayitlar.Rows.Add("MAKINE_DURUS_NEDENI_008");
            KodBekle(db, "MAKINE_DURUS_NEDENI_010", "Boşluk kalmayınca 010");
            db = Hazirla("MAKINE_MODELI");
            for (int i = 1; i <= 999; i++) db.Kayitlar.Rows.Add("MAKINE_MODELI_" + i.ToString("D3"));
            KodBekle(db, "MAKINE_MODELI_1000", "999 sonrasında numara kesilmez");
            db.KilitVerilsin = false;
            Kontrol(!new ParametreKontrolleri(db).ParametreKontrol(YeniKayit(db), ParametreKontrolleri.Islem.EKLE), "Kilit alınamazsa ekleme reddedilir");
            db = Hazirla("MAKINE_MODELI");
            Parametreler kayit = YeniKayit(db);
            kayit.Id = 10;
            kayit.Kod = "MAKINE_MODELI_007";
            DataTable sonucTablosu = new DataTable();
            sonucTablosu.Columns.Add("grup_kodu");
            kayit.SonucKayit = sonucTablosu.Rows.Add("MAKINE_MODELI");
            db.EskiKayit = kayit;
            Kontrol(new ParametreKontrolleri(db).ParametreKontrol(kayit, ParametreKontrolleri.Islem.GUNCELLE) && kayit.Kod == "MAKINE_MODELI_007", "Güncellemede kod korunur");
            Parametreler degisen = YeniKayit(db);
            degisen.Id = 10;
            degisen.Kod = "MAKINE_MODELI_008";
            Kontrol(!new ParametreKontrolleri(db).ParametreKontrol(degisen, ParametreKontrolleri.Islem.GUNCELLE), "Güncellemede kod değiştirilemez");
            kayit.Kod = "MAKINE_DURUS_NEDENI_008";
            db.Kullanimlar.Rows.Add(100, "Fabrika Müdürü Talebi");
            Kontrol(new ParametreKontrolleri(db).SilmeKontrol(10), "Tekrar kullanılan numara eski nedenle karıştırılmaz");
            db.Kullanimlar.Rows.Add(10, "Yeni seçenek");
            Kontrol(!new ParametreKontrolleri(db).SilmeKontrol(10), "Kimliği kullanılan kayıt silinemez");
            Console.WriteLine(sayac + " parametre senaryosu doğrulandı.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex.Message); return 1; }
    }
}
