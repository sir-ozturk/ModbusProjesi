using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal static class RelayPulseSqlTests
{
    private static int count;
    private static void Check(bool condition,string name)
    { if(!condition) throw new Exception(name); count++; Console.WriteLine("OK: "+name); }
    private static object Sql(string text,long id)
    {
        using(var c=new SqlConnection(ConfigurationManager.ConnectionStrings["ModbusDb"].ConnectionString))
        using(var cmd=new SqlCommand(text,c)) { cmd.Parameters.AddWithValue("@id",id); c.Open(); return cmd.ExecuteScalar(); }
    }
    private sealed class Io : IRelayPulseIo
    {
        public bool On=true;
        public int Ons;
        public int Failures;
        public Task<MakineRoleIslemleri.KanalDurumSonucu> Set(MakineRoleBaglantilari c,bool on)
        {
            if(on) { Interlocked.Increment(ref Ons); if(Failures>0) Failures--; else On=true; } else On=false;
            return Task.FromResult(new MakineRoleIslemleri.KanalDurumSonucu {Basarili=true});
        }
        public Task<MakineRoleIslemleri.KanalDurumSonucu> Read(MakineRoleBaglantilari c)
        { return Task.FromResult(new MakineRoleIslemleri.KanalDurumSonucu {Basarili=true,DuruyorMu=!On,HamCevap=On ? "1111111111111111" : "0000000000000000"}); }
    }
    private sealed class Scope : IRelayPulseStore
    {
        private readonly RelayPulseStore store;
        private readonly int machine;
        public Scope(RelayPulseStore store,int machine) { this.store=store; this.machine=machine; }
        public IRelayPulseLease TryLock(MakineRoleBaglantilari c) { return store.TryLock(c); }
        public IList<RelayPulse> GetDue(bool recovery) { return store.GetDue(recovery).Where(p=>p.MakineId==machine).ToList(); }
        public bool HasActive(int id) { return store.HasActive(id); }
        public string GetWarnings() { return store.GetWarnings(); }
    }
    private static async Task Run()
    {
        var builder=new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ModbusDb"].ConnectionString);
        if(!builder.InitialCatalog.StartsWith("DB_MODBUS_DonanimTest_Pulse_")) throw new Exception("Test database required");
        var store=new RelayPulseStore(); var io=new Io();
        int machine=new Random().Next(100000,2000000000);
        var service=new RelayPulseService(new Scope(store,machine),io,()=>RelayPulseClock.Now);
        var c=new MakineRoleBaglantilari(null) {MakineId=machine,Ip="127.0.0.1",HttpPort=new Random().Next(10000,65000),KanalNo=1,AktifMi=true};
        Check(await service.TriggerStopPulse(c)==null,"SQL-backed OFF trigger succeeds");
        long id=Convert.ToInt64(Sql("SELECT Id FROM dbo.RelayPulses WHERE MakineId=@id",machine));
        using(var held=store.TryLock(c))
        {
            var p=held.Get(id);
            Check(p.PulseAktif && p.OffDogrulandi,"Pulse and verified OFF survive a new SQL connection");
            Check(p.PulseBitis-p.PulseBaslangic==TimeSpan.FromSeconds(10),"SQL precision preserves exact ten second plan");
            using(var competing=new RelayPulseStore().TryLock(c)) Check(competing==null,"Second SQL session cannot acquire channel lock");
        }
        using(var next=store.TryLock(c)) Check(next!=null,"Disposal releases SQL session lock");
        Check(await service.TriggerStopPulse(c)!=null,"SQL duplicate request rejected");
        foreach(bool sameMachine in new[]{true,false})
        {
            var duplicate=new RelayPulse {MakineId=sameMachine ? machine : machine+1,ControllerIp=c.Ip,HttpPort=c.HttpPort,
                RelayChannel=sameMachine ? 2 : 1,PulseAktif=true,PulseBaslangic=RelayPulseClock.Now};
            duplicate.PulseBitis=duplicate.PulseBaslangic.AddSeconds(10);
            using(var lease=store.TryLock(duplicate.Baglanti()))
            {
                bool rejected=false;
                try { lease.Create(duplicate); } catch(SqlException ex) { rejected=ex.Number==2601 || ex.Number==2627; }
                Check(rejected,sameMachine ? "Unique active machine constraint" : "Unique active physical channel constraint");
            }
        }
        bool badDeadline=false;
        try { Sql("UPDATE dbo.RelayPulses SET PulseBitis=DATEADD(second,11,PulseBaslangic) WHERE Id=@id",id); }
        catch(SqlException ex) { badDeadline=ex.Number==547; }
        Check(badDeadline,"Database rejects a pulse longer than ten seconds");

        Sql("UPDATE dbo.RelayPulses SET PulseBaslangic=DATEADD(second,-20,CONVERT(datetime2(3),SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time')),PulseBitis=DATEADD(second,-10,CONVERT(datetime2(3),SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time')),SonrakiDeneme=CONVERT(datetime2(3),SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time') WHERE Id=@id",id);
        var restarted=new RelayPulseService(new Scope(new RelayPulseStore(),machine),io,()=>RelayPulseClock.Now);
        await Task.WhenAll(restarted.RecoverPendingPulses(),service.ReleaseExpiredPulses());
        using(var lease=store.TryLock(c)) Check(!lease.Get(id).PulseAktif && lease.Get(id).GercekOnZamani.HasValue,"Restart recovery persists verified ON and completion");
        Check(io.Ons==1,"Concurrent SQL watchdogs send one release");
        Check(Convert.ToInt32(Sql("SELECT COUNT(*) FROM dbo.RelayPulseAudit WHERE PulseId=@id AND HamCevap IS NOT NULL",id))>=2,"Audit view retains /98 replies and address snapshot");

        io.Failures=4;
        Check(await service.TriggerStopPulse(c)==null,"New pulse permitted after verified completion");
        id=Convert.ToInt64(Sql("SELECT MAX(Id) FROM dbo.RelayPulses WHERE MakineId=@id",machine));
        Sql("UPDATE dbo.RelayPulses SET PulseBaslangic=DATEADD(second,-20,CONVERT(datetime2(3),SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time')),PulseBitis=DATEADD(second,-10,CONVERT(datetime2(3),SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time')),SonrakiDeneme=CONVERT(datetime2(3),SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time') WHERE Id=@id",id);
        for(int i=1;i<=4;i++)
        {
            await service.ReleaseExpiredPulses();
            using(var lease=store.TryLock(c))
            {
                var p=lease.Get(id);
                Check(p.PulseAktif && p.ReleaseDenemeSayisi==i,"SQL retry counter persists "+i);
            }
            // Hızlandırılmış testte SQL ve .NET saat hassasiyeti farkının denemeyi geciktirmesi önlenir.
            Sql("UPDATE dbo.RelayPulses SET SonrakiDeneme=DATEADD(second,-1,CONVERT(datetime2(3),SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time')) WHERE Id=@id",id);
        }
        Check(store.GetWarnings().Contains("KRİTİK"),"Critical failure visible through dashboard warning query");
        await service.ReleaseExpiredPulses();
        Check(!store.HasActive(machine),"Watchdog rescue completes exhausted SQL pulse");
    }
    private static int Main()
    {
        try { Run().GetAwaiter().GetResult(); Console.WriteLine(count+" SQL pulse checks passed."); return 0; }
        catch(Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
