CREATE PROCEDURE dbo.SP_Loglar_DOLDUR
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.Loglar
    WHERE id = @id;
END