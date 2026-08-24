ALTER PROCEDURE dbo.SP_Loglar_TUMUNU_GETIR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        L.id,
        L.kullanici_id,
        K.kullanici_adi,
        K.ad,
        K.soyad,
        L.url,
        L.tablo_adi,
        L.islem_adi,
        L.islem_tipi,
        L.detay,
        L.ip_adres,
        L.islem_tarihi
    FROM dbo.Loglar L
    LEFT JOIN dbo.Kullanicilar K
        ON L.kullanici_id = K.id

    ORDER BY L.islem_tarihi DESC;
END