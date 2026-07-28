CREATE OR ALTER PROCEDURE SP_Kullanicilar_GETIR

@Id INT

AS
BEGIN

SELECT
id,kullanici_adi,sifre,ad,soyad,telefon,mail,rol_id,aktiflik_durumu,profil_resim FROM Kullanicilar WHERE id=@Id

END
GO