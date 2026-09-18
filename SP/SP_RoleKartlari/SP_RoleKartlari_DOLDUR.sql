CREATE OR ALTER PROCEDURE dbo.SP_RoleKartlari_DOLDUR
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.*,
        E.kart_adi,
        E.ip + ':' + CONVERT(VARCHAR(5), E.http_port) AS adres,
        (SELECT COUNT(*) FROM dbo.MakineRoleBaglantilari B
         WHERE B.role_kart_id = R.id AND B.aktif_mi = 1) AS dolu_kanal
    FROM dbo.RoleKartlari R
    JOIN dbo.EthernetKartlari E ON E.id = R.ethernet_kart_id
    WHERE R.id = @id;

    RETURN;
END
