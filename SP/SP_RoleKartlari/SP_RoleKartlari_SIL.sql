CREATE OR ALTER PROCEDURE dbo.SP_RoleKartlari_SIL
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.RoleKartlari
    WHERE id = @id;

    RETURN;
END
