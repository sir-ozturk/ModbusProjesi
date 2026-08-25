CREATE OR ALTER PROCEDURE dbo.SP_Roller_EKLE
    @rol_kodu NVARCHAR(20),
    @adi NVARCHAR(100),
    @aciklama NVARCHAR(500),
    @aktif_mi BIT,
    @ekleyen_id INT,
    @ekleyen_ip NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Roller
    (
        rol_kodu,
        adi,
        aciklama,
        aktif_mi,
        eklenme_tarih,
        ekleyen_id,
        ekleyen_ip
    )
    VALUES
    (
        @rol_kodu,
        @adi,
        @aciklama,
        @aktif_mi,
        GETDATE(),
        @ekleyen_id,
        @ekleyen_ip
    );

	RETURN;
END