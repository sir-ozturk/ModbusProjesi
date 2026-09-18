-- SQLCMD ile -b seçeneğiyle çalıştırın. Tek transaction: hata halinde bağlantı kapanınca geri alınır.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
GO
IF OBJECT_ID('dbo.EthernetKartlari') IS NULL
CREATE TABLE dbo.EthernetKartlari (
    id INT IDENTITY PRIMARY KEY,
    kart_adi NVARCHAR(100) NOT NULL,
    model NVARCHAR(50) NOT NULL,
    ip VARCHAR(15) NOT NULL,
    http_port INT NOT NULL CHECK (http_port BETWEEN 1 AND 65535),
    aktif_mi BIT NOT NULL DEFAULT 1,
    ekleyen_id INT NULL, ekleyen_ip NVARCHAR(50) NULL,
    eklenme_tarih DATETIME NOT NULL DEFAULT GETDATE(),
    guncelleyen_id INT NULL, guncelleyen_ip NVARCHAR(50) NULL,
    guncellenme_tarih DATETIME NULL,
    CONSTRAINT UQ_EthernetKartlari_Adres UNIQUE(ip,http_port)
);
IF OBJECT_ID('dbo.RoleKartlari') IS NULL
CREATE TABLE dbo.RoleKartlari (
    id INT IDENTITY PRIMARY KEY,
    role_adi NVARCHAR(100) NOT NULL,
    ethernet_kart_id INT NOT NULL REFERENCES dbo.EthernetKartlari(id),
    kanal_sayisi INT NOT NULL DEFAULT 16 CHECK (kanal_sayisi=16),
    aktif_mi BIT NOT NULL DEFAULT 1,
    ekleyen_id INT NULL, ekleyen_ip NVARCHAR(50) NULL,
    eklenme_tarih DATETIME NOT NULL DEFAULT GETDATE(),
    guncelleyen_id INT NULL, guncelleyen_ip NVARCHAR(50) NULL,
    guncellenme_tarih DATETIME NULL,
    CONSTRAINT UQ_RoleKartlari_Ethernet UNIQUE(ethernet_kart_id)
);
IF OBJECT_ID('dbo.MakineRoleBaglantilari') IS NULL
BEGIN
CREATE TABLE dbo.MakineRoleBaglantilari (
    id INT IDENTITY PRIMARY KEY,
    role_kart_id INT NOT NULL REFERENCES dbo.RoleKartlari(id),
    kanal_no INT NOT NULL CHECK (kanal_no BETWEEN 1 AND 16),
    makine_id INT NOT NULL REFERENCES dbo.Makineler(id),
    aktif_mi BIT NOT NULL DEFAULT 1,
    ekleyen_id INT NULL, ekleyen_ip NVARCHAR(50) NULL,
    eklenme_tarih DATETIME NOT NULL DEFAULT GETDATE(),
    guncelleyen_id INT NULL, guncelleyen_ip NVARCHAR(50) NULL,
    guncellenme_tarih DATETIME NULL
);
CREATE UNIQUE INDEX UX_MakineRole_Kanal ON dbo.MakineRoleBaglantilari(role_kart_id,kanal_no) WHERE aktif_mi=1;
CREATE UNIQUE INDEX UX_MakineRole_Makine ON dbo.MakineRoleBaglantilari(makine_id) WHERE aktif_mi=1;
END;
GO
-- Prosedürler kendi klasörlerindeki dosyalardan yüklenir.
:r "SP\SP_EthernetKartlari\SP_EthernetKartlari_EKLE.sql"
GO
:r "SP\SP_EthernetKartlari\SP_EthernetKartlari_GUNCELLE.sql"
GO
:r "SP\SP_EthernetKartlari\SP_EthernetKartlari_DOLDUR.sql"
GO
:r "SP\SP_EthernetKartlari\SP_EthernetKartlari_TUMUNU_GETIR.sql"
GO
:r "SP\SP_EthernetKartlari\SP_EthernetKartlari_SIL.sql"
GO
:r "SP\SP_RoleKartlari\SP_RoleKartlari_EKLE.sql"
GO
:r "SP\SP_RoleKartlari\SP_RoleKartlari_GUNCELLE.sql"
GO
:r "SP\SP_RoleKartlari\SP_RoleKartlari_DOLDUR.sql"
GO
:r "SP\SP_RoleKartlari\SP_RoleKartlari_TUMUNU_GETIR.sql"
GO
:r "SP\SP_RoleKartlari\SP_RoleKartlari_SIL.sql"
GO
:r "SP\SP_MakineRoleBaglantilari\SP_MakineRoleBaglantilari_EKLE.sql"
GO
:r "SP\SP_MakineRoleBaglantilari\SP_MakineRoleBaglantilari_GUNCELLE.sql"
GO
:r "SP\SP_MakineRoleBaglantilari\SP_MakineRoleBaglantilari_DOLDUR.sql"
GO
:r "SP\SP_MakineRoleBaglantilari\SP_MakineRoleBaglantilari_TUMUNU_GETIR.sql"
GO
:r "SP\SP_MakineRoleBaglantilari\SP_MakineRoleBaglantilari_SIL.sql"
GO
:r "SP\SP_MakineRoleBaglantilari\SP_MakineRoleBaglantilari_KOMUT_GETIR.sql"
GO
:r "SP\SP_Makineler\SP_Makineler_DASHBOARD_GETIR.sql"
GO
-- Makine pasife alma/silme mevcut sayfalardan da korunur.
CREATE OR ALTER TRIGGER dbo.TR_Makineler_RoleKoruma ON dbo.Makineler AFTER UPDATE, DELETE
AS BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM deleted D JOIN dbo.MakineRoleBaglantilari B ON B.makine_id=D.id AND B.aktif_mi=1
     LEFT JOIN inserted I ON I.id=D.id WHERE I.id IS NULL OR I.aktif_mi=0)
     THROW 51002,N'Önce makinenin aktif röle bağlantısını kaldırınız.',1;
END;
GO
IF NOT EXISTS(SELECT 1 FROM sys.extended_properties WHERE class=0 AND name=N'ModbusRoleSqlGecisi')
BEGIN
 IF (SELECT COUNT(*) FROM dbo.Makineler WHERE makine_no=N'18' AND aktif_mi=1)<>1
     THROW 51002,N'Geçiş için Makine No 18 tek ve aktif olmalıdır.',1;
 DECLARE @makine INT=(SELECT id FROM dbo.Makineler WHERE makine_no=N'18' AND aktif_mi=1);
 DECLARE @ethernet INT, @role INT;
 SELECT @ethernet=id FROM dbo.EthernetKartlari WHERE ip='192.168.5.190' AND http_port=8080;
 IF @ethernet IS NULL BEGIN
     INSERT dbo.EthernetKartlari(kart_adi,model,ip,http_port) VALUES(N'HW-584 Test Kartı',N'HW-584','192.168.5.190',8080);
     SET @ethernet=SCOPE_IDENTITY();
 END;
 SELECT @role=id FROM dbo.RoleKartlari WHERE ethernet_kart_id=@ethernet;
 IF @role IS NULL BEGIN
     INSERT dbo.RoleKartlari(role_adi,ethernet_kart_id) VALUES(N'16 Kanallı Röle Kartı 1',@ethernet);
     SET @role=SCOPE_IDENTITY();
 END;
 IF EXISTS(SELECT 1 FROM dbo.MakineRoleBaglantilari WHERE aktif_mi=1 AND
     (makine_id=@makine OR (role_kart_id=@role AND kanal_no=1)))
     THROW 51002,N'Geçiş bağlantısı mevcut atamalarla çakışıyor; kontrol ediniz.',1;
 INSERT dbo.MakineRoleBaglantilari(role_kart_id,kanal_no,makine_id) VALUES(@role,1,@makine);
 EXEC sys.sp_addextendedproperty @name=N'ModbusRoleSqlGecisi',@value=N'1';
END;
GO
-- Mevcut makine yönetimi yetkilerini denk donanım ekranlarına bir kez taşı.
INSERT dbo.RolYetkiler(rol_id,ekran,goruntuleme,ekleme,guncelleme,silme,aktif_mi,eklenme_tarih,ekleyen_id,ekleyen_ip)
SELECT Y.rol_id,V.ekran,MAX(CONVERT(INT,Y.goruntuleme)),MAX(CONVERT(INT,Y.ekleme)),MAX(CONVERT(INT,Y.guncelleme)),MAX(CONVERT(INT,Y.silme)),1,GETDATE(),NULL,NULL
FROM dbo.RolYetkiler Y CROSS APPLY (VALUES
 (N'ETHERNET_KART_EKLE',N'MAKINE_EKLE'),(N'ETHERNET_KART_LISTELE',N'MAKINE_LISTELE'),
 (N'ROLE_KART_EKLE',N'MAKINE_EKLE'),(N'ROLE_KART_LISTELE',N'MAKINE_LISTELE'),
 (N'MAKINE_ROLE_BAGLANTI',N'MAKINE_EKLE')) V(ekran,kaynak)
WHERE Y.ekran=V.kaynak AND Y.aktif_mi=1
 AND NOT EXISTS(SELECT 1 FROM dbo.RolYetkiler X WHERE X.rol_id=Y.rol_id AND X.ekran=V.ekran)
GROUP BY Y.rol_id,V.ekran;
GO
COMMIT TRANSACTION;
