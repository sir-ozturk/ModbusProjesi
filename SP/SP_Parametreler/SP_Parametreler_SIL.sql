USE [DB_MODBUS]
GO
/****** Object:  StoredProcedure [dbo].[SP_Parametreler_SIL]    Script Date: 02.10.2026 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[SP_Parametreler_SIL]

    @id INT

AS
BEGIN

    DELETE FROM Parametreler
    WHERE id = @id;

    RETURN

END
GO
