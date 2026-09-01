SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.SP_MakineLoglari_KAPAT
    @id INT,
    @guncelleyen_id INT = NULL,
    @guncelleyen_ip NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.MakineLoglari
    SET islem_bitis_tarih = GETDATE(),
        devam_ediyor_mu = 0,
        guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()
    WHERE id = @id
      AND devam_ediyor_mu = 1
      AND aktif_mi = 1;

    RETURN;
END
