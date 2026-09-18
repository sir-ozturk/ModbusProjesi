-- Proje kökünden SQLCMD -b ile çalıştırın. Uygulama koduyla birlikte güncellenmelidir.
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
COMMIT TRANSACTION;
