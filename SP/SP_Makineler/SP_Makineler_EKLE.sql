CREATE OR ALTER PROCEDURE dbo.SP_Makineler_EKLE
    @model_ad NVARCHAR(100),
    @entegrasyon_kod NVARCHAR(100),
    @gg_no NVARCHAR(5),
    @makine_no NVARCHAR(5),
    @makine_adi NVARCHAR(100),
    @band_no NVARCHAR(50),
    @ip NVARCHAR(50),
    @mfg NVARCHAR(50),
    @aktif_mi BIT,
    @ekleyen_id INT,
    @ekleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Makineler
    (
        model_ad,
        entegrasyon_kod,
        gg_no,
        makine_no,
        makine_adi,
        sira_no,
        band_no,
        ip,
        mfg,
        aktif_mi,
        eklenme_tarih,
        ekleyen_id,
        ekleyen_ip
    )
    VALUES
    (
        @model_ad,
        @entegrasyon_kod,
        @gg_no,
        @makine_no,
        @makine_adi,
        ISNULL((SELECT MAX(sira_no) FROM dbo.Makineler), 0) + 1,
        @band_no,
        @ip,
        @mfg,
        @aktif_mi,
        GETDATE(),
        @ekleyen_id,
        @ekleyen_ip
    );

    RETURN;
END
