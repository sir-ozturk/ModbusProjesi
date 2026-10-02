USE [DB_MODBUS]
GO
/****** Object:  StoredProcedure [dbo].[SP_Parametreler_EKLE]    Script Date: 01.10.2026 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[SP_Parametreler_EKLE]

    @grup_id INT,
    @kod NVARCHAR(50),
    @adi NVARCHAR(150),
    @aciklama NVARCHAR(500) = NULL,
    @sira_no INT,
    @aciklama_zorunlu_mu BIT,
    @aktif_mi BIT,
    @ekleyen_id INT = NULL,
    @ekleyen_ip NVARCHAR(50) = NULL

AS
BEGIN

    INSERT INTO Parametreler
    (
        grup_id,
        kod,
        adi,
        aciklama,
        sira_no,
        aciklama_zorunlu_mu,
        aktif_mi,
        eklenme_tarih,
        ekleyen_id,
        ekleyen_ip
    )
    VALUES
    (
        @grup_id,
        @kod,
        @adi,
        @aciklama,
        @sira_no,
        @aciklama_zorunlu_mu,
        @aktif_mi,
        GETDATE(),
        @ekleyen_id,
        @ekleyen_ip
    );

END
GO
