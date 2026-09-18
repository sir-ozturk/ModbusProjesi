CREATE OR ALTER PROCEDURE dbo.SP_RelayPulses_CIHAZ_KILIDI_AL
    @ControllerIp nvarchar(45), @HttpPort int
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    SET ANSI_WARNINGS ON;
    SET ANSI_PADDING ON;
    SET CONCAT_NULL_YIELDS_NULL ON;
    SET NUMERIC_ROUNDABORT OFF;

    DECLARE @kaynak nvarchar(255)=N'RelayPulseController:'+@ControllerIp+N':'+CONVERT(nvarchar(10),@HttpPort);
    DECLARE @sonuc int;
    -- Oturum kilidi OFF/ON ve doğrulamayı aynı cihaz için sıralar; transaction açılmaz.
    EXEC @sonuc=sys.sp_getapplock @Resource=@kaynak,@LockMode='Exclusive',@LockOwner='Session',@LockTimeout=0;
    SELECT @sonuc;
END

