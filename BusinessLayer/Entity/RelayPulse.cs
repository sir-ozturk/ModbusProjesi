using System;

public sealed class RelayPulse
{
    public long Id { get; set; }
    public int MakineId { get; set; }
    public string ControllerIp { get; set; }
    public int HttpPort { get; set; }
    public int RelayChannel { get; set; }
    public bool PulseAktif { get; set; }
    public DateTime PulseBaslangic { get; set; }
    public DateTime PulseBitis { get; set; }
    public DateTime SonrakiDeneme { get; set; }
    public DateTime? GercekOnZamani { get; set; }
    public bool OffDogrulandi { get; set; }
    public int ReleaseDenemeSayisi { get; set; }
    public bool KritikHata { get; set; }
    public string SonHata { get; set; }

    public MakineRoleBaglantilari Baglanti()
    {
        return new MakineRoleBaglantilari(null) { MakineId = MakineId, Ip = ControllerIp,
            HttpPort = HttpPort, KanalNo = RelayChannel, AktifMi = true };
    }
}

