-- Önce bu dosyayı, sonra SP_Makineler_DASHBOARD_GETIR.sql dosyasını çalıştırın.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;
GO
IF COL_LENGTH('dbo.Makineler', 'relay_channel') IS NULL
    ALTER TABLE dbo.Makineler ADD relay_channel INT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Makineler_RelayChannel')
    ALTER TABLE dbo.Makineler ADD CONSTRAINT CK_Makineler_RelayChannel
        CHECK (relay_channel BETWEEN 1 AND 16);
GO
-- Mevcut tek cihaz testi: makine_no 18, IO1. Diğer makineler atanmadıkça izlenmez.
IF NOT EXISTS (SELECT 1 FROM dbo.Makineler WHERE relay_channel = 1)
    UPDATE dbo.Makineler SET relay_channel = 1 WHERE makine_no = N'18' AND relay_channel IS NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Makineler') AND name = 'UX_Makineler_RelayChannel')
    CREATE UNIQUE INDEX UX_Makineler_RelayChannel ON dbo.Makineler(relay_channel)
        WHERE relay_channel IS NOT NULL;
GO
