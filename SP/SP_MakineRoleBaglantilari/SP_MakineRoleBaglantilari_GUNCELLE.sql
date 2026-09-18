CREATE OR ALTER PROCEDURE dbo.SP_MakineRoleBaglantilari_GUNCELLE
    @id INT,
    @role_kart_id INT,
    @kanal_no INT,
    @makine_id INT,
    @aktif_mi BIT,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.MakineRoleBaglantilari
    SET
        role_kart_id = @role_kart_id,
        kanal_no = @kanal_no,
        makine_id = @makine_id,
        aktif_mi = @aktif_mi,
        guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()
    WHERE id = @id;

    RETURN;
END
