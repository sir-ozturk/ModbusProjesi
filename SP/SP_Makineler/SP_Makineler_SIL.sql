CREATE PROCEDURE dbo.SP_Makineler_SIL
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Makineler
    WHERE id = @id;

    RETURN;
END