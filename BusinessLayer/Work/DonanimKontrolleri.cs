using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;

public class DonanimKontrolleri
{
    private readonly VeritabaniIslemleri _veritabaniIslemleri;

    public enum Islem
    {
        EKLE,
        GUNCELLE,
        SIL
    }

    public DonanimKontrolleri(VeritabaniIslemleri veritabaniIslemleri)
    {
        _veritabaniIslemleri = veritabaniIslemleri;
    }

    public bool AyarKilidiAl()
    {
        if (!_veritabaniIslemleri.UygulamaKilidiAl("ModbusDonanimAyar", false)) return false;
        // Bekleyen pulse'lar bırakılana kadar kayıtlı fiziksel adres korunur.
        if (new RelayPulseStore().HasAnyActive())
            return Hata("Aktif röle pulse işlemi varken donanım ayarları değiştirilemez. ON doğrulamasını bekleyiniz.");
        return true;
    }

    public bool KomutAyarKilidiAl()
    {
        return _veritabaniIslemleri.UygulamaKilidiAl("ModbusDonanimAyar", true);
    }

    public bool CihazKilidiAl(int ethernetKartId)
    {
        return _veritabaniIslemleri.UygulamaKilidiAl("ModbusRoleCihaz:" + ethernetKartId, false);
    }

    public bool EthernetKontrol(EthernetKartlari kayit, Islem islem)
    {
        if (!AyarKilidiAl())
        {
            return false;
        }

        int id = islem == Islem.EKLE ? 0 : kayit.Id;
        DataTable kartlar = TabloGetir(EthernetKartlari.C_Sp_TumunuGetir);

        if (islem != Islem.EKLE && KayitBul(kartlar, id) == null)
        {
            return Hata("Kayıt bulunamadı.");
        }

        DataRow[] roleler = TabloGetir(RoleKartlari.C_Sp_TumunuGetir).AsEnumerable()
            .Where(x => Convert.ToInt32(x["ethernet_kart_id"]) == id).ToArray();

        if (islem == Islem.SIL)
        {
            return roleler.Length == 0 || Hata("Bağlı röle kartı olan Ethernet kartı silinemez.");
        }

        IPAddress adres;
        if (string.IsNullOrWhiteSpace(kayit.KartAdi) || kayit.KartAdi.Length > 100
            || string.IsNullOrWhiteSpace(kayit.Model) || kayit.Model.Length > 50
            || !IPAddress.TryParse(kayit.Ip, out adres) || adres.AddressFamily != AddressFamily.InterNetwork
            || kayit.HttpPort < 1 || kayit.HttpPort > 65535)
        {
            return Hata("Kart adı, model, IPv4 adresi veya HTTP portu geçersiz.");
        }

        kayit.Ip = adres.ToString();
        if (kartlar.AsEnumerable().Any(x => Convert.ToInt32(x["id"]) != id
            && x["ip"].ToString() == kayit.Ip && Convert.ToInt32(x["http_port"]) == kayit.HttpPort))
        {
            return Hata("Bu IP ve HTTP portu zaten tanımlı.");
        }

        if (!kayit.AktifMi && roleler.Any(x => Convert.ToBoolean(x["aktif_mi"])))
        {
            return Hata("Önce bağlı röle kartını pasife alınız.");
        }

        if (islem == Islem.GUNCELLE)
        {
            int[] roleIdleri = roleler.Select(x => Convert.ToInt32(x["id"])).ToArray();
            foreach (DataRow baglanti in TabloGetir(MakineRoleBaglantilari.C_Sp_TumunuGetir).Rows)
            {
                if (Convert.ToBoolean(baglanti["aktif_mi"])
                    && roleIdleri.Contains(Convert.ToInt32(baglanti["role_kart_id"]))
                    && AcikDurusVarMi(Convert.ToInt32(baglanti["makine_id"])))
                {
                    return Hata("Açık duruş kaydı olan makine bulunduğu için cihaz değiştirilemez.");
                }
            }
        }

        return true;
    }

    public bool RoleKontrol(RoleKartlari kayit, Islem islem)
    {
        if (!AyarKilidiAl())
        {
            return false;
        }

        int id = islem == Islem.EKLE ? 0 : kayit.Id;
        DataTable roleler = TabloGetir(RoleKartlari.C_Sp_TumunuGetir);
        DataRow eskiKayit = KayitBul(roleler, id);

        if (islem != Islem.EKLE && eskiKayit == null)
        {
            return Hata("Kayıt bulunamadı.");
        }

        DataRow[] baglantilar = TabloGetir(MakineRoleBaglantilari.C_Sp_TumunuGetir).AsEnumerable()
            .Where(x => Convert.ToInt32(x["role_kart_id"]) == id).ToArray();

        if (islem == Islem.SIL)
        {
            return baglantilar.Length == 0 || Hata("Makine bağlantısı olan röle kartı silinemez.");
        }

        if (string.IsNullOrWhiteSpace(kayit.RoleAdi) || kayit.RoleAdi.Length > 100)
        {
            return Hata("Röle adı gereklidir ve en fazla 100 karakter olabilir.");
        }

        EthernetKartlari ethernet = new EthernetKartlari(_veritabaniIslemleri);
        ethernet.Id = kayit.EthernetKartId;
        if (!ethernet.Doldur() || (kayit.AktifMi && !ethernet.AktifMi))
        {
            return Hata("Aktif bir Ethernet kartı seçiniz.");
        }

        if (roleler.AsEnumerable().Any(x => Convert.ToInt32(x["id"]) != id
            && Convert.ToInt32(x["ethernet_kart_id"]) == kayit.EthernetKartId))
        {
            return Hata("Bu Ethernet kartına başka bir röle kartı bağlı.");
        }

        if (eskiKayit != null && baglantilar.Any(x => Convert.ToBoolean(x["aktif_mi"]))
            && (!kayit.AktifMi || Convert.ToInt32(eskiKayit["ethernet_kart_id"]) != kayit.EthernetKartId))
        {
            return Hata("Önce röle kartının aktif makine bağlantılarını kaldırınız.");
        }

        return true;
    }

    public bool BaglantiKontrol(MakineRoleBaglantilari kayit, Islem islem)
    {
        if (!AyarKilidiAl())
        {
            return false;
        }

        int id = islem == Islem.EKLE ? 0 : kayit.Id;
        DataTable baglantilar = TabloGetir(MakineRoleBaglantilari.C_Sp_TumunuGetir);
        DataRow eskiKayit = KayitBul(baglantilar, id);

        if (islem != Islem.EKLE && eskiKayit == null)
        {
            return Hata("Kayıt bulunamadı.");
        }

        if (eskiKayit != null && AcikDurusVarMi(Convert.ToInt32(eskiKayit["makine_id"])))
        {
            return Hata("Açık duruş kaydı varken bağlantı değiştirilemez veya silinemez.");
        }

        if (islem == Islem.SIL)
        {
            return true;
        }

        if (kayit.KanalNo < 1 || kayit.KanalNo > 16)
        {
            return Hata("Röle kanalı 1 ile 16 arasında olmalıdır.");
        }

        Makineler makine = new Makineler(_veritabaniIslemleri);
        makine.Id = kayit.MakineId;
        if (!makine.Doldur() || (kayit.AktifMi && !makine.AktifMi))
        {
            return Hata("Aktif bir makine seçiniz.");
        }

        RoleKartlari role = new RoleKartlari(_veritabaniIslemleri);
        role.Id = kayit.RoleKartId;
        if (!role.Doldur())
        {
            return Hata("Bir röle kartı seçiniz.");
        }

        EthernetKartlari ethernet = new EthernetKartlari(_veritabaniIslemleri);
        ethernet.Id = role.EthernetKartId;
        if (!ethernet.Doldur() || (kayit.AktifMi && (!role.AktifMi || !ethernet.AktifMi)))
        {
            return Hata("Aktif bir röle ve Ethernet kartı seçiniz.");
        }

        if (kayit.AktifMi && baglantilar.AsEnumerable().Any(x => Convert.ToInt32(x["id"]) != id
            && Convert.ToBoolean(x["aktif_mi"])
            && (Convert.ToInt32(x["makine_id"]) == kayit.MakineId
                || (Convert.ToInt32(x["role_kart_id"]) == kayit.RoleKartId
                    && Convert.ToInt32(x["kanal_no"]) == kayit.KanalNo))))
        {
            return Hata("Makine veya röle kanalı başka bir aktif bağlantıda kullanılıyor.");
        }

        if (AcikDurusVarMi(kayit.MakineId))
        {
            return Hata("Açık duruş kaydı varken bağlantı değiştirilemez. Önce duruşu sonlandırınız.");
        }

        return true;
    }

    private bool AcikDurusVarMi(int makineId)
    {
        MakineLoglari log = new MakineLoglari(_veritabaniIslemleri);
        log.MakineId = makineId;
        return log.AcikKayitGetir();
    }

    private DataTable TabloGetir(string prosedur)
    {
        _veritabaniIslemleri.SpAdi = prosedur;
        return _veritabaniIslemleri.TabloGetir();
    }

    private DataRow KayitBul(DataTable tablo, int id)
    {
        return tablo.AsEnumerable().FirstOrDefault(x => Convert.ToInt32(x["id"]) == id);
    }

    private bool Hata(string mesaj)
    {
        return _veritabaniIslemleri.HataBildir(mesaj);
    }
}

public class DonanimIslemHatasi : InvalidOperationException
{
    public DonanimIslemHatasi(string mesaj) : base(mesaj)
    {
    }
}
