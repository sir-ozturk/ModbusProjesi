CREATE OR ALTER PROCEDURE dbo.SP_EthernetKartlari_TUMUNU_GETIR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        E.*,
        E.ip + ':' + CONVERT(VARCHAR(5), E.http_port) AS adres
    FROM dbo.EthernetKartlari E
    ORDER BY E.id;

    RETURN;
END
