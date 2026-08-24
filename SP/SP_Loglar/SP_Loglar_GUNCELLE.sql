CREATE PROCEDURE dbo.SP_Loglar_GUNCELLE
    @id INT,
    @kullanici_id INT = NULL,
    @url NVARCHAR(500) = NULL,
    @tablo_adi NVARCHAR(100) = NULL,
    @islem_adi NVARCHAR(200) = NULL,
    @islem_tipi CHAR(1) = NULL,
    @detay NVARCHAR(MAX) = NULL,
    @ip_adres NVARCHAR(45) = NULL,
    @islem_tarihi DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Loglar
    SET kullanici_id = @kullanici_id,
        url = @url,
        tablo_adi = @tablo_adi,
        islem_adi = @islem_adi,
        islem_tipi = @islem_tipi,
        detay = @detay,
        ip_adres = @ip_adres,
        islem_tarihi = @islem_tarihi
    WHERE id = @id;
END