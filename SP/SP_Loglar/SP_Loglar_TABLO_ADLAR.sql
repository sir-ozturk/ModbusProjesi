ALTER PROCEDURE dbo.SP_Loglar_TABLO_ADLAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT tablo_adi
    FROM dbo.Loglar
    WHERE tablo_adi IS NOT NULL
      AND tablo_adi <> ''
    ORDER BY tablo_adi;
END