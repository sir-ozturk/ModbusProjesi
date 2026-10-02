USE [DB_MODBUS]
GO
/****** Object: StoredProcedure [dbo].[SP_MakineDurdurmaTalimatlari_EKLE] ******/
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
ALTER   PROCEDURE [dbo].[SP_MakineDurdurmaTalimatlari_EKLE]
    @makine_id INT,
    @islem_nedeni NVARCHAR(500),
    @ekleyen_id INT,
    @ekleyen_ip NVARCHAR(50),
    @durus_nedeni_parametre_id INT = NULL,
    @durus_aciklamasi NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET ANSI_WARNINGS ON;
    SET ANSI_PADDING ON;
    SET CONCAT_NULL_YIELDS_NULL ON;
    SET ARITHABORT ON;
    SET NUMERIC_ROUNDABORT OFF;
    IF NULLIF(LTRIM(RTRIM(@islem_nedeni)), N'') IS NULL
        THROW 51101, N'Durdurma nedeni gereklidir.', 1;
    IF @ekleyen_id IS NULL OR @ekleyen_id <= 0 OR NULLIF(LTRIM(RTRIM(@ekleyen_ip)), N'') IS NULL
        THROW 51102, N'Talimatı oluşturan kullanıcı ve IP bilgisi gereklidir.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;
        -- Makine bazında ekleme/sonuçlandırma işlemlerini kısa süreli sırala.
        IF NOT EXISTS (SELECT 1 FROM dbo.Makineler WITH (UPDLOCK, HOLDLOCK) WHERE id = @makine_id AND aktif_mi = 1)
            THROW 51103, N'Aktif makine bulunamadı.', 1;

        IF EXISTS (SELECT 1 FROM dbo.MakineDurdurmaTalimatlari WHERE makine_id = @makine_id AND aktif_mi = 1 AND islem_durumu IN (0,1))
            THROW 51104, N'Bu makine için bekleyen veya işlenen talimat var.', 1;
        IF EXISTS (SELECT 1 FROM dbo.MakineLoglari WHERE makine_id = @makine_id AND aktif_mi = 1 AND devam_ediyor_mu = 1 AND basarili_mi = 1 AND islem_tipi = N'DURDUR')
            THROW 51105, N'Makinenin açık duruş kaydı var.', 1;

        DECLARE @url NVARCHAR(500);
        -- Serbest URL kabul edilmez; mevcut aktif bağlantıdan ON adresi üretilir.
        SELECT @url = N'http://' + E.ip + N':' + CONVERT(NVARCHAR(5), E.http_port)
            + N'/' + RIGHT(N'0' + CONVERT(NVARCHAR(2), (B.kanal_no - 1) * 2 + 1), 2)
        FROM dbo.MakineRoleBaglantilari B
        JOIN dbo.RoleKartlari R ON R.id = B.role_kart_id AND R.aktif_mi = 1
        JOIN dbo.EthernetKartlari E ON E.id = R.ethernet_kart_id AND E.aktif_mi = 1
        WHERE B.makine_id = @makine_id AND B.aktif_mi = 1
          AND B.kanal_no BETWEEN 1 AND 16 AND E.http_port BETWEEN 1 AND 65535;
        IF @url IS NULL
            THROW 51106, N'Aktif röle bağlantısı bulunamadı.', 1;

        INSERT dbo.MakineDurdurmaTalimatlari(makine_id, url, islem_nedeni, ekleyen_id, ekleyen_ip, durus_nedeni_parametre_id, durus_aciklamasi)
        VALUES (@makine_id, @url, LTRIM(RTRIM(@islem_nedeni)), @ekleyen_id, @ekleyen_ip, @durus_nedeni_parametre_id, @durus_aciklamasi);
        DECLARE @id INT = CONVERT(INT, SCOPE_IDENTITY());
        COMMIT TRANSACTION;
        SELECT * FROM dbo.MakineDurdurmaTalimatlari WHERE id = @id;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO
