using System;
using System.Collections.Generic;
using System.Threading.Tasks;

internal static class RelayPulseTests
{
    private static int checks;
    private static void Check(bool condition, string message)
    { if (!condition) throw new Exception(message); checks++; }
    private static MakineRoleBaglantilari Connection()
    { return new MakineRoleBaglantilari(null) { MakineId=1, Ip="127.0.0.1", HttpPort=8080, KanalNo=1, AktifMi=true }; }
    private sealed class FakeIo
    {
        public bool Off=true, RejectOn, UnknownOn, ThrowOn, LostOnReply, LostOffReply, ThrowOff, ThrowRead;
        public int OffFailures, OnCommands, OffCommands, Reads;
        public Task<MakineRoleIslemleri.KanalDurumSonucu> Set(MakineRoleBaglantilari c, bool on)
        {
            if (!on)
            {
                OffCommands++;
                if (ThrowOff) throw new Exception("OFF exception");
                if (OffCommands > OffFailures) Off=true;
            }
            else
            {
                OnCommands++;
                if (ThrowOn) throw new Exception("ON exception");
                if (!RejectOn) Off=false;
            }
            return Task.FromResult(new MakineRoleIslemleri.KanalDurumSonucu { Basarili=!(on ? LostOnReply : LostOffReply) });
        }
        public Task<MakineRoleIslemleri.KanalDurumSonucu> Read(MakineRoleBaglantilari c)
        {
            Reads++;
            if (ThrowRead) throw new Exception("Read exception");
            return Task.FromResult(new MakineRoleIslemleri.KanalDurumSonucu { Basarili=!(UnknownOn && Reads==1), DuruyorMu=!Off });
        }
    }
    private static RelayPulseService Service(FakeIo io, List<int> delays)
    { return new RelayPulseService(io.Set, io.Read, ms => { delays.Add(ms); return Task.FromResult(0); }); }
    private static async Task Run()
    {
        var io=new FakeIo(); var delays=new List<int>();
        var result=await Service(io,delays).TriggerStopPulse(Connection());
        Check(result.OnDogrulandi && result.Hata==null && io.Off,"Normal pulse completes");
        Check(io.OnCommands==1 && io.OffCommands==1 && io.Reads==2,"Both commands verified");
        Check(delays.Count==1 && delays[0]==3000,"Three second delay after ON");

        io=new FakeIo { RejectOn=true }; delays=new List<int>();
        result=await Service(io,delays).TriggerStopPulse(Connection());
        Check(!result.OnDogrulandi && result.Hata!=null && io.OffCommands==0 && delays.Count==0,"Known OFF skips return command and wait");

        io=new FakeIo { UnknownOn=true }; delays=new List<int>();
        result=await Service(io,delays).TriggerStopPulse(Connection());
        Check(!result.OnDogrulandi && result.Hata!=null && io.OffCommands==1 && delays.Count==0,"Unknown ON immediately returns OFF");

        io=new FakeIo { LostOnReply=true, LostOffReply=true }; delays=new List<int>();
        result=await Service(io,delays).TriggerStopPulse(Connection());
        Check(result.OnDogrulandi && result.Hata==null,"Lost command replies resolved by status");

        io=new FakeIo { OffFailures=2 }; delays=new List<int>();
        result=await Service(io,delays).TriggerStopPulse(Connection());
        Check(result.Hata==null && io.OffCommands==3,"Third OFF attempt succeeds");
        Check(delays.Count==3 && delays[1]==500 && delays[2]==500,"Short retry intervals");

        io=new FakeIo { OffFailures=10 }; delays=new List<int>();
        result=await Service(io,delays).TriggerStopPulse(Connection());
        Check(result.OnDogrulandi && result.Hata!=null && io.OffCommands==3,"Failed release preserves stop confirmation and stops at three");

        io=new FakeIo { ThrowOn=true }; delays=new List<int>();
        result=await Service(io,delays).TriggerStopPulse(Connection());
        Check(!result.OnDogrulandi && result.Hata!=null && io.OffCommands==1,"ON exception still attempts OFF");

        io=new FakeIo { ThrowOff=true, ThrowRead=true }; delays=new List<int>();
        result=await Service(io,delays).TriggerStopPulse(Connection());
        Check(result.Hata!=null && io.OffCommands==3,"Exceptions remain bounded to three attempts");
        io=new FakeIo();
        Check((await Service(io,new List<int>()).TriggerStopPulse(Connection())).Hata==null,"Lock released after exceptions");

        io=new FakeIo();
        var blocked=new TaskCompletionSource<bool>();
        var svc=new RelayPulseService(io.Set, io.Read,ms => blocked.Task);
        var first=svc.TriggerStopPulse(Connection());
        Check(io.OnCommands==1 && io.OffCommands==0,"Wait precedes OFF");
        var other=Connection(); other.MakineId=2; other.KanalNo=2;
        result=await Service(new FakeIo(),new List<int>()).TriggerStopPulse(other);
        Check(result.Hata!=null && !result.OnDogrulandi,"Same controller cannot overlap across channels or service instances");
        blocked.SetResult(true);
        Check((await first).Hata==null && io.OffCommands==1,"In-flight pulse finishes normally");

        io=new FakeIo();
        svc=new RelayPulseService(io.Set, io.Read,ms => { throw new Exception("Delay interrupted"); });
        result=await svc.TriggerStopPulse(Connection());
        Check(result.OnDogrulandi && result.Hata!=null && io.OffCommands==1,"Wait failure still returns OFF");

        io=new FakeIo(); var invalid=Connection(); invalid.AktifMi=false;
        result=await Service(io,new List<int>()).TriggerStopPulse(invalid);
        Check(result.Hata!=null && io.OnCommands==0 && io.OffCommands==0,"Invalid connection sends no commands");

        io=new FakeIo();
        svc=new RelayPulseService(io.Set, io.Read,ms => { io.Off=true; return Task.FromResult(0); });
        result=await svc.TriggerStopPulse(Connection());
        Check(result.Hata==null,"Device automatic OFF is accepted");
    }
    private static int Main()
    {
        try { Run().GetAwaiter().GetResult(); Console.WriteLine(checks+" pulse checks passed."); return 0; }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
