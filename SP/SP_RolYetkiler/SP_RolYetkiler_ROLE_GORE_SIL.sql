CREATE PROCEDURE dbo.SP_RolYetkiler_ROLE_GORE_SIL
    @rol_id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.RolYetkiler
    WHERE rol_id = @rol_id;

    RETURN;
END