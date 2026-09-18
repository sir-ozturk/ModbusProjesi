public class RelayPulseEvents : OrtakAlanlar
{
    public RelayPulseEvents(VeritabaniIslemleri veritabaniIslemleri)
    {
        VeritabaniIslem = veritabaniIslemleri;
    }

    #region SABİTLER

    public const string C_Tablo = "dbo.RelayPulseEvents";
    public const string C_Sp_Ekle = "dbo.SP_RelayPulseEvents_EKLE";

    #endregion

    #region NESNELER


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

    public string komut;
    public string Komut
    {
        get
        {
            return komut;
        }
        set
        {
            komut = value;
        }
    }
    public string hamCevap;
    public string HamCevap
    {
        get
        {
            return hamCevap;
        }
        set
        {
            hamCevap = value;
        }
    }
    public bool basarili;
    public bool Basarili
    {
        get
        {
            return basarili;
        }
        set
        {
            basarili = value;
        }
    }
    public string hata;
    public string Hata
    {
        get
        {
            return hata;
        }
        set
        {
            hata = value;
        }
    }
    public int releaseDenemeSayisi;
    public int ReleaseDenemeSayisi
    {
        get
        {
            return releaseDenemeSayisi;
        }
        set
        {
            releaseDenemeSayisi = value;
        }
    }
    public bool kritikHata;
    public bool KritikHata
    {
        get
        {
            return kritikHata;
        }
        set
        {
            kritikHata = value;
        }
    }

    #endregion

    #region METOTLAR

    public bool Ekle()
    {
        VeritabaniIslem.ParametreleriSil();
        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle("PulseId", PulseId);
        VeritabaniIslem.ParametreEkle("Komut", Komut);
        VeritabaniIslem.ParametreEkle("HamCevap", HamCevap);
        VeritabaniIslem.ParametreEkle("Basarili", Basarili);
        VeritabaniIslem.ParametreEkle("Hata", Hata);
        VeritabaniIslem.ParametreEkle("ReleaseDenemeSayisi", ReleaseDenemeSayisi);
        VeritabaniIslem.ParametreEkle("KritikHata", KritikHata);
        return VeritabaniIslem.Calistir();
    }

    #endregion
}
