CREATE OR ALTER PROCEDURE dbo.SP_EthernetKartlari_SIL
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.EthernetKartlari
    WHERE id = @id;

    RETURN;
END
