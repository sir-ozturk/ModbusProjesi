using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

internal sealed class RelayWatchdog : ServiceBase
{
    private CancellationTokenSource stopping;
    private Task work;
    public RelayWatchdog() { ServiceName = "ModbusRelayWatchdog"; }
    protected override void OnStart(string[] args)
    {
        stopping = new CancellationTokenSource();
        work = Task.Run(() => RelayPulseService.Default.RunWatchdog(stopping.Token));
    }
    protected override void OnStop()
    {
        RequestAdditionalTime(15000);
        stopping.Cancel();
        if (!work.Wait(14000)) Trace.TraceError("Watchdog kapanışında bekleyen işlemler SQL'de bırakıldı.");
    }
    private static void Main(string[] args)
    {
        if (args.Length == 1 && args[0] == "--console")
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            var cancel = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) => { e.Cancel = true; cancel.Cancel(); };
            RelayPulseService.Default.RunWatchdog(cancel.Token).GetAwaiter().GetResult();
        }
        else ServiceBase.Run(new RelayWatchdog());
    }
}
