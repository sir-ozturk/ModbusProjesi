CREATE PROCEDURE dbo.SP_Loglar_SIL
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Loglar
    WHERE id = @id;
END