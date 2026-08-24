ALTER PROCEDURE dbo.SP_Loglar_FILTRELI_GETIR
    @kullanici_adi NVARCHAR(50) = NULL,
    @tablo_adi NVARCHAR(100) = NULL,
    @islem_adi NVARCHAR(200) = NULL,
    @islem_tipi CHAR(1) = NULL,
    @baslangic_tarih DATETIME = NULL,
    @bitis_tarih DATETIME = NULL
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

    WHERE
        (
            @kullanici_adi IS NULL
            OR @kullanici_adi = ''
            OR K.kullanici_adi LIKE '%' + @kullanici_adi + '%'
        )

        AND
        (
            @tablo_adi IS NULL
            OR @tablo_adi = ''
            OR L.tablo_adi = @tablo_adi
        )

        AND
        (
            @islem_adi IS NULL
            OR @islem_adi = ''
            OR L.islem_adi = @islem_adi
        )

        AND
        (
            @islem_tipi IS NULL
            OR @islem_tipi = ''
            OR L.islem_tipi = @islem_tipi
        )

        AND
        (
            @baslangic_tarih IS NULL
            OR L.islem_tarihi >= @baslangic_tarih
        )

        AND
        (
            @bitis_tarih IS NULL
            OR L.islem_tarihi < DATEADD(DAY, 1, @bitis_tarih)
        )

    ORDER BY L.islem_tarihi DESC;
END