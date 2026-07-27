CREATE OR ALTER PROCEDURE SP_Kullanicilar_GUNCELLE
@Id INT,
@KullaniciAdi NVARCHAR(50),
@Sifre NVARCHAR(50),
@Ad NVARCHAR(50),
@Soyad NVARCHAR(50),
@Telefon NVARCHAR(13),
@Mail NVARCHAR(100),
@RolId INT,
@AktiflikDurumu BIT,
@ProfilResim NVARCHAR(200)

AS
BEGIN

SET NOCOUNT ON;

UPDATE Kullanicilar
SET
kullanici_adi=@KullaniciAdi,
sifre=@Sifre,
ad=@Ad,
soyad=@Soyad,
telefon=@Telefon,
mail=@Mail,
rol_id=@RolId,
aktiflik_durumu=@AktiflikDurumu,
profil_resim=@ProfilResim

WHERE id=@Id;

END;



