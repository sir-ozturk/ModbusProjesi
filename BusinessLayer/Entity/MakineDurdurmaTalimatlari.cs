using System;
using System.Collections.Generic;
using System.Data;

public enum TalimatDurumu : byte
{
    Bekliyor = 0,
    Isleniyor = 1,
    Tamamlandi = 2,
    Hatali = 3,
    KontrolGerekli = 4
}

public class MakineDurdurmaTalimatlari : OrtakAlanlar
{
    public MakineDurdurmaTalimatlari(VeritabaniIslemleri veritabaniIslemleri = null)
    {
        VeritabaniIslem = veritabaniIslemleri;
    }

#region SABİTLER
    public const string C_Tablo = "dbo.MakineDurdurmaTalimatlari";
    public const string C_Sp_KomutOncesiKontrol = "dbo.SP_MakineDurdurmaTalimatlari_KOMUT_ONCESI_KONTROL";
    public const string C_Sp_Ekle = "dbo.SP_MakineDurdurmaTalimatlari_EKLE";
    public const string C_Sp_Listele = "dbo.SP_MakineDurdurmaTalimatlari_LISTELE";
    public const string C_Sp_YarimKalanlariGetir = "dbo.SP_MakineDurdurmaTalimatlari_YARIM_KALANLARI_GETIR";
    public const string C_Sp_SiradakiniAl = "dbo.SP_MakineDurdurmaTalimatlari_SIRADAKINI_AL";
    public const string C_Sp_Sonuclandir = "dbo.SP_MakineDurdurmaTalimatlari_SONUCLANDIR";
    public const string C_Sutun_makine_id = "makine_id";
    public const string C_Sutun_url = "url";
    public const string C_Sutun_islem_nedeni = "islem_nedeni";
    public const string C_Sutun_durus_nedeni_parametre_id = "durus_nedeni_parametre_id";
    public const string C_Sutun_durus_aciklamasi = "durus_aciklamasi";
    public const string C_Sutun_islem_durumu = "islem_durumu";
    public const string C_Sutun_sonuc = "sonuc";
    public const string C_Sutun_islem_baslangic_tarih = "islem_baslangic_tarih";
    public const string C_Sutun_islem_bitis_tarih = "islem_bitis_tarih";
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

    private int? durusNedeniParametreId;
    public int? DurusNedeniParametreId
    {
        get
        {
            return durusNedeniParametreId;
        }

        set
        {
            durusNedeniParametreId = value;
        }
    }

    private string durusAciklamasi;
    public string DurusAciklamasi
    {
        get
        {
            return durusAciklamasi;
        }

        set
        {
            durusAciklamasi = value;
        }
    }

    private TalimatDurumu islemDurumu;
    public TalimatDurumu IslemDurumu
    {
        get
        {
            return islemDurumu;
        }

        set
        {
            islemDurumu = value;
        }
    }

    private string sonuc;
    public string Sonuc
    {
        get
        {
            return sonuc;
        }

        set
        {
            sonuc = value;
        }
    }

    private DateTime? islemBaslangicTarih;
    public DateTime? IslemBaslangicTarih
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

    private readonly List<string> komutKilitleri = new List<string>();
#endregion
#region METOTLAR
    // Web tarafı yetki kontrolünden sonra açık bağlantı ile çağırır.
    // Satır döndüren prosedür kullanılır; HttpContext tabanlı genel log yolu kullanılmaz.
    public bool Ekle()
    {
        if (VeritabaniIslem == null)
        {
            throw new InvalidOperationException("Veritabanı bağlantısı gereklidir.");
        }

        try
        {
            string nedenMetni;
            string aciklama;
            var durusNedeniId1 = DurusNedeniParametreId;
            if (durusNedeniId1 != null)
            {
                if (!new ParametreKontrolleri(VeritabaniIslem).DurusNedeniKontrol(durusNedeniId1.Value, DurusAciklamasi, out nedenMetni, out aciklama))
                {
                    return false;
                }
            }
            else
            {
                if (!new ParametreKontrolleri(VeritabaniIslem).DurusNedeniKontrol(0, DurusAciklamasi, out nedenMetni, out aciklama))
                {
                    return false;
                }
            }

            IslemNedeni = nedenMetni;
            DurusAciklamasi = aciklama;
            VeritabaniIslem.SpAdi = C_Sp_Ekle;
            VeritabaniIslem.ParametreEkle(C_Sutun_makine_id, MakineId);
            VeritabaniIslem.ParametreEkle(C_Sutun_islem_nedeni, IslemNedeni);
            VeritabaniIslem.ParametreEkle(C_Sutun_durus_nedeni_parametre_id, DurusNedeniParametreId);
            VeritabaniIslem.ParametreEkle(C_Sutun_durus_aciklamasi, DurusAciklamasi);
            VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
            VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);
            SonucKayit = VeritabaniIslem.SatirGetir();
            if (SonucKayit == null)
            {
                return false;
            }

            Oku(SonucKayit);
            return true;
        }
        finally
        {
            VeritabaniIslem.ParametreleriSil();
        }
    }

    public DataTable Listele(int? makineId = null, TalimatDurumu? durum = null, int adet = 100)
    {
        if (VeritabaniIslem == null)
        {
            throw new InvalidOperationException("Veritabanı bağlantısı gereklidir.");
        }

        try
        {
            VeritabaniIslem.SpAdi = C_Sp_Listele;
            VeritabaniIslem.ParametreEkle(C_Sutun_makine_id, makineId);
            if (durum.HasValue)
            {
                VeritabaniIslem.ParametreEkle(C_Sutun_islem_durumu, (object)(byte)durum.Value);
            }
            else
            {
                VeritabaniIslem.ParametreEkle(C_Sutun_islem_durumu, null);
            }

            VeritabaniIslem.ParametreEkle("adet", adet);
            VeriTablosu = VeritabaniIslem.TabloGetir();
            return VeriTablosu;
        }
        finally
        {
            VeritabaniIslem.ParametreleriSil();
        }
    }

    public IList<MakineDurdurmaTalimatlari> YarimKalanlariGetir()
    {
        try
        {
            VeritabaniIslem.SpAdi = C_Sp_YarimKalanlariGetir;
            List<MakineDurdurmaTalimatlari> talimatlar = new List<MakineDurdurmaTalimatlari>();
            using (DataTable tablo = VeritabaniIslem.TabloGetir())
            {
                foreach (DataRow satir in tablo.Rows)
                {
                    talimatlar.Add(Satirdan(satir));
                }
            }

            return talimatlar;
        }
        finally
        {
            VeritabaniIslem.ParametreleriSil();
        }
    }

    public MakineDurdurmaTalimatlari SiradakiniAl(int gecerlilikSaniye)
    {
        try
        {
            VeritabaniIslem.SpAdi = C_Sp_SiradakiniAl;
            VeritabaniIslem.ParametreEkle("gecerlilik_saniye", gecerlilikSaniye);
            IsleyiciBilgileriniEkle();
            DataRow satir = VeritabaniIslem.SatirGetir();
            if (satir == null)
            {
                return null;
            }
            else
            {
                return Satirdan(satir);
            }
        }
        finally
        {
            VeritabaniIslem.ParametreleriSil();
        }
    }

    public MakineRoleBaglantilari BaglantiyiKilitle(int makineId)
    {
        if (komutKilitleri.Count != 0)
        {
            throw new InvalidOperationException("Önceki komut kilitleri bırakılmadı.");
        }

        KomutKilidiAl("ModbusDonanimAyar", true);
        try
        {
            VeritabaniIslem.SpAdi = MakineRoleBaglantilari.C_Sp_KomutGetir;
            VeritabaniIslem.ParametreEkle(C_Sutun_makine_id, makineId);
            DataRow satir = VeritabaniIslem.SatirGetir();
            if (satir == null)
            {
                return null;
            }

            MakineRoleBaglantilari baglanti = new MakineRoleBaglantilari(null)
            {
                MakineId = makineId,
                Id = Convert.ToInt32(satir[C_Sutun_id]),
                KanalNo = Convert.ToInt32(satir[MakineRoleBaglantilari.C_Sutun_kanal_no]),
                Ip = Convert.ToString(satir[MakineRoleBaglantilari.C_Sutun_ip]),
                HttpPort = Convert.ToInt32(satir[MakineRoleBaglantilari.C_Sutun_http_port]),
                EthernetKartId = Convert.ToInt32(satir[MakineRoleBaglantilari.C_Sutun_ethernet_kart_id]),
                RoleKartId = Convert.ToInt32(satir[MakineRoleBaglantilari.C_Sutun_role_kart_id]),
                AktifMi = true
            };
            KomutKilidiAl("ModbusRoleCihaz:" + baglanti.EthernetKartId, false);
            return baglanti;
        }
        finally
        {
            VeritabaniIslem.ParametreleriSil();
        }
    }

    public string KomutOncesiKontrol(MakineDurdurmaTalimatlari talimat, int gecerlilikSaniye)
    {
        try
        {
            VeritabaniIslem.SpAdi = C_Sp_KomutOncesiKontrol;
            VeritabaniIslem.ParametreEkle(C_Sutun_id, talimat.Id);
            VeritabaniIslem.ParametreEkle(C_Sutun_makine_id, talimat.MakineId);
            VeritabaniIslem.ParametreEkle("gecerlilik_saniye", gecerlilikSaniye);
            object sonuc = VeritabaniIslem.DegerGetir();
            if (sonuc == null || sonuc == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToString(sonuc);
            }
        }
        finally
        {
            VeritabaniIslem.ParametreleriSil();
        }
    }

    public void Sonuclandir(MakineDurdurmaTalimatlari talimat, TalimatDurumu durum, string sonuc, bool onDogrulandi, bool offDogrulandi)
    {
        try
        {
            VeritabaniIslem.SpAdi = C_Sp_Sonuclandir;
            VeritabaniIslem.ParametreEkle(C_Sutun_id, talimat.Id);
            VeritabaniIslem.ParametreEkle(C_Sutun_islem_durumu, (byte)durum);
            VeritabaniIslem.ParametreEkle(C_Sutun_sonuc, sonuc);
            VeritabaniIslem.ParametreEkle("on_dogrulandi", onDogrulandi);
            VeritabaniIslem.ParametreEkle("off_dogrulandi", offDogrulandi);
            IsleyiciBilgileriniEkle();
            // Duruş kaydını da aynı prosedür oluşturur; burada ikinci kez log eklenmez.
            using (DataTable tablo = VeritabaniIslem.TabloGetir())
            {
                if (tablo.Rows.Count != 1)
                {
                    throw new InvalidOperationException("Talimat sonucu doğrulanamadı.");
                }

                talimat.Oku(tablo.Rows[0]);
            }
        }
        finally
        {
            VeritabaniIslem.ParametreleriSil();
        }
    }

    private void IsleyiciBilgileriniEkle()
    {
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);
    }

    private void KomutKilidiAl(string kaynak, bool paylasimli)
    {
        VeritabaniIslem.OturumKilidiAl(kaynak, paylasimli);
        komutKilitleri.Add(kaynak);
    }

    public void KomutKilitleriniBirak()
    {
        for (int i = komutKilitleri.Count - 1; i >= 0; i--)
        {
            VeritabaniIslem.OturumKilidiniBirak(komutKilitleri[i]);
            komutKilitleri.RemoveAt(i);
        }
    }

    public static MakineDurdurmaTalimatlari Satirdan(DataRow satir)
    {
        MakineDurdurmaTalimatlari talimat = new MakineDurdurmaTalimatlari();
        talimat.Oku(satir);
        return talimat;
    }

    private void Oku(DataRow satir)
    {
        Id = Convert.ToInt32(satir[C_Sutun_id]);
        MakineId = Convert.ToInt32(satir[C_Sutun_makine_id]);
        Url = Convert.ToString(satir[C_Sutun_url]);
        IslemNedeni = Convert.ToString(satir[C_Sutun_islem_nedeni]);
        if (satir.IsNull(C_Sutun_durus_nedeni_parametre_id))
        {
            DurusNedeniParametreId = (int? )null;
        }
        else
        {
            DurusNedeniParametreId = Convert.ToInt32(satir[C_Sutun_durus_nedeni_parametre_id]);
        }

        if (satir.IsNull(C_Sutun_durus_aciklamasi))
        {
            DurusAciklamasi = null;
        }
        else
        {
            DurusAciklamasi = Convert.ToString(satir[C_Sutun_durus_aciklamasi]);
        }

        IslemDurumu = (TalimatDurumu)Convert.ToByte(satir[C_Sutun_islem_durumu]);
        if (satir.IsNull(C_Sutun_sonuc))
        {
            Sonuc = null;
        }
        else
        {
            Sonuc = Convert.ToString(satir[C_Sutun_sonuc]);
        }

        IslemBaslangicTarih = Tarih(satir, C_Sutun_islem_baslangic_tarih);
        IslemBitisTarih = Tarih(satir, C_Sutun_islem_bitis_tarih);
        EkleyenId = Convert.ToInt32(satir[C_Sutun_ekleyen_id]);
        EkleyenIp = Convert.ToString(satir[C_Sutun_ekleyen_ip]);
        EklenmeTarih = Convert.ToDateTime(satir[C_Sutun_eklenme_tarih]);
        if (satir.IsNull(C_Sutun_guncelleyen_id))
        {
            GuncelleyenId = 0;
        }
        else
        {
            GuncelleyenId = Convert.ToInt32(satir[C_Sutun_guncelleyen_id]);
        }

        GuncelleyenIp = Convert.ToString(satir[C_Sutun_guncelleyen_ip]);
        var guncellenmeTarihi2 = Tarih(satir, C_Sutun_guncellenme_tarih);
        if (guncellenmeTarihi2 != null)
        {
            GuncellenmeTarih = guncellenmeTarihi2.Value;
        }
        else
        {
            GuncellenmeTarih = DateTime.MinValue;
        }

        AktifMi = Convert.ToBoolean(satir[C_Sutun_aktif_mi]);
    }

    private static DateTime? Tarih(DataRow satir, string sutunAdi)
    {
        if (satir.IsNull(sutunAdi))
        {
            return (DateTime? )null;
        }
        else
        {
            return Convert.ToDateTime(satir[sutunAdi]);
        }
    }
#endregion
}
