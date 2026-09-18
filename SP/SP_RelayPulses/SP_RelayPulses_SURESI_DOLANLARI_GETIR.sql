CREATE OR ALTER PROCEDURE dbo.SP_RelayPulses_SURESI_DOLANLARI_GETIR
    @Recovery bit
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    SET ANSI_WARNINGS ON;
    SET ANSI_PADDING ON;
    SET CONCAT_NULL_YIELDS_NULL ON;
    SET NUMERIC_ROUNDABORT OFF;

    DECLARE @simdi datetime2(3)=CONVERT(datetime2(3),SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time');
    SELECT * FROM dbo.RelayPulses
    WHERE PulseAktif=1 AND (PulseBitis<=@simdi OR ReleaseDenemeSayisi>0)
      AND (@Recovery=1 OR SonrakiDeneme<=@simdi OR (KritikHata=0 AND DATEADD(second,12,PulseBaslangic)<=@simdi))
    ORDER BY PulseBitis;
END

