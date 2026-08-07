CREATE OR ALTER PROCEDURE SP_Kullanicilar_KAYITVARMI

    @id INT = 0,
    @kullanici_adi NVARCHAR(50),
    @telefon NVARCHAR(13),
    @mail NVARCHAR(100)

AS
BEGIN

    SELECT COUNT(*)
    FROM Kullanicilar
    WHERE
    (
        kullanici_adi = @kullanici_adi
        OR telefon = @telefon
        OR mail = @mail
    )
    AND id <> @id;

END
GO