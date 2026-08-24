ALTER PROCEDURE dbo.SP_Loglar_ISLEM_ADLAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT islem_adi
    FROM dbo.Loglar
    WHERE islem_adi IS NOT NULL
      AND islem_adi <> ''
    ORDER BY islem_adi;
END