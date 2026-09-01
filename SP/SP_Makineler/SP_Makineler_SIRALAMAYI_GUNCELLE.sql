SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.SP_Makineler_SIRALAMAYI_GUNCELLE
    @siralama_xml XML,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Siralamalar TABLE
    (
        makine_id INT NOT NULL PRIMARY KEY,
        sira_no INT NOT NULL UNIQUE
    );

    INSERT INTO @Siralamalar
    (
        makine_id,
        sira_no
    )
    SELECT
        X.Makine.value('@id', 'INT'),
        X.Makine.value('@sira', 'INT')
    FROM @siralama_xml.nodes('/Makineler/Makine') X(Makine);

    IF EXISTS
    (
        SELECT 1
        FROM @Siralamalar S
        LEFT JOIN dbo.Makineler M ON M.id = S.makine_id
        WHERE M.id IS NULL OR M.aktif_mi = 0
    )
    BEGIN
        THROW 50001, 'Geçersiz makine sıralaması.', 1;
    END

    IF (SELECT COUNT(*) FROM @Siralamalar) !=
       (SELECT COUNT(*) FROM dbo.Makineler WHERE aktif_mi = 1)
    BEGIN
        THROW 50002, 'Tüm aktif makineler sıralamaya dahil edilmelidir.', 1;
    END

    BEGIN TRANSACTION;

    UPDATE dbo.Makineler
    SET sira_no = -sira_no;

    UPDATE M
    SET M.sira_no = S.sira_no,
        M.guncelleyen_id = @guncelleyen_id,
        M.guncelleyen_ip = @guncelleyen_ip,
        M.guncellenme_tarih = GETDATE()
    FROM dbo.Makineler M
    INNER JOIN @Siralamalar S ON S.makine_id = M.id;

    DECLARE @AktifMakineSayisi INT = (SELECT COUNT(*) FROM @Siralamalar);

    ;WITH PasifMakineler AS
    (
        SELECT
            id,
            ROW_NUMBER() OVER (ORDER BY ABS(sira_no)) AS yeni_sira
        FROM dbo.Makineler
        WHERE aktif_mi = 0
    )
    UPDATE M
    SET M.sira_no = @AktifMakineSayisi + P.yeni_sira
    FROM dbo.Makineler M
    INNER JOIN PasifMakineler P ON P.id = M.id;

    COMMIT TRANSACTION;

    RETURN;
END
