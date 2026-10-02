USE [DB_MODBUS]
GO
/****** Object:  StoredProcedure [dbo].[SP_Parametreler_GRUBA_GORE_GETIR]    Script Date: 01.10.2026 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[SP_Parametreler_GRUBA_GORE_GETIR]

    @grup_kodu NVARCHAR(50)

AS
BEGIN

    SELECT
        P.id,
        P.grup_id,
        P.kod,
        P.adi,
        P.aciklama,
        P.sira_no,
        P.aciklama_zorunlu_mu,
        P.aktif_mi,
        P.eklenme_tarih,
        P.ekleyen_id,
        P.ekleyen_ip,
        P.guncelleyen_id,
        P.guncelleyen_ip,
        P.guncellenme_tarih
    FROM Parametreler P
    INNER JOIN ParametreGruplari G ON G.id = P.grup_id
    WHERE G.kod = @grup_kodu
      AND G.aktif_mi = 1
      AND P.aktif_mi = 1
    ORDER BY P.sira_no, P.id;

END
GO
