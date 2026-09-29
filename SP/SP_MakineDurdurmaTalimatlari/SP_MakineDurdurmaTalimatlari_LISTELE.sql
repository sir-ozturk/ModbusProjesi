SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE OR ALTER PROCEDURE dbo.SP_MakineDurdurmaTalimatlari_LISTELE
    @makine_id INT = NULL,
    @islem_durumu TINYINT = NULL,
    @adet INT = 100
AS
BEGIN
    SET NOCOUNT ON;
    IF @adet IS NULL OR @adet < 1 OR @adet > 1000
        THROW 51115, N'Liste adedi 1 ile 1000 arasında olmalıdır.', 1;
    IF @islem_durumu IS NOT NULL AND @islem_durumu NOT IN (0,1,2,3,4)
        THROW 51109, N'Geçersiz sonuç durumu.', 1;
    SELECT TOP (@adet) * FROM dbo.MakineDurdurmaTalimatlari
    WHERE aktif_mi = 1
      AND (@makine_id IS NULL OR makine_id = @makine_id)
      AND (@islem_durumu IS NULL OR islem_durumu = @islem_durumu)
    ORDER BY eklenme_tarih DESC, id DESC;
END;
GO
