using System;
using System.Data;

public class ParametreKontrolleri
{
    private readonly VeritabaniIslemleri _veritabaniIslemleri;
    public enum Islem
    {
        EKLE,
        GUNCELLE
    }

    public ParametreKontrolleri(VeritabaniIslemleri veritabaniIslemleri)
    {
        _veritabaniIslemleri = veritabaniIslemleri;
    }

    public bool ParametreKontrol(Parametreler kayit, Islem islem)
    {
        var parametreKodu1 = kayit.Kod;
        if (parametreKodu1 != null)
        {
            kayit.Kod = parametreKodu1.Trim().ToUpperInvariant();
        }
        else
        {
            kayit.Kod = "".Trim().ToUpperInvariant();
        }

        var parametreAdi2 = kayit.Adi;
        if (parametreAdi2 != null)
        {
            kayit.Adi = parametreAdi2.Trim();
        }
        else
        {
            kayit.Adi = "".Trim();
        }

        if (string.IsNullOrWhiteSpace(kayit.Aciklama))
        {
            kayit.Aciklama = null;
        }
        else
        {
            kayit.Aciklama = kayit.Aciklama.Trim();
        }

        if (kayit.Kod.Length == 0 || kayit.Kod.Length > 50)
        {
            return Hata("Kod gereklidir ve 50 karakteri aşamaz.");
        }

        foreach (char karakter in kayit.Kod)
        {
            if (!(karakter >= 'A' && karakter <= 'Z') && !(karakter >= '0' && karakter <= '9') && karakter != '_')
            {
                return Hata("Kod yalnızca A-Z, rakam ve alt çizgi içerebilir.");
            }
        }

        if (string.IsNullOrWhiteSpace(kayit.Adi) || kayit.Adi.Length > 150)
        {
            return Hata("Parametre adı gereklidir ve 150 karakteri aşamaz.");
        }

        if (kayit.Aciklama != null && kayit.Aciklama.Length > 500)
        {
            return Hata("Tanım açıklaması 500 karakteri aşamaz.");
        }

        if (kayit.SiraNo < 0)
        {
            return Hata("Sıra numarası sıfır veya daha büyük olmalıdır.");
        }

        string ip;
        if (islem == Islem.EKLE)
        {
            ip = kayit.EkleyenIp;
        }
        else
        {
            ip = kayit.GuncelleyenIp;
        }

        if (ip != null && ip.Length > 50)
        {
            return Hata("Kayıt IP bilgisi 50 karakteri aşamaz.");
        }

        // Mevcut donanim kontrolleri gibi BAGIMLI transaction gerektirir.
        if (!_veritabaniIslemleri.UygulamaKilidiAl("ModbusParametreAyar", false))
        {
            return false;
        }

        if (islem == Islem.GUNCELLE)
        {
            Parametreler eskiKayit = new Parametreler(_veritabaniIslemleri);
            eskiKayit.Id = kayit.Id;
            if (!eskiKayit.Doldur())
            {
                return Hata("Güncellenecek parametre bulunamadı.");
            }

            if (eskiKayit.GrupId != kayit.GrupId || eskiKayit.Kod != kayit.Kod)
            {
                return Hata("Parametrenin grubu ve kodu değiştirilemez.");
            }

            return true;
        }

        bool aktifGrup = false;
        ParametreGruplari gruplar = new ParametreGruplari(_veritabaniIslemleri);
        using (DataTable tablo = gruplar.Listele(true))
        {
            foreach (DataRow satir in tablo.Rows)
            {
                if (Convert.ToInt32(satir[OrtakAlanlar.C_Sutun_id]) == kayit.GrupId)
                {
                    aktifGrup = true;
                    break;
                }
            }
        }

        if (!aktifGrup)
        {
            return Hata("Seçilen parametre grubu bulunamadı veya pasiftir.");
        }

        Parametreler parametreler = new Parametreler(_veritabaniIslemleri);
        using (DataTable tablo = parametreler.Listele(kayit.GrupId))
        {
            foreach (DataRow satir in tablo.Rows)
            {
                if (string.Equals(satir[Parametreler.C_Sutun_kod].ToString(), kayit.Kod, StringComparison.OrdinalIgnoreCase))
                {
                    return Hata("Bu grupta aynı kodla bir parametre zaten bulunmaktadır.");
                }
            }
        }

        return true;
    }

    public bool DurusNedeniKontrol(int id, string aciklama, out string nedenMetni, out string durusAciklamasi)
    {
        nedenMetni = null;
        if (string.IsNullOrWhiteSpace(aciklama))
        {
            durusAciklamasi = null;
        }
        else
        {
            durusAciklamasi = aciklama.Trim();
        }

        if (id <= 0)
        {
            return Hata("Geçerli bir duruş nedeni seçiniz.");
        }

        if (durusAciklamasi != null && durusAciklamasi.Length > 500)
        {
            return Hata("Duruş açıklaması 500 karakteri aşamaz.");
        }

        if (!_veritabaniIslemleri.UygulamaKilidiAl("ModbusParametreAyar", false))
        {
            return false;
        }

        Parametreler kayit = new Parametreler(_veritabaniIslemleri);
        kayit.Id = id;
        if (!kayit.Doldur()
            || !kayit.AktifMi
            || !Convert.ToBoolean(kayit.SonucKayit["grup_aktif_mi"])
            || kayit.SonucKayit["grup_kodu"].ToString() != ParametreGruplari.C_Grup_MakineDurusNedeni)
        {
            return Hata("Seçilen duruş nedeni bulunamadı veya aktif değildir.");
        }

        if (kayit.AciklamaZorunluMu && durusAciklamasi == null)
        {
            return Hata("Seçilen duruş nedeni için açıklama giriniz.");
        }

        if (durusAciklamasi == null)
        {
            nedenMetni = kayit.Adi + "";
        }
        else
        {
            nedenMetni = kayit.Adi + (" - " + durusAciklamasi);
        }

        if (nedenMetni.Length > 500)
        {
            return Hata("Duruş nedeni ve açıklamasının toplamı 500 karakteri aşamaz.");
        }

        return true;
    }

    private bool Hata(string mesaj)
    {
        return _veritabaniIslemleri.HataBildir(mesaj);
    }

    public bool SilmeKontrol(int id)
    {
        if (!_veritabaniIslemleri.UygulamaKilidiAl("ModbusParametreAyar", false))
        {
            return false;
        }

        Parametreler kayit = new Parametreler(_veritabaniIslemleri);
        kayit.Id = id;
        if (!kayit.Doldur())
        {
            return Hata("Silinecek parametre bulunamadı.");
        }

        string gecmisNeden = null;
        if (kayit.SonucKayit["grup_kodu"].ToString() == ParametreGruplari.C_Grup_MakineDurusNedeni)
        {
            // Eski sekiz sabit neden; adi degistirilse de ilk metni korunur.
            switch (kayit.Kod)
            {
                case "MEKANIK_ARIZA":
                    gecmisNeden = "Mekanik Arıza";
                    break;
                case "HATALI_OLCU":
                    gecmisNeden = "Hatalı Ölçü";
                    break;
                case "YUKSEK_FIRE":
                    gecmisNeden = "Yüksek Fire";
                    break;
                case "IPLIK_KOPUSU":
                    gecmisNeden = "İplik Kopuşu";
                    break;
                case "PROGRAMLI_BAKIM":
                    gecmisNeden = "Programlı Bakım";
                    break;
                case "FAZLA_ADET":
                    gecmisNeden = "Fazla Adet";
                    break;
                case "OPERATOR_TALEBI":
                    gecmisNeden = "Operatör Talebi";
                    break;
                case "FABRIKA_MUDURU_TALEBI":
                    gecmisNeden = "Fabrika Müdürü Talebi";
                    break;
            }
        }

        _veritabaniIslemleri.SpAdi = Parametreler.C_Sp_KullanimKayitlariGetir;
        _veritabaniIslemleri.ParametreEkle(OrtakAlanlar.C_Sutun_id, id);
        using (DataTable tablo = _veritabaniIslemleri.TabloGetir())
        {
            foreach (DataRow satir in tablo.Rows)
            {
                bool kimlikleKullanilmis = satir["durus_nedeni_parametre_id"] != DBNull.Value && Convert.ToInt32(satir["durus_nedeni_parametre_id"]) == id;
                bool gecmisteKullanilmis = satir["durus_nedeni_parametre_id"] == DBNull.Value && gecmisNeden != null && string.Equals(satir["islem_nedeni"].ToString(), gecmisNeden, StringComparison.Ordinal);
                if (kimlikleKullanilmis || gecmisteKullanilmis)
                {
                    return Hata("Bu parametre talimat veya duruş kayıtlarında kullanıldığı için silinemez. Pasifleştirebilirsiniz.");
                }
            }
        }

        return true;
    }
}
