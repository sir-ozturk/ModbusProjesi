CREATE OR ALTER PROCEDURE dbo.SP_RoleKartlari_EKLE
    @role_adi NVARCHAR(100),
    @ethernet_kart_id INT,
    @aktif_mi BIT,
    @ekleyen_id INT,
    @ekleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.RoleKartlari
    (
        role_adi,
        ethernet_kart_id,
        aktif_mi,
        eklenme_tarih,
        ekleyen_id,
        ekleyen_ip
    )
    VALUES
    (
        @role_adi,
        @ethernet_kart_id,
        @aktif_mi,
        GETDATE(),
        @ekleyen_id,
        @ekleyen_ip
    );

    RETURN;
END
