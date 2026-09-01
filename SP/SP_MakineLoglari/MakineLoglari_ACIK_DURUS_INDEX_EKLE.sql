SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.MakineLoglari')
      AND name = 'UX_MakineLoglari_AcikDurus'
)
BEGIN
    CREATE UNIQUE INDEX UX_MakineLoglari_AcikDurus
        ON dbo.MakineLoglari(makine_id)
        WHERE devam_ediyor_mu = 1
          AND basarili_mi = 1
          AND aktif_mi = 1
          AND islem_tipi = N'DURDUR';
END
