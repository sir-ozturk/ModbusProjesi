-- Web ve watchdog durdurulduğunda çalıştırılır. Mevcut UTC kayıtları yalnız bir kez dönüştürülür.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
DECLARE @lock int;
EXEC @lock=sys.sp_getapplock @Resource=N'RelayPulseTimeZoneMigration',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=0;
IF @lock<0 THROW 51000, 'Saat gecisi baska bir islem tarafindan yapiliyor.', 1;
IF OBJECT_ID(N'dbo.RelayPulses',N'U') IS NULL OR OBJECT_ID(N'dbo.RelayPulseEvents',N'U') IS NULL
    THROW 51000, 'Once 003_RelayPulseGuvenligi.sql uygulanmalidir.', 1;
IF NOT EXISTS (SELECT 1 FROM sys.extended_properties WHERE major_id=OBJECT_ID(N'dbo.RelayPulses') AND minor_id=0 AND name=N'RelayPulseTimeZone')
BEGIN
    UPDATE dbo.RelayPulses SET
        PulseBaslangic=CONVERT(datetime2(3),PulseBaslangic AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time'),
        PulseBitis=CONVERT(datetime2(3),PulseBitis AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time'),
        SonrakiDeneme=CONVERT(datetime2(3),SonrakiDeneme AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time'),
        GercekOnZamani=CONVERT(datetime2(3),GercekOnZamani AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time');
    UPDATE dbo.RelayPulseEvents SET Tarih=CONVERT(datetime2(3),Tarih AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time');
    EXEC sys.sp_addextendedproperty @name=N'RelayPulseTimeZone',@value=N'Turkey Standard Time',
        @level0type=N'SCHEMA',@level0name=N'dbo',@level1type=N'TABLE',@level1name=N'RelayPulses';
END;
ELSE IF NOT EXISTS (SELECT 1 FROM sys.extended_properties WHERE major_id=OBJECT_ID(N'dbo.RelayPulses') AND minor_id=0 AND name=N'RelayPulseTimeZone' AND CONVERT(nvarchar(128),value)=N'Turkey Standard Time')
    THROW 51000, 'Beklenmeyen pulse saat dilimi; otomatik donusum durduruldu.', 1;
DECLARE @defaultName sysname;
SELECT @defaultName=D.name FROM sys.default_constraints D JOIN sys.columns C ON C.object_id=D.parent_object_id AND C.column_id=D.parent_column_id
WHERE D.parent_object_id=OBJECT_ID(N'dbo.RelayPulseEvents') AND C.name=N'Tarih';
IF @defaultName IS NOT NULL
BEGIN
    DECLARE @dropDefault nvarchar(max)=N'ALTER TABLE dbo.RelayPulseEvents DROP CONSTRAINT '+QUOTENAME(@defaultName);
    EXEC(@dropDefault);
END;
ALTER TABLE dbo.RelayPulseEvents ADD CONSTRAINT DF_RelayPulseEvents_Tarih_Turkiye
DEFAULT (CONVERT(datetime2(3),SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Turkey Standard Time')) FOR Tarih;
COMMIT;
