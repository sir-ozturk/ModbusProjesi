CREATE OR ALTER PROCEDURE dbo.SP_MakineRoleBaglantilari_KOMUT_GETIR
    @makine_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        B.*,
        E.ip,
        E.http_port,
        R.ethernet_kart_id
    FROM dbo.MakineRoleBaglantilari B
    JOIN dbo.RoleKartlari R ON R.id = B.role_kart_id
    JOIN dbo.EthernetKartlari E ON E.id = R.ethernet_kart_id
    JOIN dbo.Makineler M ON M.id = B.makine_id
    WHERE B.makine_id = @makine_id
      AND B.aktif_mi = 1
      AND R.aktif_mi = 1
      AND E.aktif_mi = 1
      AND M.aktif_mi = 1;

    RETURN;
END
