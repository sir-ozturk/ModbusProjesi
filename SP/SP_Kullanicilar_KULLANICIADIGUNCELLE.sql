CREATE PROCEDURE [dbo].[SP_Kullanicilar_KULLANICIADIGUNCELLE]

    @id INT,
    @kullanici_adi NVARCHAR(50)

AS
BEGIN

    UPDATE Kullanicilar
    SET kullanici_adi = @kullanici_adi
    WHERE id = @id;

END