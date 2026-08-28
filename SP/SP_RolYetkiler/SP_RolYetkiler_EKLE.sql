CREATE OR ALTER PROCEDURE dbo.SP_RolYetkiler_EKLE
    @rol_id INT,
    @ekran NVARCHAR(100),
    @goruntuleme BIT,
    @ekleme BIT,
    @guncelleme BIT,
    @silme BIT,
    @aktif_mi BIT,
    @ekleyen_id INT,
    @ekleyen_ip NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.RolYetkiler
    (
        rol_id,
        ekran,
        goruntuleme,
        ekleme,
        guncelleme,
        silme,
        aktif_mi,
        eklenme_tarih,
        ekleyen_id,
        ekleyen_ip
    )
    VALUES
    (
        @rol_id,
        @ekran,
        @goruntuleme,
        @ekleme,
        @guncelleme,
        @silme,
        @aktif_mi,
        GETDATE(),
        @ekleyen_id,
        @ekleyen_ip
    );

    RETURN;
END
