param([string]$Sunucu='NEDEN10',[string]$Veritabani='DB_MODBUS_DonanimTest_20260916')
$ErrorActionPreference='Stop'
if($Veritabani -notlike 'DB_MODBUS_DonanimTest_*'){throw 'Yalnızca ayrı test veritabanına izin verilir.'}
$cs="Data Source=$Sunucu;Initial Catalog=$Veritabani;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=5"
function TestBaglantisiAc { $c=New-Object System.Data.SqlClient.SqlConnection $cs; $c.Open(); return $c }
function SQL($c,[string]$s){$k=$c.CreateCommand();$k.CommandText=$s;$k.CommandTimeout=5;try{return $k.ExecuteScalar()}finally{$k.Dispose()}}
$c=TestBaglantisiAc
$script:sayac=0
function Test($c,$ad,$s,$beklenen){
 $hata=0
 try{$null=SQL $c ('BEGIN TRAN;'+$script:degiskenler+$s)}
 catch{$e=$_.Exception;while($e.InnerException){$e=$e.InnerException};if($e -isnot [System.Data.SqlClient.SqlException]){throw};$hata=$e.Number;if($hata -ne $beklenen){throw ($ad+': '+$e.Message)}}
 finally{$null=SQL $c 'IF @@TRANCOUNT>0 ROLLBACK;'}
 if($hata -ne $beklenen){throw ($ad+': Beklenen hata olusmadi.')}
 $script:sayac++;Write-Output ('OK: '+$ad)
}
try{
 $null=SQL $c "SET NOCOUNT ON; IF NOT EXISTS(SELECT 1 FROM dbo.EthernetKartlari WHERE ip='127.0.0.77') BEGIN INSERT dbo.EthernetKartlari(kart_adi,model,ip,http_port) VALUES(N'Test A',N'HW-584','127.0.0.77',8080),(N'Test B',N'HW-584','127.0.0.78',8080); INSERT dbo.RoleKartlari(role_adi,ethernet_kart_id) SELECT N'Test Role',id FROM dbo.EthernetKartlari WHERE ip IN('127.0.0.77','127.0.0.78'); DECLARE @s INT=(SELECT MAX(sira_no) FROM dbo.Makineler); INSERT dbo.Makineler(model_ad,entegrasyon_kod,gg_no,makine_no,makine_adi,sira_no,band_no,ip,mfg,aktif_mi,eklenme_tarih) VALUES(N'Test',N'Test',N'99991',N'99991',N'Test A',@s+1,N'Test',N'127.0.0.91',N'99991',1,GETDATE()),(N'Test',N'Test',N'99992',N'99992',N'Test B',@s+2,N'Test',N'127.0.0.92',N'99992',1,GETDATE()); END;"
 $script:degiskenler="SET NOCOUNT ON; DECLARE @e INT=(SELECT id FROM dbo.EthernetKartlari WHERE ip='127.0.0.77'); DECLARE @r INT=(SELECT id FROM dbo.RoleKartlari WHERE ethernet_kart_id=@e); DECLARE @r2 INT=(SELECT R.id FROM dbo.RoleKartlari R JOIN dbo.EthernetKartlari E ON E.id=R.ethernet_kart_id WHERE E.ip='127.0.0.78'); DECLARE @m INT=(SELECT id FROM dbo.Makineler WHERE makine_no=N'99991'); DECLARE @m2 INT=(SELECT id FROM dbo.Makineler WHERE makine_no=N'99992');"
 $ekle="EXEC dbo.SP_MakineRoleBaglantilari_EKLE @role_kart_id=@r,@kanal_no=1,@makine_id=@m,@aktif_mi=1,@ekleyen_id=0,@ekleyen_ip=N'127.0.0.1';"
 Test $c 'Kanal 1 atamasi' $ekle 0
 Test $c 'Kanal 16 atamasi' $ekle.Replace('@kanal_no=1,','@kanal_no=16,') 0
 Test $c 'Kanal 0 reddi' $ekle.Replace('@kanal_no=1,','@kanal_no=0,') 51002
 Test $c 'Kanal 17 reddi' $ekle.Replace('@kanal_no=1,','@kanal_no=17,') 51002
 Test $c 'Ayni kanal ikinci makineye atanamaz' ($ekle+$ekle.Replace('@makine_id=@m,','@makine_id=@m2,')) 51002
 Test $c 'Makine ikinci kartta atanamaz' ($ekle+$ekle.Replace('@role_kart_id=@r,','@role_kart_id=@r2,')) 51002
 Test $c 'SQL benzersiz kanal indeksi' ($ekle+'INSERT dbo.MakineRoleBaglantilari(role_kart_id,kanal_no,makine_id) VALUES(@r,1,@m2);') 2601
 Test $c 'Pasif makine reddi' ('UPDATE dbo.Makineler SET aktif_mi=0 WHERE id=@m;'+$ekle) 51002
 Test $c 'Pasif Ethernet reddi' ('UPDATE dbo.EthernetKartlari SET aktif_mi=0 WHERE id=@e;'+$ekle) 51002
 Test $c 'Bagli makine pasife alinamaz' ($ekle+'UPDATE dbo.Makineler SET aktif_mi=0 WHERE id=@m;') 51002
 Test $c 'Bagli makine silinemez' ($ekle+'DELETE dbo.Makineler WHERE id=@m;') 547
 Test $c 'Bagli role pasife alinamaz' ($ekle+"EXEC dbo.SP_RoleKartlari_GUNCELLE @id=@r,@role_adi=N'Test',@ethernet_kart_id=@e,@aktif_mi=0,@guncelleyen_id=0,@guncelleyen_ip=N'127.0.0.1';") 51002
 Test $c 'Ethernet adres tekrari' "EXEC dbo.SP_EthernetKartlari_EKLE @kart_adi=N'Test',@model=N'HW-584',@ip='127.0.0.77',@http_port=8080,@aktif_mi=1,@ekleyen_id=0,@ekleyen_ip=N'127.0.0.1';" 51002
 Test $c 'Ethernet ikinci role karti' "EXEC dbo.SP_RoleKartlari_EKLE @role_adi=N'Test',@ethernet_kart_id=@e,@aktif_mi=1,@ekleyen_id=0,@ekleyen_ip=N'127.0.0.1';" 51002
 $durus=$ekle+"INSERT dbo.MakineLoglari(makine_id,islem_tipi,islem_nedeni,islem_baslangic_tarih,devam_ediyor_mu,basarili_mi,aktif_mi,eklenme_tarih,ekleyen_id,ekleyen_ip) VALUES(@m,N'DURDUR',N'Test',GETDATE(),1,1,1,GETDATE(),(SELECT TOP 1 id FROM dbo.Kullanicilar),N'127.0.0.1'); DECLARE @b INT=(SELECT id FROM dbo.MakineRoleBaglantilari WHERE makine_id=@m AND aktif_mi=1);"
 Test $c 'Acik durusta baglanti degisemez' ($durus+"EXEC dbo.SP_MakineRoleBaglantilari_GUNCELLE @id=@b,@role_kart_id=@r,@kanal_no=2,@makine_id=@m,@aktif_mi=1,@guncelleyen_id=0,@guncelleyen_ip=N'127.0.0.1';") 51002
 Test $c 'Acik durusta baglanti silinemez' ($durus+'EXEC dbo.SP_MakineRoleBaglantilari_SIL @id=@b;') 51002
 $cihazGuncelle="EXEC dbo.SP_EthernetKartlari_GUNCELLE @id=@e,@kart_adi=N'Test',@model=N'HW-584',@ip='127.0.0.77',@http_port=8080,@aktif_mi=1,@guncelleyen_id=0,@guncelleyen_ip=N'127.0.0.1';"
 Test $c 'Acik durusta cihaz degisemez' ($durus+$cihazGuncelle) 51002
 $null=SQL $c ($script:degiskenler+$ekle+$ekle.Replace('@role_kart_id=@r,','@role_kart_id=@r2,').Replace('@makine_id=@m,','@makine_id=@m2,'))
 $c2=TestBaglantisiAc
 try{
  $null=SQL $c ('BEGIN TRAN;'+$script:degiskenler+'EXEC dbo.SP_MakineRoleBaglantilari_KOMUT_GETIR @makine_id=@m;')
  Test $c2 'Ayni cihaz komut kilidi' 'EXEC dbo.SP_MakineRoleBaglantilari_KOMUT_GETIR @makine_id=@m;' 51001
  Test $c2 'Farkli cihaz beklemez' 'EXEC dbo.SP_MakineRoleBaglantilari_KOMUT_GETIR @makine_id=@m2;' 0
  Test $c2 'Komut sirasinda ayar degisemez' $cihazGuncelle 51001
 }finally{$null=SQL $c 'IF @@TRANCOUNT>0 ROLLBACK;';$c2.Dispose()}
 # Fixture bağlantıları sonraki test çalıştırması için temizlenir; sadece test DB.
 $null=SQL $c ($script:degiskenler+'DELETE dbo.MakineRoleBaglantilari WHERE makine_id IN(@m,@m2);')
 Write-Output "$script:sayac SQL kontrolu basarili."
}finally{$c.Dispose()}
