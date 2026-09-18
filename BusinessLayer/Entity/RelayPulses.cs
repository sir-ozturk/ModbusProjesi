using System;
using System.Data;

public class RelayPulses : OrtakAlanlar
{
    public RelayPulses(VeritabaniIslemleri veritabaniIslemleri)
    {
        VeritabaniIslem = veritabaniIslemleri;
    }

    #region SABİTLER

    public const string C_Tablo = "dbo.RelayPulses";
    public const string C_Sp_Ekle = "dbo.SP_RelayPulses_EKLE";
    public const string C_Sp_Guncelle = "dbo.SP_RelayPulses_GUNCELLE";
    public const string C_Sp_Doldur = "dbo.SP_RelayPulses_DOLDUR";
    public const string C_Sp_SuresiDolanlariGetir = "dbo.SP_RelayPulses_SURESI_DOLANLARI_GETIR";
    public const string C_Sp_AktifKayitVarMi = "dbo.SP_RelayPulses_AKTIF_KAYIT_VAR_MI";
    public const string C_Sp_UyarilariGetir = "dbo.SP_RelayPulses_UYARILARI_GETIR";
    public const string C_Sp_CihazKilidiAl = "dbo.SP_RelayPulses_CIHAZ_KILIDI_AL";

    #endregion

    #region NESNELER

    // Pulse kimlikleri bigint olduğundan OrtakAlanlar.Id yerine kendi alanı kullanılır.

    public long pulseId;
    public long PulseId
    {
        get
        {
            return pulseId;
        }
        set
        {
            pulseId = value;
        }
    }
    
    public RelayPulse kayit;
    public RelayPulse Kayit
    {
        get
        {
            return kayit;
        }
        set
        {
            kayit = value;
        }
    }

    #endregion

    #region METOTLAR

    private void IslemHazirla(string prosedur)
    {
        VeritabaniIslem.ParametreleriSil();
        VeritabaniIslem.SpAdi = prosedur;
    }

    public bool Ekle()
    {
        IslemHazirla(C_Sp_Ekle);
        VeritabaniIslem.ParametreEkle("MakineId", Kayit.MakineId);
        VeritabaniIslem.ParametreEkle("ControllerIp", Kayit.ControllerIp);
        VeritabaniIslem.ParametreEkle("HttpPort", Kayit.HttpPort);
        VeritabaniIslem.ParametreEkle("RelayChannel", Kayit.RelayChannel);
        VeritabaniIslem.ParametreEkle("PulseBaslangic", Kayit.PulseBaslangic);
        VeritabaniIslem.ParametreEkle("PulseBitis", Kayit.PulseBitis);
        Kayit.Id = Convert.ToInt64(VeritabaniIslem.DegerGetir());
        PulseId = Kayit.Id;
        return PulseId > 0;
    }

    public bool Guncelle()
    {
        IslemHazirla(C_Sp_Guncelle);
        VeritabaniIslem.ParametreEkle("Id", Kayit.Id);
        VeritabaniIslem.ParametreEkle("PulseAktif", Kayit.PulseAktif);
        VeritabaniIslem.ParametreEkle("SonrakiDeneme", Kayit.SonrakiDeneme);
        VeritabaniIslem.ParametreEkle("GercekOnZamani", Kayit.GercekOnZamani);
        VeritabaniIslem.ParametreEkle("OffDogrulandi", Kayit.OffDogrulandi);
        VeritabaniIslem.ParametreEkle("ReleaseDenemeSayisi", Kayit.ReleaseDenemeSayisi);
        VeritabaniIslem.ParametreEkle("KritikHata", Kayit.KritikHata);
        VeritabaniIslem.ParametreEkle("SonHata", Kayit.SonHata);
        return VeritabaniIslem.Calistir();
    }

    public bool Doldur()
    {
        IslemHazirla(C_Sp_Doldur);
        VeritabaniIslem.ParametreEkle("Id", PulseId);
        SonucKayit = VeritabaniIslem.SatirGetir();
        Kayit = SonucKayit == null ? null : SatirdanGetir(SonucKayit);
        return Kayit != null;
    }

    public void SuresiDolanlariGetir(bool recovery)
    {
        IslemHazirla(C_Sp_SuresiDolanlariGetir);
        VeritabaniIslem.ParametreEkle("Recovery", recovery);
        VeriTablosu = VeritabaniIslem.TabloGetir();
    }

    public bool AktifKayitVarMi(int? makineId)
    {
        IslemHazirla(C_Sp_AktifKayitVarMi);
        VeritabaniIslem.ParametreEkle("MakineId", makineId);
        return Convert.ToBoolean(VeritabaniIslem.DegerGetir());
    }

    public void UyarilariGetir()
    {
        IslemHazirla(C_Sp_UyarilariGetir);
        VeriTablosu = VeritabaniIslem.TabloGetir();
    }

    public bool CihazKilidiAl(string ip, int port)
    {
        IslemHazirla(C_Sp_CihazKilidiAl);
        VeritabaniIslem.ParametreEkle("ControllerIp", ip);
        VeritabaniIslem.ParametreEkle("HttpPort", port);
        return Convert.ToInt32(VeritabaniIslem.DegerGetir()) >= 0;
    }

    public static RelayPulse SatirdanGetir(DataRow satir)
    {
        return new RelayPulse
        {
            Id = Convert.ToInt64(satir["Id"]), MakineId = Convert.ToInt32(satir["MakineId"]),
            ControllerIp = Convert.ToString(satir["ControllerIp"]), HttpPort = Convert.ToInt32(satir["HttpPort"]),
            RelayChannel = Convert.ToInt32(satir["RelayChannel"]), PulseAktif = Convert.ToBoolean(satir["PulseAktif"]),
            PulseBaslangic = TurkiyeTarihi(satir["PulseBaslangic"]), PulseBitis = TurkiyeTarihi(satir["PulseBitis"]),
            SonrakiDeneme = TurkiyeTarihi(satir["SonrakiDeneme"]),
            GercekOnZamani = satir["GercekOnZamani"] == DBNull.Value ? (DateTime?)null : TurkiyeTarihi(satir["GercekOnZamani"]),
            OffDogrulandi = Convert.ToBoolean(satir["OffDogrulandi"]), ReleaseDenemeSayisi = Convert.ToInt32(satir["ReleaseDenemeSayisi"]),
            KritikHata = Convert.ToBoolean(satir["KritikHata"]), SonHata = satir["SonHata"] as string
        };
    }

    private static DateTime TurkiyeTarihi(object deger)
    {
        return DateTime.SpecifyKind(Convert.ToDateTime(deger), DateTimeKind.Unspecified);
    }

    #endregion
}
