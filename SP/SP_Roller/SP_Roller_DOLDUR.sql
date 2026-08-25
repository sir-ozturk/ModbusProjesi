CREATE PROCEDURE dbo.SP_Roller_DOLDUR
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        rol_kodu,
        adi,
        aciklama,
        aktif_mi,
        eklenme_tarih,
        ekleyen_id,
        ekleyen_ip,
        guncelleyen_id,
        guncelleyen_ip,
        guncellenme_tarih
    FROM dbo.Roller
    WHERE id = @id;
END