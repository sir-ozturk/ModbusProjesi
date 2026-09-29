using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using MakineDurdurmaUygulamasi;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

internal static class TalimatIsleyiciTests
{
    private static int checks;
    private static void Check(bool ok, string text)
    { if (!ok) throw new Exception(text); checks++; }

    private static MakineDurdurmaTalimatlari Job()
    { return new MakineDurdurmaTalimatlari { Id=1, MakineId=1, Url="http://127.0.0.1:8080/01", IslemDurumu=TalimatDurumu.Isleniyor }; }
    private static MakineRoleBaglantilari Connection()
    { return new MakineRoleBaglantilari(null) { MakineId=1, Ip="127.0.0.1", HttpPort=8080, KanalNo=1, AktifMi=true }; }

    private sealed class Store : VeritabaniIslemleri
    {
        public readonly CancellationTokenSource Stop = new CancellationTokenSource();
        public readonly List<MakineDurdurmaTalimatlari> Pending = new List<MakineDurdurmaTalimatlari>();
        public readonly List<MakineDurdurmaTalimatlari> Recovery = new List<MakineDurdurmaTalimatlari>();
        public MakineRoleBaglantilari Link = Connection();
        public string Validation;
        public bool FailSave, FailClaim, Busy, Disposed, On, Off;
        public int Claimed, Saves, Released;
        public TalimatDurumu SavedState;
        private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();
        public readonly List<string> Locks = new List<string>();
        public bool FailLockRelease, FailGlobalLock;
        public bool EmptySave;

        public override void ParametreEkle(string name, object value) { parameters.Add(name, value); }
        public override void ParametreleriSil() { parameters.Clear(); }
        public override void OturumKilidiAl(string name, bool shared)
        {
            if ((FailGlobalLock && name == "ModbusTalimatIsleyici") || (Busy && name.StartsWith("ModbusRoleCihaz:")))
                throw new DonanimIslemHatasi("Busy");
            Check(shared == (name == "ModbusDonanimAyar"), "Lock mode preserved");
            Locks.Add(name);
        }
        public override void OturumKilidiniBirak(string name)
        {
            if (FailLockRelease) throw new Exception("Lock release failed");
            Check(Locks[Locks.Count-1] == name, "Command locks released in reverse order");
            Locks.RemoveAt(Locks.Count-1);
            if (name == "ModbusDonanimAyar") Released++;
        }
        public override object DegerGetir()
        {
            Check((int)parameters["id"] == 1 && (int)parameters["makine_id"] == 1
                && (int)parameters["gecerlilik_saniye"] == 60, "Pre-command parameters retained");
            Check(SpAdi == "dbo.SP_MakineDurdurmaTalimatlari_KOMUT_ONCESI_KONTROL", "Pre-command stored procedure selected");
            return Validation == null ? (object)DBNull.Value : Validation;
        }
        public override DataTable TabloGetir()
        {
            try
            {
                if (SpAdi == "dbo.SP_MakineDurdurmaTalimatlari_YARIM_KALANLARI_GETIR")
                    return Rows(Recovery);
                if (SpAdi == MakineRoleBaglantilari.C_Sp_KomutGetir)
                {
                    Check((int)parameters["makine_id"] == 1, "Machine link requested by id");
                    DataTable table = new DataTable();
                    foreach (string name in new[] { "id", "kanal_no", "http_port", "ethernet_kart_id", "role_kart_id" })
                        table.Columns.Add(name, typeof(int));
                    table.Columns.Add("ip", typeof(string));
                    if (Link != null) table.Rows.Add(Link.Id, Link.KanalNo, Link.HttpPort, Link.EthernetKartId, Link.RoleKartId, Link.Ip);
                    return table;
                }
                Check((int)parameters["guncelleyen_id"] == 1 && (string)parameters["guncelleyen_ip"] == "127.0.0.1",
                    "Worker audit identity retained");
                if (SpAdi == "dbo.SP_MakineDurdurmaTalimatlari_SIRADAKINI_AL")
                {
                    Check((int)parameters["gecerlilik_saniye"] == 60, "Claim expiry retained");
                    Claimed++;
                    if (FailClaim) throw new Exception("Claim reply lost");
                    if (Pending.Count == 0) { Stop.Cancel(); return Rows(new List<MakineDurdurmaTalimatlari>()); }
                    var result = Pending[0]; Pending.RemoveAt(0); return Rows(new[] { result });
                }
                if (SpAdi == "dbo.SP_MakineDurdurmaTalimatlari_SONUCLANDIR")
                {
                    Saves++; SavedState = (TalimatDurumu)(byte)parameters["islem_durumu"];
                    On = (bool)parameters["on_dogrulandi"]; Off = (bool)parameters["off_dogrulandi"];
                    Check((int)parameters["id"] == 1 && !string.IsNullOrWhiteSpace((string)parameters["sonuc"]),
                        "Finalization includes id and result");
                    if (FailSave) throw new Exception("Save reply lost");
                    Stop.Cancel();
                    var job = Job(); job.IslemDurumu = SavedState;
                    return Rows(EmptySave ? new MakineDurdurmaTalimatlari[0] : new[] { job });
                }
                throw new Exception("Unexpected procedure: " + SpAdi);
            }
            finally { ParametreleriSil(); }
        }
        public override bool Bitir()
        {
            Check(parameters.Count == 0, "Parameters cleared even after failure");
            Locks.Clear(); Disposed = true; return true;
        }
        private static DataTable Rows(IEnumerable<MakineDurdurmaTalimatlari> jobs)
        {
            var table = new DataTable();
            foreach (string name in new[] { "id", "makine_id", "ekleyen_id", "guncelleyen_id" }) table.Columns.Add(name, typeof(int));
            foreach (string name in new[] { "url", "islem_nedeni", "sonuc", "ekleyen_ip", "guncelleyen_ip" }) table.Columns.Add(name, typeof(string));
            foreach (string name in new[] { "islem_baslangic_tarih", "islem_bitis_tarih", "eklenme_tarih", "guncellenme_tarih" }) table.Columns.Add(name, typeof(DateTime));
            table.Columns.Add("islem_durumu", typeof(byte)); table.Columns.Add("aktif_mi", typeof(bool));
            foreach (var job in jobs)
            {
                var row = table.NewRow();
                row["id"] = job.Id; row["makine_id"] = job.MakineId; row["url"] = job.Url;
                row["islem_durumu"] = (byte)job.IslemDurumu; row["ekleyen_id"] = 1;
                row["eklenme_tarih"] = DateTime.Now; row["aktif_mi"] = true;
                table.Rows.Add(row);
            }
            return table;
        }
    }

    private sealed class Io
    {
        public int OnCount, OffCount, Reads;
        public bool State, FailOff, UnknownOn;
        public Func<int,Task> Delay = ms => Task.FromResult(0);
        public RelayPulseService Service()
        {
            return new RelayPulseService((c,on) =>
            {
                if (on) { OnCount++; State=true; }
                else { OffCount++; if (!FailOff) State=false; }
                return Task.FromResult(new MakineRoleIslemleri.KanalDurumSonucu { Basarili=true });
            }, c =>
            {
                Reads++;
                return Task.FromResult(new MakineRoleIslemleri.KanalDurumSonucu
                { Basarili=!(UnknownOn && Reads==1), DuruyorMu=State });
            }, ms => Delay(ms));
        }
    }

    private static async Task Execute(Store store, Io io, bool expectError)
    {
        bool error=false;
        var worker=new TalimatIsleyici(() => store, 1, "127.0.0.1", 60, io.Service());
        try { await worker.CalistirAsync(store.Stop.Token); }
        catch (Exception) { error=true; }
        Check(error==expectError,"Unexpected completion/error");
        Check(store.Disposed && !worker.Calisiyor,"Session and running flag released");
    }

    private static async Task Run()
    {
        // Bağlantı açmadan SQL hatası oluştur: sonraki prosedüre sorgu tipi veya
        // eski parametre taşınmadığını gerçek yardımcı sınıf üzerinde doğrula.
        var database = new VeritabaniIslemleri();
        using (var command = new SqlCommand())
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id", 1);
            typeof(VeritabaniIslemleri).GetField("sqlCommand", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(database, command);
            bool queryFailed = false;
            try { database.SpAdi = "dbo.SP_MakineDurdurmaTalimatlari_KOMUT_ONCESI_KONTROL"; database.DegerGetir(); }
            catch (InvalidOperationException) { queryFailed = true; }
            Check(queryFailed, "SQL failure reaches caller");
            Check(command.CommandType == CommandType.StoredProcedure && command.Parameters.Count == 0,
                "SQL failure restores procedure mode and clears parameters");
        }

        var store=new Store(); var io=new Io(); store.Pending.Add(Job());
        await Execute(store,io,false);
        Check(store.SavedState==TalimatDurumu.Tamamlandi && store.On && store.Off,"Success requires both confirmations");
        Check(io.OnCount==1 && io.OffCount==1 && store.Saves==1 && store.Released==1,"Exactly one pulse and save");

        store=new Store(); io=new Io(); var bad=Job(); bad.Url="http://127.0.0.2:8080/01"; store.Pending.Add(bad);
        await Execute(store,io,false);
        Check(store.SavedState==TalimatDurumu.Hatali && io.OnCount==0 && io.OffCount==0,"Changed address never contacted");

        store=new Store { Validation="Expired" }; io=new Io(); store.Pending.Add(Job());
        await Execute(store,io,false);
        Check(io.OnCount==0 && store.SavedState==TalimatDurumu.Hatali,"Expired job sends no command");

        store=new Store { Link=null }; io=new Io(); store.Pending.Add(Job());
        await Execute(store,io,false);
        Check(io.OnCount==0 && store.SavedState==TalimatDurumu.Hatali,"Missing link sends no command");

        store=new Store { Busy=true }; io=new Io(); store.Pending.Add(Job());
        await Execute(store,io,false);
        Check(io.OnCount==0 && store.SavedState==TalimatDurumu.Hatali,"Busy device sends no command");

        store=new Store(); io=new Io { FailOff=true }; store.Pending.Add(Job()); store.Pending.Add(Job());
        await Execute(store,io,true);
        Check(store.On && !store.Off && store.SavedState==TalimatDurumu.KontrolGerekli,"Partial success preserves ON");
        Check(io.OffCount==3 && store.Claimed==1 && store.Pending.Count==1,"OFF bounded; next job not claimed");

        store=new Store(); io=new Io { UnknownOn=true }; store.Pending.Add(Job());
        await Execute(store,io,true);
        Check(!store.On && store.Off && store.SavedState==TalimatDurumu.KontrolGerekli,"Unknown ON is not ordinary failure");

        store=new Store { FailSave=true }; io=new Io(); store.Pending.Add(Job()); store.Pending.Add(Job());
        await Execute(store,io,true);
        Check(store.Saves==1 && store.Claimed==1 && io.OnCount==1 && io.OffCount==1,"Save failure never retries pulse or takes next job");

        store=new Store { FailClaim=true }; io=new Io();
        await Execute(store,io,true);
        Check(store.Claimed==1 && io.OnCount==0,"Ambiguous claim is not retried");

        store=new Store(); io=new Io(); store.Recovery.Add(Job());
        await Execute(store,io,true);
        Check(io.OnCount==0 && io.OffCount==1 && !store.On && store.Off,"Recovery sends OFF only; never invents ON proof");
        Check(store.SavedState==TalimatDurumu.KontrolGerekli && store.Claimed==0,"Recovery stops for review before new jobs");

        store=new Store(); io=new Io(); bad=Job(); bad.Url="http://127.0.0.2:8080/01"; store.Recovery.Add(bad);
        await Execute(store,io,true);
        Check(io.OnCount==0 && io.OffCount==0 && !store.Off,"Recovery rejects changed address");

        store=new Store(); io=new Io(); store.Pending.Add(Job());
        var entered=new TaskCompletionSource<bool>(); var release=new TaskCompletionSource<bool>();
        io.Delay=ms => { entered.TrySetResult(true); return release.Task; };
        var worker=new TalimatIsleyici(() => store,1,"127.0.0.1",60,io.Service());
        var running=worker.CalistirAsync(store.Stop.Token);
        await entered.Task;
        bool duplicate=false;
        Task duplicateRun=null;
        try { duplicateRun=worker.CalistirAsync(store.Stop.Token); } catch (InvalidOperationException) { duplicate=true; }
        Check(duplicate && duplicateRun==null,"Duplicate RUN rejected");
        store.Stop.Cancel();
        Check(!running.IsCompleted && io.OffCount==0,"STOP waits for current pulse");
        release.SetResult(true);
        await running;
        Check(io.OffCount==1 && store.Saves==1 && store.SavedState==TalimatDurumu.Tamamlandi,"STOP still finishes OFF and saves");

        var job=Job(); var link=Connection(); link.HttpPort=80; job.Url="http://127.0.0.1/01";
        Check(TalimatIsleyici.AdresKontrol(job,link)==null,"Default HTTP port accepted");
        foreach (var url in new[] {"https://127.0.0.1/01", "http://127.0.0.1/00", "http://127.0.0.1/01?x=1", "http://user@127.0.0.1/01", "http://127.0.0.1/01#x"})
        { job.Url=url; Check(TalimatIsleyici.AdresKontrol(job,link)!=null,"Unsafe/mismatched URL rejected"); }

        store=new Store { FailGlobalLock=true }; io=new Io(); store.Pending.Add(Job());
        await Execute(store,io,true);
        Check(store.Claimed==0 && io.OnCount==0, "Second worker cannot claim or send commands");

        store=new Store { FailLockRelease=true }; io=new Io(); store.Pending.Add(Job()); store.Pending.Add(Job());
        await Execute(store,io,true);
        Check(store.Claimed==1 && io.OnCount==1 && store.Locks.Count==0, "Release failure closes session before new work");

        store=new Store { EmptySave=true }; io=new Io(); store.Pending.Add(Job());
        await Execute(store,io,true);
        Check(store.Saves==1 && io.OnCount==1, "Missing finalization row stops without resending pulse");

        int factories=0;
        var cancelled=new CancellationTokenSource(); cancelled.Cancel();
        worker=new TalimatIsleyici(() => { factories++; return new Store(); },1,"127.0.0.1",60,new Io().Service());
        await worker.CalistirAsync(cancelled.Token);
        Check(factories==0 && !worker.Calisiyor,"Cancelled start opens no connection");
    }

    private static int Main()
    {
        try { Run().GetAwaiter().GetResult(); Console.WriteLine(checks+" talimat checks passed; no database or real relay used."); return 0; }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
