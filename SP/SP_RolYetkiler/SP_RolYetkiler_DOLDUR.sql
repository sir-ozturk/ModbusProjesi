CREATE PROCEDURE dbo.SP_RolYetkiler_DOLDUR
    @id INT
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
    WHERE id = @id;
END