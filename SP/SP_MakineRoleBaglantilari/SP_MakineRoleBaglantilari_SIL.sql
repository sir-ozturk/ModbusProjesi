CREATE OR ALTER PROCEDURE dbo.SP_MakineRoleBaglantilari_SIL
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.MakineRoleBaglantilari
    WHERE id = @id;

    RETURN;
END
