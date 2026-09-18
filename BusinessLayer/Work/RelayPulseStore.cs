using System;
using System.Collections.Generic;
using System.Data;

// İş akışına depo arayüzü sağlar; bütün veri işlemleri Entity ve prosedürlerdedir.
public sealed class RelayPulseStore : IRelayPulseStore
{
    private static Lease BaglantiAc()
    {
        var veritabani = new VeritabaniIslemleri { LogYasak = true, HatalariFirlat = true };
        try
        {
            // Oturum kilidi bağlantı kapanınca bırakılır; bağlantı havuzuna taşınmaz.
            // Her yazma bağımsız kalıcıdır; sayfa transaction'ından etkilenmez.
            veritabani.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ, 3, false);
            return new Lease(veritabani);
        }
        catch { veritabani.Bitir(); throw; }
    }

    public IRelayPulseLease TryLock(MakineRoleBaglantilari connection)
    {
        var baglanti = BaglantiAc();
        try
        {
            if (baglanti.Kayitlar.CihazKilidiAl(connection.Ip, connection.HttpPort)) return baglanti;
            baglanti.Dispose();
            return null;
        }
        catch { baglanti.Dispose(); throw; }
    }

    public IList<RelayPulse> GetDue(bool recovery)
    {
        var sonuc = new List<RelayPulse>();
        using (var baglanti = BaglantiAc())
        {
            baglanti.Kayitlar.SuresiDolanlariGetir(recovery);
            foreach (DataRow satir in baglanti.Kayitlar.VeriTablosu.Rows)
                sonuc.Add(RelayPulses.SatirdanGetir(satir));
        }
        return sonuc;
    }

    public bool HasActive(int machineId)
    {
        using (var baglanti = BaglantiAc()) return baglanti.Kayitlar.AktifKayitVarMi(machineId);
    }

    public bool HasAnyActive()
    {
        using (var baglanti = BaglantiAc()) return baglanti.Kayitlar.AktifKayitVarMi(null);
    }

    public string GetWarnings()
    {
        var mesajlar = new List<string>();
        using (var baglanti = BaglantiAc())
        {
            baglanti.Kayitlar.UyarilariGetir();
            foreach (DataRow satir in baglanti.Kayitlar.VeriTablosu.Rows)
                mesajlar.Add(string.Format("Makine {0} / {1} kanal {2}: {3}", satir["MakineId"], satir["ControllerIp"], satir["RelayChannel"],
                    satir["SonHata"] == DBNull.Value ? "ON dönüşü gecikti; fiziksel röle IO durumu henüz doğrulanmadı." : RelayPulseService.UserError(satir["SonHata"].ToString())));
        }
        return string.Join(" | ", mesajlar);
    }

    private sealed class Lease : IRelayPulseLease
    {
        private readonly VeritabaniIslemleri veritabani;
        public readonly RelayPulses Kayitlar;

        public Lease(VeritabaniIslemleri veritabani)
        {
            this.veritabani = veritabani;
            Kayitlar = new RelayPulses(veritabani);
        }

        public RelayPulse Get(long id)
        {
            Kayitlar.PulseId = id;
            return Kayitlar.Doldur() ? Kayitlar.Kayit : null;
        }

        public void Create(RelayPulse pulse)
        {
            Kayitlar.Kayit = pulse;
            if (!Kayitlar.Ekle()) throw new InvalidOperationException("Pulse kaydı oluşturulamadı.");
        }

        public void Save(RelayPulse pulse)
        {
            Kayitlar.Kayit = pulse;
            if (!Kayitlar.Guncelle()) throw new InvalidOperationException("Pulse kaydı güncellenemedi.");
        }

        public void Log(RelayPulse pulse, string command, string response, bool success, string error)
        {
            var olay = new RelayPulseEvents(veritabani)
            {
                PulseId = pulse.Id, Komut = command, HamCevap = response, Basarili = success,
                Hata = error, ReleaseDenemeSayisi = pulse.ReleaseDenemeSayisi, KritikHata = pulse.KritikHata
            };
            if (!olay.Ekle()) throw new InvalidOperationException("Pulse olay kaydı oluşturulamadı.");
        }

        public void Dispose() { veritabani.Bitir(); }
    }
}