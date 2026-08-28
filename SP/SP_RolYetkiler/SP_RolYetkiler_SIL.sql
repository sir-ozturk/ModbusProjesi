CREATE OR ALTER PROCEDURE dbo.SP_RolYetkiler_SIL
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.RolYetkiler
    WHERE id = @id;
END
