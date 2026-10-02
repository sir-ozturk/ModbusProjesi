USE [DB_MODBUS]
GO
/****** Object: StoredProcedure [dbo].[SP_MakineLoglari_EKLE] ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

ALTER   PROCEDURE [dbo].[SP_MakineLoglari_EKLE]
    @makine_id INT,
    @islem_tipi NVARCHAR(30),
    @islem_nedeni NVARCHAR(500),
    @devam_ediyor_mu BIT,
    @basarili_mi BIT,
    @hata_mesaji NVARCHAR(1000),
    @aktif_mi BIT,
    @ekleyen_id INT,
    @ekleyen_ip NVARCHAR(50),
    @durus_nedeni_parametre_id INT = NULL,
    @durus_aciklamasi NVARCHAR(500) = NULL
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
        ekleyen_ip,
        durus_nedeni_parametre_id,
        durus_aciklamasi
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
        @ekleyen_ip,
        @durus_nedeni_parametre_id,
        @durus_aciklamasi
    );

    RETURN;
END

GO
