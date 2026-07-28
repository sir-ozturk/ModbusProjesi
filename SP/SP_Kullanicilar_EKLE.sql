CREATE OR ALTER PROCEDURE SP_Kullanicilar_EKLE
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
INSERT INTO Kullanicilar
(
kullanici_adi,
sifre,
ad,
soyad,
telefon,
mail,
rol_id,
aktiflik_durumu,
olusturulma_tarihi,
profil_resim
)
VALUES
(
@KullaniciAdi,
@Sifre,
@Ad,
@Soyad,
@Telefon,
@Mail,
@RolId,
@AktiflikDurumu,
GETDATE(),
@ProfilResim
);
END;
GO