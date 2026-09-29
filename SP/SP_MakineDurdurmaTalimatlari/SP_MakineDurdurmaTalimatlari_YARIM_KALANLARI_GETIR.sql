SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE OR ALTER PROCEDURE dbo.SP_MakineDurdurmaTalimatlari_YARIM_KALANLARI_GETIR
AS
BEGIN
    SET NOCOUNT ON;
    -- Yalnızca tek işleyicinin açılışında, yeni iş almadan önce kullanılır.
    -- Çalışan başka uygulama olmadığı uygulama tarafından doğrulanmalıdır.
    -- Yaşa göre otomatik sahiplenme veya tekrar ON gönderme yapılmaz.
    SELECT * FROM dbo.MakineDurdurmaTalimatlari
    WHERE aktif_mi = 1 AND islem_durumu = 1
    ORDER BY islem_baslangic_tarih, id;
END;
GO
