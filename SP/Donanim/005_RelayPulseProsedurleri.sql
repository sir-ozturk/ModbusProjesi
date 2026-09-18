-- Proje kökünden SQLCMD ile çalıştırılır. Önce 003 ve 004 uygulanmalıdır.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
GO
:r "SP\SP_RelayPulses\SP_RelayPulses_EKLE.sql"
GO
:r "SP\SP_RelayPulses\SP_RelayPulses_GUNCELLE.sql"
GO
:r "SP\SP_RelayPulses\SP_RelayPulses_DOLDUR.sql"
GO
:r "SP\SP_RelayPulses\SP_RelayPulses_SURESI_DOLANLARI_GETIR.sql"
GO
:r "SP\SP_RelayPulses\SP_RelayPulses_AKTIF_KAYIT_VAR_MI.sql"
GO
:r "SP\SP_RelayPulses\SP_RelayPulses_UYARILARI_GETIR.sql"
GO
:r "SP\SP_RelayPulses\SP_RelayPulses_CIHAZ_KILIDI_AL.sql"
GO
:r "SP\SP_RelayPulseEvents\SP_RelayPulseEvents_EKLE.sql"
GO
COMMIT;

