CREATE OR ALTER PROCEDURE dbo.SP_RelayPulseEvents_EKLE
    @PulseId bigint, @Komut nvarchar(100), @HamCevap nvarchar(max)=NULL,
    @Basarili bit, @Hata nvarchar(max)=NULL, @ReleaseDenemeSayisi int, @KritikHata bit
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    SET ANSI_WARNINGS ON;
    SET ANSI_PADDING ON;
    SET CONCAT_NULL_YIELDS_NULL ON;
    SET NUMERIC_ROUNDABORT OFF;

    INSERT dbo.RelayPulseEvents(PulseId,Komut,HamCevap,Basarili,Hata,ReleaseDenemeSayisi,KritikHata)
    VALUES(@PulseId,@Komut,@HamCevap,@Basarili,@Hata,@ReleaseDenemeSayisi,@KritikHata);
END

