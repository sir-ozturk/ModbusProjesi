/* Giriþ yapan kullanýcýyý kontrol eder */

CREATE OR ALTER PROCEDURE SP_Kullanicilar_GIRIS

@KullaniciAdi NVARCHAR(50),
@Sifre NVARCHAR(50)

AS
BEGIN

SELECT * FROM Kullanicilar WHERE kullanici_adi = @KullaniciAdi
AND sifre = @Sifre
AND aktiflik_durumu = 1;

END;
GO