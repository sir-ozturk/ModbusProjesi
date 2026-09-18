using System.Threading.Tasks;

public interface IRelayPulseIo
{
    Task<MakineRoleIslemleri.KanalDurumSonucu> Set(MakineRoleBaglantilari connection, bool on);
    Task<MakineRoleIslemleri.KanalDurumSonucu> Read(MakineRoleBaglantilari connection);
}

