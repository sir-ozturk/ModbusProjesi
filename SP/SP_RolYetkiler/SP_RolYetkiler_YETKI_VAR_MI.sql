CREATE PROCEDURE dbo.SP_RolYetkiler_YETKI_VAR_MI
    @rol_id INT,
    @ekran NVARCHAR(100),
    @islem_turu NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CASE @islem_turu
            WHEN 'GORUNTULE' THEN goruntuleme
            WHEN 'EKLE' THEN ekleme
            WHEN 'GUNCELLE' THEN guncelleme
            WHEN 'SIL' THEN silme
            WHEN 'YAZDIR' THEN yazdirma
            ELSE 0
        END
    FROM dbo.RolYetkiler
    WHERE rol_id = @rol_id
      AND ekran = @ekran
      AND aktif_mi = 1;
END