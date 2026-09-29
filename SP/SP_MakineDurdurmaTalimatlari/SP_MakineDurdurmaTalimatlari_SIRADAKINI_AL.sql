SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE OR ALTER PROCEDURE dbo.SP_MakineDurdurmaTalimatlari_SIRADAKINI_AL
    @gecerlilik_saniye INT,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET ANSI_WARNINGS ON;
    SET ANSI_PADDING ON;
    SET CONCAT_NULL_YIELDS_NULL ON;
    SET ARITHABORT ON;
    SET NUMERIC_ROUNDABORT OFF;
    -- İşletmenin belirleyeceği süre zorunludur; sessiz bir varsayılan yoktur.
    IF @gecerlilik_saniye IS NULL OR @gecerlilik_saniye < 1 OR @gecerlilik_saniye > 86400
        THROW 51107, N'Geçerlilik süresi 1 ile 86400 saniye arasında olmalıdır.', 1;
    IF @guncelleyen_id IS NULL OR @guncelleyen_id <= 0 OR NULLIF(LTRIM(RTRIM(@guncelleyen_ip)), N'') IS NULL
        THROW 51108, N'İşleyici kullanıcı ve IP bilgisi gereklidir.', 1;

    DECLARE @simdi DATETIME = GETDATE();
    UPDATE dbo.MakineDurdurmaTalimatlari
    SET islem_durumu = 3, sonuc = N'Talimatın geçerlilik süresi doldu; cihaz komutu gönderilmedi.',
        islem_bitis_tarih = @simdi, guncellenme_tarih = @simdi,
        guncelleyen_id = @guncelleyen_id, guncelleyen_ip = @guncelleyen_ip
    WHERE aktif_mi = 1 AND islem_durumu = 0
      AND eklenme_tarih <= DATEADD(SECOND, -@gecerlilik_saniye, @simdi);

    -- Seçme ve sahiplenme tek UPDATE'tir. Ayrı SELECT + UPDATE yapılmaz.
    -- Tek işleyici desteklenir. READCOMMITTEDLOCK, RCSI açık DB'lerle uyumludur.
    ;WITH Siradaki AS
    (
        SELECT TOP (1) *
        FROM dbo.MakineDurdurmaTalimatlari WITH (UPDLOCK, READPAST, READCOMMITTEDLOCK)
        WHERE aktif_mi = 1 AND islem_durumu = 0
          AND eklenme_tarih > DATEADD(SECOND, -@gecerlilik_saniye, @simdi)
        ORDER BY eklenme_tarih, id
    )
    UPDATE Siradaki
    SET islem_durumu = 1, islem_baslangic_tarih = @simdi,
        guncellenme_tarih = @simdi, guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip
    OUTPUT inserted.*;
END;
GO
