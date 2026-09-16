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
CREATE OR ALTER PROCEDURE dbo.SP_EthernetKartlari_EKLE
    @kart_adi NVARCHAR(100),
    @model NVARCHAR(50),
    @ip VARCHAR(15),
    @http_port INT,
    @aktif_mi BIT, @ekleyen_id INT, @ekleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
    BEGIN TRANSACTION;
    DECLARE @kilit INT;
    EXEC @kilit=sys.sp_getapplock @Resource=N'ModbusDonanimAyar', @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=0;
    IF @kilit<0 THROW 51001, N'Donanım için başka bir işlem devam ediyor. Tekrar deneyiniz.', 1;
DECLARE @id INT=0;
    IF NULLIF(LTRIM(RTRIM(@kart_adi)),N'') IS NULL OR NULLIF(LTRIM(RTRIM(@model)),N'') IS NULL OR @http_port NOT BETWEEN 1 AND 65535
        THROW 51002,N'Kart adı, model veya HTTP portu geçersiz.',1;
    IF EXISTS(SELECT 1 FROM dbo.EthernetKartlari WHERE ip=@ip AND http_port=@http_port AND id<>@id)
        THROW 51002,N'Bu IP ve HTTP portu zaten tanımlı.',1;
    IF @aktif_mi=0 AND EXISTS(SELECT 1 FROM dbo.RoleKartlari WHERE ethernet_kart_id=@id AND aktif_mi=1)
        THROW 51002,N'Önce bağlı röle kartını pasife alınız.',1;
    IF EXISTS(SELECT 1 FROM dbo.MakineRoleBaglantilari B JOIN dbo.RoleKartlari R ON R.id=B.role_kart_id
        JOIN dbo.MakineLoglari L ON L.makine_id=B.makine_id
        WHERE R.ethernet_kart_id=@id AND B.aktif_mi=1 AND L.devam_ediyor_mu=1 AND L.basarili_mi=1 AND L.aktif_mi=1 AND L.islem_tipi=N'DURDUR')
        THROW 51002,N'Açık duruş kaydı olan makine bulunduğu için cihaz değiştirilemez.',1;

    INSERT dbo.EthernetKartlari (kart_adi,model,ip,http_port,aktif_mi,ekleyen_id,ekleyen_ip) VALUES (@kart_adi,@model,@ip,@http_port,@aktif_mi,@ekleyen_id,@ekleyen_ip);
    COMMIT;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK;
        THROW;
    END CATCH
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_EthernetKartlari_GUNCELLE
    @id INT,
    @kart_adi NVARCHAR(100),
    @model NVARCHAR(50),
    @ip VARCHAR(15),
    @http_port INT,
    @aktif_mi BIT, @guncelleyen_id INT, @guncelleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
    BEGIN TRANSACTION;
    DECLARE @kilit INT;
    EXEC @kilit=sys.sp_getapplock @Resource=N'ModbusDonanimAyar', @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=0;
    IF @kilit<0 THROW 51001, N'Donanım için başka bir işlem devam ediyor. Tekrar deneyiniz.', 1;
IF NOT EXISTS(SELECT 1 FROM dbo.EthernetKartlari WHERE id=@id) THROW 51002,N'Kayıt bulunamadı.',1;
    IF NULLIF(LTRIM(RTRIM(@kart_adi)),N'') IS NULL OR NULLIF(LTRIM(RTRIM(@model)),N'') IS NULL OR @http_port NOT BETWEEN 1 AND 65535
        THROW 51002,N'Kart adı, model veya HTTP portu geçersiz.',1;
    IF EXISTS(SELECT 1 FROM dbo.EthernetKartlari WHERE ip=@ip AND http_port=@http_port AND id<>@id)
        THROW 51002,N'Bu IP ve HTTP portu zaten tanımlı.',1;
    IF @aktif_mi=0 AND EXISTS(SELECT 1 FROM dbo.RoleKartlari WHERE ethernet_kart_id=@id AND aktif_mi=1)
        THROW 51002,N'Önce bağlı röle kartını pasife alınız.',1;
    IF EXISTS(SELECT 1 FROM dbo.MakineRoleBaglantilari B JOIN dbo.RoleKartlari R ON R.id=B.role_kart_id
        JOIN dbo.MakineLoglari L ON L.makine_id=B.makine_id
        WHERE R.ethernet_kart_id=@id AND B.aktif_mi=1 AND L.devam_ediyor_mu=1 AND L.basarili_mi=1 AND L.aktif_mi=1 AND L.islem_tipi=N'DURDUR')
        THROW 51002,N'Açık duruş kaydı olan makine bulunduğu için cihaz değiştirilemez.',1;

    UPDATE dbo.EthernetKartlari SET kart_adi=@kart_adi,model=@model,ip=@ip,http_port=@http_port,aktif_mi=@aktif_mi,guncelleyen_id=@guncelleyen_id,guncelleyen_ip=@guncelleyen_ip,guncellenme_tarih=GETDATE() WHERE id=@id;
    COMMIT;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK;
        THROW;
    END CATCH
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_EthernetKartlari_DOLDUR @id INT
AS BEGIN SET NOCOUNT ON; SELECT E.*, E.ip+':'+CONVERT(VARCHAR(5),E.http_port) AS adres FROM dbo.EthernetKartlari E WHERE E.id=@id; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_EthernetKartlari_TUMUNU_GETIR 
AS BEGIN SET NOCOUNT ON; SELECT E.*, E.ip+':'+CONVERT(VARCHAR(5),E.http_port) AS adres FROM dbo.EthernetKartlari E ORDER BY E.id; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_EthernetKartlari_SIL @id INT
AS BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @kilit INT;
    EXEC @kilit=sys.sp_getapplock @Resource=N'ModbusDonanimAyar', @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=0;
    IF @kilit<0 THROW 51001, N'Donanım için başka bir işlem devam ediyor. Tekrar deneyiniz.', 1;

 IF EXISTS(SELECT 1 FROM dbo.RoleKartlari WHERE ethernet_kart_id=@id) THROW 51002,N'Bağlı röle kartı olan Ethernet kartı silinemez.',1;
 DELETE FROM dbo.EthernetKartlari WHERE id=@id;
 IF @@ROWCOUNT=0 THROW 51002,N'Kayıt bulunamadı.',1;
 COMMIT;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK; THROW; END CATCH
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_RoleKartlari_EKLE
    @role_adi NVARCHAR(100),
    @ethernet_kart_id INT,
    @aktif_mi BIT, @ekleyen_id INT, @ekleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
    BEGIN TRANSACTION;
    DECLARE @kilit INT;
    EXEC @kilit=sys.sp_getapplock @Resource=N'ModbusDonanimAyar', @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=0;
    IF @kilit<0 THROW 51001, N'Donanım için başka bir işlem devam ediyor. Tekrar deneyiniz.', 1;
DECLARE @id INT=0;
    IF NULLIF(LTRIM(RTRIM(@role_adi)),N'') IS NULL THROW 51002,N'Röle adı gereklidir.',1;
    IF NOT EXISTS(SELECT 1 FROM dbo.EthernetKartlari WHERE id=@ethernet_kart_id AND (aktif_mi=1 OR @aktif_mi=0))
        THROW 51002,N'Aktif bir Ethernet kartı seçiniz.',1;
    IF EXISTS(SELECT 1 FROM dbo.RoleKartlari WHERE ethernet_kart_id=@ethernet_kart_id AND id<>@id)
        THROW 51002,N'Bu Ethernet kartına başka bir röle kartı bağlı.',1;
    IF EXISTS(SELECT 1 FROM dbo.MakineRoleBaglantilari WHERE role_kart_id=@id AND aktif_mi=1)
       AND (@aktif_mi=0 OR EXISTS(SELECT 1 FROM dbo.RoleKartlari WHERE id=@id AND ethernet_kart_id<>@ethernet_kart_id))
        THROW 51002,N'Önce röle kartının aktif makine bağlantılarını kaldırınız.',1;

    INSERT dbo.RoleKartlari (role_adi,ethernet_kart_id,aktif_mi,ekleyen_id,ekleyen_ip) VALUES (@role_adi,@ethernet_kart_id,@aktif_mi,@ekleyen_id,@ekleyen_ip);
    COMMIT;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK;
        THROW;
    END CATCH
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_RoleKartlari_GUNCELLE
    @id INT,
    @role_adi NVARCHAR(100),
    @ethernet_kart_id INT,
    @aktif_mi BIT, @guncelleyen_id INT, @guncelleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
    BEGIN TRANSACTION;
    DECLARE @kilit INT;
    EXEC @kilit=sys.sp_getapplock @Resource=N'ModbusDonanimAyar', @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=0;
    IF @kilit<0 THROW 51001, N'Donanım için başka bir işlem devam ediyor. Tekrar deneyiniz.', 1;
IF NOT EXISTS(SELECT 1 FROM dbo.RoleKartlari WHERE id=@id) THROW 51002,N'Kayıt bulunamadı.',1;
    IF NULLIF(LTRIM(RTRIM(@role_adi)),N'') IS NULL THROW 51002,N'Röle adı gereklidir.',1;
    IF NOT EXISTS(SELECT 1 FROM dbo.EthernetKartlari WHERE id=@ethernet_kart_id AND (aktif_mi=1 OR @aktif_mi=0))
        THROW 51002,N'Aktif bir Ethernet kartı seçiniz.',1;
    IF EXISTS(SELECT 1 FROM dbo.RoleKartlari WHERE ethernet_kart_id=@ethernet_kart_id AND id<>@id)
        THROW 51002,N'Bu Ethernet kartına başka bir röle kartı bağlı.',1;
    IF EXISTS(SELECT 1 FROM dbo.MakineRoleBaglantilari WHERE role_kart_id=@id AND aktif_mi=1)
       AND (@aktif_mi=0 OR EXISTS(SELECT 1 FROM dbo.RoleKartlari WHERE id=@id AND ethernet_kart_id<>@ethernet_kart_id))
        THROW 51002,N'Önce röle kartının aktif makine bağlantılarını kaldırınız.',1;

    UPDATE dbo.RoleKartlari SET role_adi=@role_adi,ethernet_kart_id=@ethernet_kart_id,aktif_mi=@aktif_mi,guncelleyen_id=@guncelleyen_id,guncelleyen_ip=@guncelleyen_ip,guncellenme_tarih=GETDATE() WHERE id=@id;
    COMMIT;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK;
        THROW;
    END CATCH
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_RoleKartlari_DOLDUR @id INT
AS BEGIN SET NOCOUNT ON; SELECT R.*, E.kart_adi, E.ip+':'+CONVERT(VARCHAR(5),E.http_port) AS adres, (SELECT COUNT(*) FROM dbo.MakineRoleBaglantilari B WHERE B.role_kart_id=R.id AND B.aktif_mi=1) AS dolu_kanal FROM dbo.RoleKartlari R JOIN dbo.EthernetKartlari E ON E.id=R.ethernet_kart_id WHERE R.id=@id; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_RoleKartlari_TUMUNU_GETIR 
AS BEGIN SET NOCOUNT ON; SELECT R.*, E.kart_adi, E.ip+':'+CONVERT(VARCHAR(5),E.http_port) AS adres, (SELECT COUNT(*) FROM dbo.MakineRoleBaglantilari B WHERE B.role_kart_id=R.id AND B.aktif_mi=1) AS dolu_kanal FROM dbo.RoleKartlari R JOIN dbo.EthernetKartlari E ON E.id=R.ethernet_kart_id ORDER BY R.id; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_RoleKartlari_SIL @id INT
AS BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @kilit INT;
    EXEC @kilit=sys.sp_getapplock @Resource=N'ModbusDonanimAyar', @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=0;
    IF @kilit<0 THROW 51001, N'Donanım için başka bir işlem devam ediyor. Tekrar deneyiniz.', 1;

 IF EXISTS(SELECT 1 FROM dbo.MakineRoleBaglantilari WHERE role_kart_id=@id) THROW 51002,N'Makine bağlantısı olan röle kartı silinemez.',1;
 DELETE FROM dbo.RoleKartlari WHERE id=@id;
 IF @@ROWCOUNT=0 THROW 51002,N'Kayıt bulunamadı.',1;
 COMMIT;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK; THROW; END CATCH
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_MakineRoleBaglantilari_EKLE
    @role_kart_id INT,
    @kanal_no INT,
    @makine_id INT,
    @aktif_mi BIT, @ekleyen_id INT, @ekleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
    BEGIN TRANSACTION;
    DECLARE @kilit INT;
    EXEC @kilit=sys.sp_getapplock @Resource=N'ModbusDonanimAyar', @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=0;
    IF @kilit<0 THROW 51001, N'Donanım için başka bir işlem devam ediyor. Tekrar deneyiniz.', 1;
DECLARE @id INT=0;
    IF @kanal_no NOT BETWEEN 1 AND 16 THROW 51002,N'Röle kanalı 1 ile 16 arasında olmalıdır.',1;
    IF NOT EXISTS(SELECT 1 FROM dbo.Makineler WHERE id=@makine_id AND (aktif_mi=1 OR @aktif_mi=0))
        THROW 51002,N'Aktif bir makine seçiniz.',1;
    IF NOT EXISTS(SELECT 1 FROM dbo.RoleKartlari R JOIN dbo.EthernetKartlari E ON E.id=R.ethernet_kart_id
        WHERE R.id=@role_kart_id AND ((R.aktif_mi=1 AND E.aktif_mi=1) OR @aktif_mi=0))
        THROW 51002,N'Aktif bir röle ve Ethernet kartı seçiniz.',1;
    IF @aktif_mi=1 AND EXISTS(SELECT 1 FROM dbo.MakineRoleBaglantilari WHERE id<>@id AND aktif_mi=1 AND (makine_id=@makine_id OR (role_kart_id=@role_kart_id AND kanal_no=@kanal_no)))
        THROW 51002,N'Makine veya röle kanalı başka bir aktif bağlantıda kullanılıyor.',1;
    IF EXISTS(SELECT 1 FROM dbo.MakineLoglari L WHERE L.makine_id IN
       (SELECT makine_id FROM dbo.MakineRoleBaglantilari WHERE id=@id UNION SELECT @makine_id)
       AND L.devam_ediyor_mu=1 AND L.basarili_mi=1 AND L.aktif_mi=1 AND L.islem_tipi=N'DURDUR')
        THROW 51002,N'Açık duruş kaydı varken bağlantı değiştirilemez. Önce mevcut bağlantı üzerinden duruşu sonlandırınız.',1;

    INSERT dbo.MakineRoleBaglantilari (role_kart_id,kanal_no,makine_id,aktif_mi,ekleyen_id,ekleyen_ip) VALUES (@role_kart_id,@kanal_no,@makine_id,@aktif_mi,@ekleyen_id,@ekleyen_ip);
    COMMIT;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK;
        THROW;
    END CATCH
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_MakineRoleBaglantilari_GUNCELLE
    @id INT,
    @role_kart_id INT,
    @kanal_no INT,
    @makine_id INT,
    @aktif_mi BIT, @guncelleyen_id INT, @guncelleyen_ip NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
    BEGIN TRANSACTION;
    DECLARE @kilit INT;
    EXEC @kilit=sys.sp_getapplock @Resource=N'ModbusDonanimAyar', @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=0;
    IF @kilit<0 THROW 51001, N'Donanım için başka bir işlem devam ediyor. Tekrar deneyiniz.', 1;
IF NOT EXISTS(SELECT 1 FROM dbo.MakineRoleBaglantilari WHERE id=@id) THROW 51002,N'Kayıt bulunamadı.',1;
    IF @kanal_no NOT BETWEEN 1 AND 16 THROW 51002,N'Röle kanalı 1 ile 16 arasında olmalıdır.',1;
    IF NOT EXISTS(SELECT 1 FROM dbo.Makineler WHERE id=@makine_id AND (aktif_mi=1 OR @aktif_mi=0))
        THROW 51002,N'Aktif bir makine seçiniz.',1;
    IF NOT EXISTS(SELECT 1 FROM dbo.RoleKartlari R JOIN dbo.EthernetKartlari E ON E.id=R.ethernet_kart_id
        WHERE R.id=@role_kart_id AND ((R.aktif_mi=1 AND E.aktif_mi=1) OR @aktif_mi=0))
        THROW 51002,N'Aktif bir röle ve Ethernet kartı seçiniz.',1;
    IF @aktif_mi=1 AND EXISTS(SELECT 1 FROM dbo.MakineRoleBaglantilari WHERE id<>@id AND aktif_mi=1 AND (makine_id=@makine_id OR (role_kart_id=@role_kart_id AND kanal_no=@kanal_no)))
        THROW 51002,N'Makine veya röle kanalı başka bir aktif bağlantıda kullanılıyor.',1;
    IF EXISTS(SELECT 1 FROM dbo.MakineLoglari L WHERE L.makine_id IN
       (SELECT makine_id FROM dbo.MakineRoleBaglantilari WHERE id=@id UNION SELECT @makine_id)
       AND L.devam_ediyor_mu=1 AND L.basarili_mi=1 AND L.aktif_mi=1 AND L.islem_tipi=N'DURDUR')
        THROW 51002,N'Açık duruş kaydı varken bağlantı değiştirilemez. Önce mevcut bağlantı üzerinden duruşu sonlandırınız.',1;

    UPDATE dbo.MakineRoleBaglantilari SET role_kart_id=@role_kart_id,kanal_no=@kanal_no,makine_id=@makine_id,aktif_mi=@aktif_mi,guncelleyen_id=@guncelleyen_id,guncelleyen_ip=@guncelleyen_ip,guncellenme_tarih=GETDATE() WHERE id=@id;
    COMMIT;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK;
        THROW;
    END CATCH
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_MakineRoleBaglantilari_DOLDUR @id INT
AS BEGIN SET NOCOUNT ON; SELECT B.*, R.role_adi, E.kart_adi, E.ip, E.http_port, E.ip+':'+CONVERT(VARCHAR(5),E.http_port) AS adres, M.makine_no, M.makine_adi FROM dbo.MakineRoleBaglantilari B JOIN dbo.RoleKartlari R ON R.id=B.role_kart_id JOIN dbo.EthernetKartlari E ON E.id=R.ethernet_kart_id JOIN dbo.Makineler M ON M.id=B.makine_id WHERE B.id=@id; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_MakineRoleBaglantilari_TUMUNU_GETIR 
AS BEGIN SET NOCOUNT ON; SELECT B.*, R.role_adi, E.kart_adi, E.ip, E.http_port, E.ip+':'+CONVERT(VARCHAR(5),E.http_port) AS adres, M.makine_no, M.makine_adi FROM dbo.MakineRoleBaglantilari B JOIN dbo.RoleKartlari R ON R.id=B.role_kart_id JOIN dbo.EthernetKartlari E ON E.id=R.ethernet_kart_id JOIN dbo.Makineler M ON M.id=B.makine_id ORDER BY B.id; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_MakineRoleBaglantilari_SIL @id INT
AS BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @kilit INT;
    EXEC @kilit=sys.sp_getapplock @Resource=N'ModbusDonanimAyar', @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=0;
    IF @kilit<0 THROW 51001, N'Donanım için başka bir işlem devam ediyor. Tekrar deneyiniz.', 1;

 IF EXISTS(SELECT 1 FROM dbo.MakineRoleBaglantilari B JOIN dbo.MakineLoglari L ON L.makine_id=B.makine_id WHERE B.id=@id AND L.devam_ediyor_mu=1 AND L.basarili_mi=1 AND L.aktif_mi=1 AND L.islem_tipi=N'DURDUR') THROW 51002,N'Açık duruş kaydı varken bağlantı silinemez.',1;
 DELETE FROM dbo.MakineRoleBaglantilari WHERE id=@id;
 IF @@ROWCOUNT=0 THROW 51002,N'Kayıt bulunamadı.',1;
 COMMIT;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK; THROW; END CATCH
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_MakineRoleBaglantilari_KOMUT_GETIR @makine_id INT
AS BEGIN
 SET NOCOUNT ON;
 IF @@TRANCOUNT=0 THROW 51002,N'Röle komutu için transaction gereklidir.',1;
 DECLARE @kilit INT, @cihaz INT, @kaynak NVARCHAR(255);
 EXEC @kilit=sys.sp_getapplock @Resource=N'ModbusDonanimAyar',@LockMode='Shared',@LockOwner='Transaction',@LockTimeout=0;
 IF @kilit<0 THROW 51001,N'Donanım ayarları güncelleniyor. Tekrar deneyiniz.',1;
 SELECT @cihaz=R.ethernet_kart_id FROM dbo.MakineRoleBaglantilari B
 JOIN dbo.RoleKartlari R ON R.id=B.role_kart_id WHERE B.makine_id=@makine_id AND B.aktif_mi=1;
 IF @cihaz IS NULL RETURN;
 SET @kaynak=N'ModbusRoleCihaz:'+CONVERT(NVARCHAR(20),@cihaz);
 EXEC @kilit=sys.sp_getapplock @Resource=@kaynak,@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=0;
 IF @kilit<0 THROW 51001,N'Bu Ethernet kartında başka bir komut işleniyor. Tekrar deneyiniz.',1;
 SELECT B.*, E.ip, E.http_port, R.ethernet_kart_id
 FROM dbo.MakineRoleBaglantilari B JOIN dbo.RoleKartlari R ON R.id=B.role_kart_id
 JOIN dbo.EthernetKartlari E ON E.id=R.ethernet_kart_id JOIN dbo.Makineler M ON M.id=B.makine_id
 WHERE B.makine_id=@makine_id AND B.aktif_mi=1 AND R.aktif_mi=1 AND E.aktif_mi=1 AND M.aktif_mi=1;
END;
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
CREATE OR ALTER PROCEDURE dbo.SP_Makineler_DASHBOARD_GETIR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        M.id,
        M.makine_adi,
        M.makine_no,
        M.ip,
        M.mfg,
        M.sira_no,
        CAST(CASE WHEN B.id IS NOT NULL AND R.aktif_mi=1 AND E.aktif_mi=1 THEN 1 ELSE 0 END AS BIT) AS role_bagli_mi,
        R.role_adi, B.kanal_no, E.ip AS role_ip, E.http_port AS role_http_port,
        CAST(CASE WHEN ML.id IS NULL THEN 0 ELSE 1 END AS BIT) AS duruyor_mu,
        ML.islem_nedeni,
        ML.islem_baslangic_tarih,
        CASE
            WHEN ML.id IS NULL THEN 0
            ELSE DATEDIFF(MINUTE, ML.islem_baslangic_tarih, GETDATE())
        END AS durus_dakika
    FROM dbo.Makineler M
    LEFT JOIN dbo.MakineRoleBaglantilari B ON B.makine_id=M.id AND B.aktif_mi=1
    LEFT JOIN dbo.RoleKartlari R ON R.id=B.role_kart_id
    LEFT JOIN dbo.EthernetKartlari E ON E.id=R.ethernet_kart_id
    OUTER APPLY
    (
        SELECT TOP 1
            L.id,
            L.islem_nedeni,
            L.islem_baslangic_tarih
        FROM dbo.MakineLoglari L
        WHERE L.makine_id = M.id
          AND L.islem_tipi = N'DURDUR'
          AND L.devam_ediyor_mu = 1
          AND L.basarili_mi = 1
          AND L.aktif_mi = 1
        ORDER BY L.islem_baslangic_tarih DESC
    ) ML
    WHERE M.aktif_mi = 1
    ORDER BY M.sira_no;

    RETURN;
END

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
