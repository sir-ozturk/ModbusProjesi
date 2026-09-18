CREATE OR ALTER PROCEDURE dbo.SP_RelayPulses_EKLE
    @MakineId int, @ControllerIp nvarchar(45), @HttpPort int, @RelayChannel int,
    @PulseBaslangic datetime2(3), @PulseBitis datetime2(3)
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    SET ANSI_WARNINGS ON;
    SET ANSI_PADDING ON;
    SET CONCAT_NULL_YIELDS_NULL ON;
    SET NUMERIC_ROUNDABORT OFF;

    INSERT dbo.RelayPulses(MakineId,ControllerIp,HttpPort,RelayChannel,PulseAktif,PulseBaslangic,PulseBitis,SonrakiDeneme)
    OUTPUT inserted.Id
    VALUES(@MakineId,@ControllerIp,@HttpPort,@RelayChannel,1,@PulseBaslangic,@PulseBitis,@PulseBitis);
END

