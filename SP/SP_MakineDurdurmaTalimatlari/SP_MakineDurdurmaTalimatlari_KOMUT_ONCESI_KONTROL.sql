SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE OR ALTER PROCEDURE dbo.SP_MakineDurdurmaTalimatlari_KOMUT_ONCESI_KONTROL
    @id INT,
    @makine_id INT,
    @gecerlilik_saniye INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CASE
     WHEN NOT EXISTS (SELECT 1 FROM dbo.MakineDurdurmaTalimatlari
       WHERE id=@id AND makine_id=@makine_id AND aktif_mi=1 AND islem_durumu=1
         AND eklenme_tarih > DATEADD(SECOND,-@gecerlilik_saniye,GETDATE()))
       THEN N'Talimat artık geçerli değil veya süresi doldu.'
     WHEN NOT EXISTS (SELECT 1 FROM dbo.Makineler WHERE id=@makine_id AND aktif_mi=1)
       THEN N'Makine aktif değil.'
     WHEN EXISTS (SELECT 1 FROM dbo.MakineLoglari WHERE makine_id=@makine_id
       AND aktif_mi=1 AND devam_ediyor_mu=1 AND basarili_mi=1 AND islem_tipi=N'DURDUR')
       THEN N'Makinenin açık duruş kaydı var.'
     ELSE NULL END;
END;
GO
