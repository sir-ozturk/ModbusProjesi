CREATE OR ALTER PROCEDURE dbo.SP_Makineler_GUNCELLE
    @id INT,
    @model_ad NVARCHAR(100),
    @entegrasyon_kod NVARCHAR(100),
    @gg_no NVARCHAR(5),
    @makine_no NVARCHAR(5),
    @makine_adi NVARCHAR(100),
    @band_no NVARCHAR(50),
    @ip NVARCHAR(50),
    @mfg NVARCHAR(50),
    @aktif_mi BIT,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Makineler
    SET
        model_ad = @model_ad,
        entegrasyon_kod = @entegrasyon_kod,
        gg_no = @gg_no,
        makine_no = @makine_no,
        makine_adi = @makine_adi,
        band_no = @band_no,
        ip = @ip,
        mfg = @mfg,
        aktif_mi = @aktif_mi,
        guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()
    WHERE id = @id;

    RETURN;
END
