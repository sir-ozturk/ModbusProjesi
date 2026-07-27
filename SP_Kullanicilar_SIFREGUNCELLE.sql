CREATE OR ALTER PROCEDURE SP_Kullanicilar_SIFREGUNCELLE

@KullaniciAdi NVARCHAR(50),
@Mail NVARCHAR(100),
@Sifre NVARCHAR(50)

AS
BEGIN

UPDATE Kullanicilar SET sifre = @Sifre WHERE kullanici_adi = @KullaniciAdi AND mail = @Mail;

END;
GO