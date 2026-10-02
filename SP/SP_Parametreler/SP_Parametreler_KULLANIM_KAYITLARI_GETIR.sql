USE [DB_MODBUS]
GO
/****** Object:  StoredProcedure [dbo].[SP_Parametreler_KULLANIM_KAYITLARI_GETIR]    Script Date: 02.10.2026 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[SP_Parametreler_KULLANIM_KAYITLARI_GETIR]

    @id INT

AS
BEGIN

    SELECT
        durus_nedeni_parametre_id,
        islem_nedeni
    FROM MakineDurdurmaTalimatlari
    WHERE durus_nedeni_parametre_id = @id
       OR durus_nedeni_parametre_id IS NULL

    UNION ALL

    SELECT
        durus_nedeni_parametre_id,
        islem_nedeni
    FROM MakineLoglari
    WHERE durus_nedeni_parametre_id = @id
       OR durus_nedeni_parametre_id IS NULL;

END
GO
