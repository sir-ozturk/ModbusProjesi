CREATE OR ALTER PROCEDURE dbo.SP_Makineler_DASHBOARD_GETIR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        M.id,
        M.makine_adi,
        M.makine_no,
        M.ip,
        M.mfg,
        M.sira_no,
        CAST(CASE WHEN B.id IS NOT NULL AND R.aktif_mi=1 AND E.aktif_mi=1 THEN 1 ELSE 0 END AS BIT) AS role_bagli_mi,
        R.role_adi, B.kanal_no, E.ip AS role_ip, E.http_port AS role_http_port,
        CAST(CASE WHEN ML.id IS NULL THEN 0 ELSE 1 END AS BIT) AS duruyor_mu,
        ML.islem_nedeni,
        ML.islem_baslangic_tarih,
        CASE
            WHEN ML.id IS NULL THEN 0
            ELSE DATEDIFF(MINUTE, ML.islem_baslangic_tarih, GETDATE())
        END AS durus_dakika
    FROM dbo.Makineler M
    LEFT JOIN dbo.MakineRoleBaglantilari B ON B.makine_id=M.id AND B.aktif_mi=1
    LEFT JOIN dbo.RoleKartlari R ON R.id=B.role_kart_id
    LEFT JOIN dbo.EthernetKartlari E ON E.id=R.ethernet_kart_id
    OUTER APPLY
    (
        SELECT TOP 1
            L.id,
            L.islem_nedeni,
            L.islem_baslangic_tarih
        FROM dbo.MakineLoglari L
        WHERE L.makine_id = M.id
          AND L.islem_tipi = N'DURDUR'
          AND L.devam_ediyor_mu = 1
          AND L.basarili_mi = 1
          AND L.aktif_mi = 1
        ORDER BY L.islem_baslangic_tarih DESC
    ) ML
    WHERE M.aktif_mi = 1
    ORDER BY M.sira_no;

    RETURN;
END
