CREATE OR ALTER PROCEDURE dbo.SP_RolYetkiler_ROLE_GORE_GETIR
    @rol_id INT
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
        aktif_mi,
        eklenme_tarih,
        ekleyen_id,
        ekleyen_ip,
        guncelleyen_id,
        guncelleyen_ip,
        guncellenme_tarih
    FROM dbo.RolYetkiler
    WHERE rol_id = @rol_id
      AND aktif_mi = 1
    ORDER BY ekran;
END
