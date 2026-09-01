CREATE OR ALTER PROCEDURE dbo.SP_MakineLoglari_ACIK_KAYIT_GETIR
    @makine_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        id,
        makine_id,
        islem_tipi,
        islem_nedeni,
        islem_baslangic_tarih,
        islem_bitis_tarih,
        devam_ediyor_mu,
        basarili_mi,
        hata_mesaji,
        aktif_mi
    FROM dbo.MakineLoglari
    WHERE makine_id = @makine_id
      AND islem_tipi = N'DURDUR'
      AND devam_ediyor_mu = 1
      AND basarili_mi = 1
      AND aktif_mi = 1
    ORDER BY islem_baslangic_tarih DESC;

    RETURN;
END
