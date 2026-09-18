using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal static class RelayPulseTests
{
    private static int checks;
    private static void Check(bool value, string label)
    { if (!value) throw new Exception(label); checks++; Console.WriteLine("OK: " + label); }

    private sealed class MemoryStore : IRelayPulseStore
    {
        public DateTime Now = new DateTime(2026,9,18,0,0,0,DateTimeKind.Utc);
        public readonly List<RelayPulse> Rows = new List<RelayPulse>();
        public readonly List<string> Events = new List<string>();
        private readonly HashSet<string> locks = new HashSet<string>();
        public bool FailCreate;
        public bool FailSave;
        private static RelayPulse Copy(RelayPulse p)
        {
            return new RelayPulse { Id=p.Id,MakineId=p.MakineId,ControllerIp=p.ControllerIp,HttpPort=p.HttpPort,
                RelayChannel=p.RelayChannel,PulseAktif=p.PulseAktif,PulseBaslangic=p.PulseBaslangic,PulseBitis=p.PulseBitis,
                SonrakiDeneme=p.SonrakiDeneme,GercekOnZamani=p.GercekOnZamani,OffDogrulandi=p.OffDogrulandi,
                ReleaseDenemeSayisi=p.ReleaseDenemeSayisi,KritikHata=p.KritikHata,SonHata=p.SonHata };
        }
        public IRelayPulseLease TryLock(MakineRoleBaglantilari c)
        {
            string key=c.Ip+":"+c.HttpPort;
            lock(locks) { if (!locks.Add(key)) return null; return new Lease(this,key); }
        }
        public IList<RelayPulse> GetDue(bool recovery)
        {
            lock(Rows) return Rows.Where(p=>p.PulseAktif && (p.PulseBitis<=Now || p.ReleaseDenemeSayisi>0)
                && (recovery || p.SonrakiDeneme<=Now || (!p.KritikHata && Now>=p.PulseBaslangic.AddSeconds(12)))).Select(Copy).ToList();
        }
        public bool HasActive(int id) { lock(Rows) return Rows.Any(p=>p.PulseAktif && p.MakineId==id); }
        public string GetWarnings() { return ""; }
        private sealed class Lease : IRelayPulseLease
        {
            private readonly MemoryStore db;
            private readonly string key;
            public Lease(MemoryStore db,string key) { this.db=db; this.key=key; }
            public RelayPulse Get(long id) { lock(db.Rows) return db.Rows.Where(p=>p.Id==id).Select(Copy).SingleOrDefault(); }
            public void Create(RelayPulse p)
            {
                if(db.FailCreate) throw new InvalidOperationException("DB unavailable");
                lock(db.Rows)
                {
                    if(db.Rows.Any(x=>x.PulseAktif && (x.MakineId==p.MakineId ||
                        (x.ControllerIp==p.ControllerIp && x.HttpPort==p.HttpPort && x.RelayChannel==p.RelayChannel))))
                        throw new InvalidOperationException("Duplicate pulse");
                    p.Id=db.Rows.Count+1; db.Rows.Add(Copy(p));
                }
            }
            public void Save(RelayPulse p)
            {
                if(db.FailSave) throw new InvalidOperationException("Save unavailable");
                lock(db.Rows) db.Rows[db.Rows.FindIndex(x=>x.Id==p.Id)]=Copy(p);
            }
            public void Log(RelayPulse p,string command,string response,bool success,string error)
            { lock(db.Events) db.Events.Add(command+" "+success+" "+response+" "+error); }
            public void Dispose() { lock(db.locks) db.locks.Remove(key); }
        }
    }

    private sealed class FakeIo : IRelayPulseIo
    {
        public bool On = true;
        public bool BadRead;
        public int OnFailures;
        public int OffCommands, OnCommands;
        public bool TimeoutOff;
        public TaskCompletionSource<bool> HoldOff;
        public Action BeforeOff;
        public async Task<MakineRoleIslemleri.KanalDurumSonucu> Set(MakineRoleBaglantilari c,bool on)
        {
            if(!on)
            {
                if(BeforeOff!=null) BeforeOff();
                Interlocked.Increment(ref OffCommands);
                if(HoldOff!=null) await HoldOff.Task;
                On=false;
            }
            else
            {
                Interlocked.Increment(ref OnCommands);
                if(OnFailures>0) OnFailures--; else On=true;
            }
            return new MakineRoleIslemleri.KanalDurumSonucu { Basarili=!(TimeoutOff && !on),
                Hata=(TimeoutOff && !on) ? "timeout" : null, HataDetayi=(TimeoutOff && !on) ? "TaskCanceledException" : null };
        }
        public Task<MakineRoleIslemleri.KanalDurumSonucu> Read(MakineRoleBaglantilari c)
        {
            return Task.FromResult(new MakineRoleIslemleri.KanalDurumSonucu { Basarili=!BadRead,
                DuruyorMu=!On,HamCevap=BadRead ? "bad" : (On ? "1111111111111111" : "0000000000000000"),
                Hata=BadRead ? "invalid /98" : null });
        }
    }

    private static MakineRoleBaglantilari Connection()
    { return new MakineRoleBaglantilari(null) {MakineId=1,Ip="127.0.0.1",HttpPort=8080,KanalNo=1,AktifMi=true}; }
    private static RelayPulseService Service(MemoryStore db,FakeIo io)
    { return new RelayPulseService(db,io,()=>db.Now); }

    private static async Task Run()
    {
        DateTime turkiyeSimdi = RelayPulseClock.Now;
        Check(Math.Abs((turkiyeSimdi - DateTime.UtcNow).TotalHours - 3) < 0.01, "Türkiye saati UTC+3");
        Check(RelayPulseService.UserError("Zaman aşımı. System.Threading.Tasks.TaskCanceledException: stack") == "Zaman aşımı.", "Old stack traces hidden from dashboard");
        var db=new MemoryStore(); var io=new FakeIo(); var svc=Service(db,io);
        io.BeforeOff=()=>Check(db.Rows.Count==1 && db.Rows[0].PulseAktif,"Pulse durable before OFF");
        Check(await svc.TriggerStopPulse(Connection())==null,"OFF verified");
        Check(db.Rows[0].PulseBitis-db.Rows[0].PulseBaslangic==TimeSpan.FromSeconds(10),"10 second deadline");
        Check(await svc.TriggerStopPulse(Connection())!=null && io.OffCommands==1,"Duplicate does not send OFF or extend deadline");
        db.Now=db.Now.AddSeconds(9); await svc.ReleaseExpiredPulses();
        Check(io.OnCommands==0,"No early normal release");
        db.Now=db.Now.AddSeconds(1); await svc.ReleaseExpiredPulses();
        Check(io.OnCommands==1 && !db.Rows[0].PulseAktif && db.Rows[0].GercekOnZamani==db.Now,"ON verified at deadline");

        db=new MemoryStore(); io=new FakeIo(); svc=Service(db,io);
        await svc.TriggerStopPulse(Connection());
        var restarted=Service(db,io);
        await restarted.RecoverPendingPulses();
        Check(io.OnCommands==0,"Recovery preserves unexpired deadline");
        db.Now=db.Now.AddSeconds(11); await restarted.RecoverPendingPulses();
        Check(!db.Rows[0].PulseAktif && io.OnCommands==1,"New service recovers durable pulse");

        db=new MemoryStore(); io=new FakeIo {OnFailures=20}; svc=Service(db,io);
        await svc.TriggerStopPulse(Connection()); db.Now=db.Now.AddSeconds(10);
        for(int attempt=1;attempt<=4;attempt++)
        {
            await svc.ReleaseExpiredPulses();
            Check(db.Rows[0].ReleaseDenemeSayisi==attempt && db.Rows[0].PulseAktif,"Failed ON stays active attempt "+attempt);
            int delay=attempt<4 ? attempt : 5;
            Check(db.Rows[0].SonrakiDeneme==db.Now.AddSeconds(delay),"Persisted retry delay "+delay);
            await svc.ReleaseExpiredPulses();
            Check(io.OnCommands==attempt,"No retry before scheduled time");
            db.Now=db.Now.AddSeconds(delay);
        }
        Check(db.Rows[0].KritikHata && db.Rows[0].SonHata.Contains("KRİTİK"),"Exhaustion records critical error");
        io.OnFailures=0; await svc.ReleaseExpiredPulses();
        Check(io.OnCommands==5 && !db.Rows[0].PulseAktif,"Independent watchdog rescues after retry exhaustion");

        db=new MemoryStore(); io=new FakeIo(); svc=Service(db,io);
        await svc.TriggerStopPulse(Connection()); db.Now=db.Now.AddSeconds(13); io.OnFailures=1;
        await svc.ReleaseExpiredPulses();
        Check(db.Rows[0].KritikHata && db.Rows[0].SonHata.Contains("12 saniye"),"12 second abnormal OFF detected");

        db=new MemoryStore(); io=new FakeIo {OnFailures=10}; svc=Service(db,io);
        await svc.TriggerStopPulse(Connection()); db.Now=db.Now.AddSeconds(10); await svc.ReleaseExpiredPulses();
        db.Now=db.Now.AddSeconds(1); await svc.ReleaseExpiredPulses();
        Check(db.Rows[0].SonrakiDeneme==db.Now.AddSeconds(2),"Second retry scheduled two seconds later");
        db.Now=db.Now.AddSeconds(1); await svc.ReleaseExpiredPulses();
        Check(io.OnCommands==3 && db.Rows[0].KritikHata,"12 second watchdog bypasses pending retry wait");

        db=new MemoryStore(); io=new FakeIo {BadRead=true}; svc=Service(db,io);
        Check(await svc.TriggerStopPulse(Connection())!=null,"Malformed /98 never counts as success");
        Check(io.OnCommands==1 && db.Rows[0].PulseAktif,"Unknown OFF prompts immediate ON, remains pending without verification");
        io.BadRead=false; db.Now=db.Now.AddSeconds(1); await svc.ReleaseExpiredPulses();
        Check(!db.Rows[0].PulseAktif,"Early emergency retry remains recoverable before original deadline");

        db=new MemoryStore(); io=new FakeIo {TimeoutOff=true}; svc=Service(db,io);
        Check(await svc.TriggerStopPulse(Connection())==null && db.Rows[0].OffDogrulandi,"Lost command response resolved by actual IO verification");
        Check(db.Events.Any(e=>e.Contains("TaskCanceledException")),"Timeout detail audited");

        db=new MemoryStore {FailCreate=true}; io=new FakeIo(); svc=Service(db,io);
        try { await svc.TriggerStopPulse(Connection()); } catch(InvalidOperationException) { }
        Check(io.OffCommands==0,"No OFF when durable insert fails");

        db=new MemoryStore(); io=new FakeIo {HoldOff=new TaskCompletionSource<bool>()}; svc=Service(db,io);
        Task<string> first=svc.TriggerStopPulse(Connection());
        var secondChannel=Connection(); secondChannel.MakineId=2; secondChannel.KanalNo=2;
        Check(await Service(db,io).TriggerStopPulse(secondChannel)!=null && io.OffCommands==1,"Different channels cannot overlap on one controller");
        Check(await Service(db,io).TriggerStopPulse(Connection())!=null,"Cross-worker duplicate blocked while OFF is in flight");
        db.Now=db.Now.AddSeconds(10); await Service(db,io).ReleaseExpiredPulses();
        Check(io.OnCommands==0,"Release cannot overtake an in-flight OFF");
        io.HoldOff.SetResult(true); await first;
        await Task.WhenAll(svc.ReleaseExpiredPulses(),Service(db,io).ReleaseExpiredPulses());
        Check(io.OnCommands==1 && !db.Rows[0].PulseAktif,"Concurrent watchdogs send only one verified release");

        db=new MemoryStore(); io=new FakeIo(); svc=Service(db,io);
        io.BeforeOff=()=>db.FailSave=true;
        try { await svc.TriggerStopPulse(Connection()); } catch(InvalidOperationException) { }
        Check(io.OnCommands==1 && db.Rows[0].PulseAktif,"Post-OFF storage failure attempts emergency ON without losing pending row");
        db.FailSave=false; db.Now=db.Now.AddSeconds(10); await Service(db,io).RecoverPendingPulses();
        Check(!db.Rows[0].PulseAktif,"Recovery reconciles an emergency ON");
    }

    private static int Main()
    {
        try { Run().GetAwaiter().GetResult(); Console.WriteLine(checks+" pulse checks passed."); return 0; }
        catch(Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
