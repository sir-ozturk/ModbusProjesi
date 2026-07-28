CREATE OR ALTER PROCEDURE SP_Kullanicilar_LISTELE
AS
BEGIN

SELECT
K.id,
K.kullanici_adi,
K.ad,
K.soyad,
K.telefon,
K.mail,
K.aktiflik_durumu,
R.rol_adi
FROM Kullanicilar K INNER JOIN Roller R ON K.rol_id = R.id
END;
GO