CREATE OR ALTER PROCEDURE SP_Kullanicilar_SIFREKONTROL

@KullaniciAdi NVARCHAR(50),
@Mail NVARCHAR(100)

AS
BEGIN

SELECT * FROM Kullanicilar WHERE kullanici_adi = @KullaniciAdi
AND mail = @Mail
AND aktiflik_durumu = 1;

END;
GO