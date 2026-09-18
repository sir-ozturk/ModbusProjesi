using System;
using System.Web;
using System.Web.Hosting;

public class RelayPulseApplication : HttpApplication
{
    protected void Application_Start(object sender, EventArgs e)
    {
        // IIS kapanışı izler; bekleyen işlemler sonraki başlangıçta SQL kayıtlarından kurtarılır.
        HostingEnvironment.QueueBackgroundWorkItem(token => RelayPulseService.Default.RunWatchdog(token));
    }
}
