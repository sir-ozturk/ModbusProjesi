CREATE PROCEDURE dbo.SP_Makineler_TUMUNU_GETIR
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
    ORDER BY model_ad, makine_no;
END