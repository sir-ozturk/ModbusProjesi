SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE OR ALTER PROCEDURE dbo.SP_MakineDurdurmaTalimatlari_SONUCLANDIR
    @id INT,
    @islem_durumu TINYINT,
    @sonuc NVARCHAR(2000),
    @on_dogrulandi BIT,
    @off_dogrulandi BIT,
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
    IF @islem_durumu IS NULL OR @islem_durumu NOT IN (2,3,4)
        THROW 51109, N'Geçersiz sonuç durumu.', 1;
    IF @on_dogrulandi IS NULL OR @off_dogrulandi IS NULL OR NULLIF(LTRIM(RTRIM(@sonuc)), N'') IS NULL
        THROW 51110, N'Doğrulama bilgileri ve sonuç açıklaması gereklidir.', 1;
    IF @islem_durumu = 2 AND (@on_dogrulandi = 0 OR @off_dogrulandi = 0)
        THROW 51111, N'ON ve OFF doğrulanmadan işlem tamamlanamaz.', 1;
    IF @islem_durumu = 3 AND @on_dogrulandi = 1
        THROW 51112, N'ON doğrulanan kısmi işlem Kontrol Gerekli olarak kaydedilmelidir.', 1;
    IF @guncelleyen_id IS NULL OR @guncelleyen_id <= 0 OR NULLIF(LTRIM(RTRIM(@guncelleyen_ip)), N'') IS NULL
        THROW 51108, N'İşleyici kullanıcı ve IP bilgisi gereklidir.', 1;

    DECLARE @makine_id INT, @neden NVARCHAR(500), @ekleyen_id INT, @ekleyen_ip NVARCHAR(50);
    SELECT @makine_id = makine_id FROM dbo.MakineDurdurmaTalimatlari WHERE id = @id;
    BEGIN TRY
        BEGIN TRANSACTION;
        -- EKLE ile aynı kilit sırası: önce makine, sonra talimat.
        IF NOT EXISTS (SELECT 1 FROM dbo.Makineler WITH (UPDLOCK, HOLDLOCK) WHERE id = @makine_id)
            THROW 51113, N'Talimat veya makine bulunamadı.', 1;
        SELECT @neden = islem_nedeni, @ekleyen_id = ekleyen_id, @ekleyen_ip = ekleyen_ip
        FROM dbo.MakineDurdurmaTalimatlari WITH (UPDLOCK, HOLDLOCK)
        WHERE id = @id AND aktif_mi = 1 AND islem_durumu = 1;
        IF @ekleyen_id IS NULL
            THROW 51114, N'Yalnızca işlenmekte olan aktif talimat sonuçlandırılabilir.', 1;

        -- OFF hatasında da doğrulanmış ON için duruş kaydı tutulur.
        -- İşleyici ayrıca MakineLoglari.Ekle çağırmamalıdır.
        IF @on_dogrulandi = 1 AND NOT EXISTS
        (
            SELECT 1 FROM dbo.MakineLoglari WITH (UPDLOCK, HOLDLOCK)
            WHERE makine_id = @makine_id AND aktif_mi = 1 AND devam_ediyor_mu = 1
              AND basarili_mi = 1 AND islem_tipi = N'DURDUR'
        )
        BEGIN
            DECLARE @hata NVARCHAR(1000) = CASE WHEN @islem_durumu = 2 THEN NULL ELSE LEFT(@sonuc, 1000) END;
            EXEC dbo.SP_MakineLoglari_EKLE
                @makine_id = @makine_id, @islem_tipi = N'DURDUR', @islem_nedeni = @neden,
                @devam_ediyor_mu = 1, @basarili_mi = 1, @hata_mesaji = @hata,
                @aktif_mi = 1, @ekleyen_id = @ekleyen_id, @ekleyen_ip = @ekleyen_ip;
        END;

        UPDATE dbo.MakineDurdurmaTalimatlari
        SET islem_durumu = @islem_durumu, sonuc = @sonuc,
            islem_bitis_tarih = GETDATE(), guncellenme_tarih = GETDATE(),
            guncelleyen_id = @guncelleyen_id, guncelleyen_ip = @guncelleyen_ip
        WHERE id = @id AND aktif_mi = 1 AND islem_durumu = 1;
        COMMIT TRANSACTION;
        SELECT * FROM dbo.MakineDurdurmaTalimatlari WHERE id = @id;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO
