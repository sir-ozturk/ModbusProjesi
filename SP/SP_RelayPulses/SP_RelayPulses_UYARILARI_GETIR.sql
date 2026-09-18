CREATE OR ALTER PROCEDURE dbo.SP_RelayPulses_UYARILARI_GETIR
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    SET ANSI_WARNINGS ON;
    SET ANSI_PADDING ON;
    SET CONCAT_NULL_YIELDS_NULL ON;
    SET NUMERIC_ROUNDABORT OFF;

    DECLARE @simdi datetime2(3)=CONVERT(datetime2(3),SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time');
    SELECT TOP(20) MakineId,ControllerIp,RelayChannel,SonHata FROM dbo.RelayPulses
    WHERE PulseAktif=1 AND (SonHata IS NOT NULL OR DATEADD(second,12,PulseBaslangic)<=@simdi)
    ORDER BY PulseBaslangic;
END

