CREATE OR ALTER PROCEDURE dbo.SP_RolYetkiler_GUNCELLE
    @id INT,
    @rol_id INT,
    @ekran NVARCHAR(100),
    @goruntuleme BIT,
    @ekleme BIT,
    @guncelleme BIT,
    @silme BIT,
    @aktif_mi BIT,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.RolYetkiler
    SET rol_id = @rol_id,
        ekran = @ekran,
        goruntuleme = @goruntuleme,
        ekleme = @ekleme,
        guncelleme = @guncelleme,
        silme = @silme,
        aktif_mi = @aktif_mi,
        guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()
    WHERE id = @id;

    RETURN;
END
