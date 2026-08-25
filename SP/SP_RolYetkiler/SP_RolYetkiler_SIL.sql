CREATE PROCEDURE dbo.SP_RolYetkiler_SIL
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.RolYetkiler
    WHERE id = @id;

    RETURN;
END