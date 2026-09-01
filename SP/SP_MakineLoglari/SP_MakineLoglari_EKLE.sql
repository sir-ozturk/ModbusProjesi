SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.SP_MakineLoglari_EKLE
    @makine_id INT,
    @islem_tipi NVARCHAR(30),
    @islem_nedeni NVARCHAR(500),
    @devam_ediyor_mu BIT,
    @basarili_mi BIT,
    @hata_mesaji NVARCHAR(1000),
    @aktif_mi BIT,
    @ekleyen_id INT,
    @ekleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.MakineLoglari
    (
        makine_id,
        islem_tipi,
        islem_nedeni,
        islem_baslangic_tarih,
        islem_bitis_tarih,
        devam_ediyor_mu,
        basarili_mi,
        hata_mesaji,
        aktif_mi,
        eklenme_tarih,
        ekleyen_id,
        ekleyen_ip
    )
    VALUES
    (
        @makine_id,
        @islem_tipi,
        @islem_nedeni,
        GETDATE(),
        NULL,
        @devam_ediyor_mu,
        @basarili_mi,
        @hata_mesaji,
        @aktif_mi,
        GETDATE(),
        @ekleyen_id,
        @ekleyen_ip
    );

    RETURN;
END
