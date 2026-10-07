USE [DB_MODBUS]
GO
/****** Object:  StoredProcedure [dbo].[SP_Parametreler_LISTELE]    Script Date: 01.10.2026 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[SP_Parametreler_LISTELE]

    @grup_id INT = NULL,
    @aktif_mi BIT = NULL,
    @arama NVARCHAR(150) = NULL

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
        P.guncellenme_tarih,
        G.kod AS grup_kodu,
        G.adi AS grup_adi,
        G.aktif_mi AS grup_aktif_mi
    FROM Parametreler P
    INNER JOIN ParametreGruplari G ON G.id = P.grup_id
    WHERE (@grup_id IS NULL OR P.grup_id = @grup_id)
      AND (@aktif_mi IS NULL OR P.aktif_mi = @aktif_mi)
      AND (@arama IS NULL OR CHARINDEX(@arama, P.kod) > 0 OR CHARINDEX(@arama, P.adi) > 0)
    ORDER BY P.grup_id, P.sira_no, P.id;

END
GO
