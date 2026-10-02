USE [DB_MODBUS]
GO
/****** Object:  StoredProcedure [dbo].[SP_ParametreGruplari_LISTELE]    Script Date: 01.10.2026 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[SP_ParametreGruplari_LISTELE]

    @aktif_mi BIT = NULL

AS
BEGIN

    SELECT
        G.id,
        G.kod,
        G.adi,
        G.sira_no,
        G.aktif_mi,
        G.eklenme_tarih,
        G.ekleyen_id,
        G.ekleyen_ip,
        G.guncelleyen_id,
        G.guncelleyen_ip,
        G.guncellenme_tarih
    FROM ParametreGruplari G
    WHERE (@aktif_mi IS NULL OR G.aktif_mi = @aktif_mi)
    ORDER BY G.sira_no, G.id;

END
GO
