SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.RelayPulses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RelayPulses
    (
        Id bigint IDENTITY PRIMARY KEY,
        MakineId int NOT NULL,
        ControllerIp nvarchar(45) NOT NULL,
        HttpPort int NOT NULL CHECK(HttpPort BETWEEN 1 AND 65535),
        RelayChannel int NOT NULL CHECK(RelayChannel BETWEEN 1 AND 16),
        PulseAktif bit NOT NULL,
        PulseBaslangic datetime2(3) NOT NULL,
        PulseBitis datetime2(3) NOT NULL,
        SonrakiDeneme datetime2(3) NOT NULL,
        GercekOnZamani datetime2(3) NULL,
        OffDogrulandi bit NOT NULL DEFAULT 0,
        ReleaseDenemeSayisi int NOT NULL DEFAULT 0,
        KritikHata bit NOT NULL DEFAULT 0,
        SonHata nvarchar(max) NULL,
        CONSTRAINT CK_RelayPulse_Sure CHECK(PulseBitis=DATEADD(second,10,PulseBaslangic))
    );
    CREATE UNIQUE INDEX UX_RelayPulse_Makine ON dbo.RelayPulses(MakineId) WHERE PulseAktif=1;
    CREATE UNIQUE INDEX UX_RelayPulse_Kanal ON dbo.RelayPulses(ControllerIp,HttpPort,RelayChannel) WHERE PulseAktif=1;
    CREATE INDEX IX_RelayPulse_Bitis ON dbo.RelayPulses(PulseBitis,SonrakiDeneme) WHERE PulseAktif=1;
END;
IF OBJECT_ID(N'dbo.RelayPulseEvents', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RelayPulseEvents
    (
        Id bigint IDENTITY PRIMARY KEY,
        PulseId bigint NOT NULL REFERENCES dbo.RelayPulses(Id),
        Tarih datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
        Komut nvarchar(100) NOT NULL,
        HamCevap nvarchar(max) NULL,
        Basarili bit NOT NULL,
        Hata nvarchar(max) NULL,
        ReleaseDenemeSayisi int NOT NULL,
        KritikHata bit NOT NULL
    );
    CREATE INDEX IX_RelayPulseEvents_Pulse ON dbo.RelayPulseEvents(PulseId,Tarih);
END;
COMMIT;
GO
CREATE OR ALTER VIEW dbo.RelayPulseAudit AS
SELECT E.*, P.MakineId,P.ControllerIp,P.HttpPort,P.RelayChannel,
P.PulseBaslangic AS OffKomutPlanZamani,P.PulseBitis AS PlanlananOnZamani,
P.GercekOnZamani,P.PulseAktif,P.OffDogrulandi
FROM dbo.RelayPulseEvents E JOIN dbo.RelayPulses P ON P.Id=E.PulseId;
GO
