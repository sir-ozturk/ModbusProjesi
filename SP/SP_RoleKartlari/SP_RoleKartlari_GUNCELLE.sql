CREATE OR ALTER PROCEDURE dbo.SP_RoleKartlari_GUNCELLE
    @id INT,
    @role_adi NVARCHAR(100),
    @ethernet_kart_id INT,
    @aktif_mi BIT,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.RoleKartlari
    SET
        role_adi = @role_adi,
        ethernet_kart_id = @ethernet_kart_id,
        aktif_mi = @aktif_mi,
        guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()
    WHERE id = @id;

    RETURN;
END
