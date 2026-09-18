using System;
using System.Collections.Generic;

public interface IRelayPulseLease : IDisposable
{
    RelayPulse Get(long id);
    void Create(RelayPulse pulse);
    void Save(RelayPulse pulse);
    void Log(RelayPulse pulse, string command, string response, bool success, string error);
}

public interface IRelayPulseStore
{
    IRelayPulseLease TryLock(MakineRoleBaglantilari connection);
    IList<RelayPulse> GetDue(bool recovery);
    bool HasActive(int machineId);
    string GetWarnings();
}

