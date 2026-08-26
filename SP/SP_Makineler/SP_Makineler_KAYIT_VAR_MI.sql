CREATE PROCEDURE dbo.SP_Makineler_KAYIT_VAR_MI
    @id INT,
    @makine_no NVARCHAR(5),
    @ip NVARCHAR(50),
    @mfg NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.Makineler
    WHERE id <> @id
      AND
      (
          makine_no = @makine_no
          OR ip = @ip
          OR mfg = @mfg
      );
END