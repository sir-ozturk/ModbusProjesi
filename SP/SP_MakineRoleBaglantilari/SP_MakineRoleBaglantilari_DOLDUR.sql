CREATE OR ALTER PROCEDURE dbo.SP_MakineRoleBaglantilari_DOLDUR
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        B.*,
        R.role_adi,
        E.kart_adi,
        E.ip,
        E.http_port,
        E.ip + ':' + CONVERT(VARCHAR(5), E.http_port) AS adres,
        M.makine_no,
        M.makine_adi
    FROM dbo.MakineRoleBaglantilari B
    JOIN dbo.RoleKartlari R ON R.id = B.role_kart_id
    JOIN dbo.EthernetKartlari E ON E.id = R.ethernet_kart_id
    JOIN dbo.Makineler M ON M.id = B.makine_id
    WHERE B.id = @id;

    RETURN;
END
