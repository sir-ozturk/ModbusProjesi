CREATE OR ALTER PROCEDURE dbo.SP_MakineRoleBaglantilari_EKLE
    @role_kart_id INT,
    @kanal_no INT,
    @makine_id INT,
    @aktif_mi BIT,
    @ekleyen_id INT,
    @ekleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.MakineRoleBaglantilari
    (
        role_kart_id,
        kanal_no,
        makine_id,
        aktif_mi,
        eklenme_tarih,
        ekleyen_id,
        ekleyen_ip
    )
    VALUES
    (
        @role_kart_id,
        @kanal_no,
        @makine_id,
        @aktif_mi,
        GETDATE(),
        @ekleyen_id,
        @ekleyen_ip
    );

    RETURN;
END
