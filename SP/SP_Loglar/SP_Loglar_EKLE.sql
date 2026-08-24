CREATE PROCEDURE dbo.SP_Loglar_EKLE
    @kullanici_id INT = NULL,
    @url NVARCHAR(500) = NULL,
    @tablo_adi NVARCHAR(100) = NULL,
    @islem_adi NVARCHAR(200) = NULL,
    @islem_tipi CHAR(1) = NULL,
    @detay NVARCHAR(MAX) = NULL,
    @ip_adres NVARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Loglar
    (
        kullanici_id,
        url,
        tablo_adi,
        islem_adi,
        islem_tipi,
        detay,
        ip_adres,
        islem_tarihi
    )
    VALUES
    (
        @kullanici_id,
        @url,
        @tablo_adi,
        @islem_adi,
        @islem_tipi,
        @detay,
        @ip_adres,
        GETDATE()
    );
END