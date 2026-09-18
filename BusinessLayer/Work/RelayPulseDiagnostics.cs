using System;
using System.Diagnostics;
using System.IO;

public static class RelayPulseDiagnostics
{
    private static readonly object gate = new object();
    public static void Error(string message)
    {
        try { Trace.TraceError(message); }
        catch (Exception ex) { message += " Trace listener hatası: " + ex.Message; }
        // SQL kullanılamazsa bağımsız dosya kaydı tutulur; uygulamanın App_Data yazma izni olmalıdır.
        try
        {
            lock (gate)
            {
                string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
                Directory.CreateDirectory(directory);
                File.AppendAllText(Path.Combine(directory, "RelayPulseCritical-" + RelayPulseClock.Now.ToString("yyyyMMdd") + ".log"),
                    RelayPulseClock.Now.ToString("o") + " " + message + Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            // Log yazma hatası acil ON denemesini hiçbir zaman engellemez.
            try { Trace.TraceError("KRİTİK: Yerel röle hata dosyasına yazılamadı: " + ex); }
            catch { /* Kalıcı pulse ve ekran uyarısı diğer hata bildirim yoludur. */ }
        }
    }
}
