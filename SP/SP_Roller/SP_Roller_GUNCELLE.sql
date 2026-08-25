CREATE PROCEDURE dbo.SP_Roller_GUNCELLE
    @id INT,
    @adi NVARCHAR(100),
    @aciklama NVARCHAR(500),
    @aktif_mi BIT,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Roller
    SET
        adi = @adi,
        aciklama = @aciklama,
        aktif_mi = @aktif_mi,
        guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()
    WHERE id = @id

    RETURN
END