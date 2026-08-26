CREATE PROCEDURE dbo.SP_Makineler_DOLDUR
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        model_ad,
        entegrasyon_kod,
        gg_no,
        makine_no,
        band_no,
        ip,
        mfg,
        aktif_mi,
        eklenme_tarih,
        ekleyen_id,
        ekleyen_ip,
        guncelleyen_id,
        guncelleyen_ip,
        guncellenme_tarih
    FROM dbo.Makineler
    WHERE id = @id;

    RETURN;
END