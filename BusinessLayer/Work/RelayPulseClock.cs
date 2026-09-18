using System;

public static class RelayPulseClock
{
    private static readonly TimeZoneInfo Turkey = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
    // SQL datetime2 alanları Türkiye saatini tutar; sunucunun yerel saatinden bağımsızdır.
    public static DateTime Now { get { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Turkey); } }
}
