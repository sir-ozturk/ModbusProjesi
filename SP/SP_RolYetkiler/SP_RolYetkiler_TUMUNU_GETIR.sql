CREATE PROCEDURE dbo.SP_RolYetkiler_TUMUNU_GETIR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        rol_id,
        ekran,
        goruntuleme,
        ekleme,
        guncelleme,
        silme,
        yazdirma,
        aktif_mi,
        eklenme_tarih,
        ekleyen_id,
        ekleyen_ip,
        guncelleyen_id,
        guncelleyen_ip,
        guncellenme_tarih
    FROM dbo.RolYetkiler
    WHERE aktif_mi = 1
    ORDER BY rol_id, ekran;
END