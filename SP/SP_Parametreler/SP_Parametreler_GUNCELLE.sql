USE [DB_MODBUS]
GO
/****** Object:  StoredProcedure [dbo].[SP_Parametreler_GUNCELLE]    Script Date: 01.10.2026 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[SP_Parametreler_GUNCELLE]

    @id INT,
    @adi NVARCHAR(150),
    @aciklama NVARCHAR(500) = NULL,
    @sira_no INT,
    @aciklama_zorunlu_mu BIT,
    @aktif_mi BIT,
    @guncelleyen_id INT = NULL,
    @guncelleyen_ip NVARCHAR(50) = NULL

AS
BEGIN

    UPDATE Parametreler
    SET
        adi = @adi,
        aciklama = @aciklama,
        sira_no = @sira_no,
        aciklama_zorunlu_mu = @aciklama_zorunlu_mu,
        aktif_mi = @aktif_mi,
        guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()

    WHERE id = @id;

    RETURN

END
GO
