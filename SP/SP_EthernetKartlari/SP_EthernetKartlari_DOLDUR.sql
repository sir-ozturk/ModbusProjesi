CREATE OR ALTER PROCEDURE dbo.SP_EthernetKartlari_DOLDUR
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        E.*,
        E.ip + ':' + CONVERT(VARCHAR(5), E.http_port) AS adres
    FROM dbo.EthernetKartlari E
    WHERE E.id = @id;

    RETURN;
END
