param([string]$Sunucu='NEDEN10',[string]$Veritabani='DB_MODBUS_DonanimTest_20260917_Prosedur')
$ErrorActionPreference='Stop'
if($Veritabani -notlike 'DB_MODBUS_DonanimTest_*'){throw 'Yalnızca ayrı test veritabanına izin verilir.'}
# İş kuralları C# katmanındadır. Bu yardımcı sadece SQL tablo kısıtlarını sınar.
$c=New-Object System.Data.SqlClient.SqlConnection "Data Source=$Sunucu;Initial Catalog=$Veritabani;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=5"
$c.Open()
$sayac=0
try {
    $senaryolar=@(
        @{Ad='Geçersiz HTTP portu'; Hata=547; SQL="INSERT dbo.EthernetKartlari(kart_adi,model,ip,http_port) VALUES(N'Test',N'HW-584','127.0.0.253',0);"},
        @{Ad='Tekrarlanan Ethernet adresi'; Hata=2627; SQL="INSERT dbo.EthernetKartlari(kart_adi,model,ip,http_port) SELECT TOP 1 kart_adi,model,ip,http_port FROM dbo.EthernetKartlari;"},
        @{Ad='Eksik Ethernet ilişkisi'; Hata=547; SQL="INSERT dbo.RoleKartlari(role_adi,ethernet_kart_id) VALUES(N'Test',-1);"},
        @{Ad='Ethernet başına tek röle kartı'; Hata=2627; SQL="INSERT dbo.RoleKartlari(role_adi,ethernet_kart_id) SELECT TOP 1 role_adi,ethernet_kart_id FROM dbo.RoleKartlari;"},
        @{Ad='Geçersiz röle kanalı'; Hata=547; SQL="INSERT dbo.MakineRoleBaglantilari(role_kart_id,kanal_no,makine_id,aktif_mi) SELECT TOP 1 role_kart_id,0,makine_id,0 FROM dbo.MakineRoleBaglantilari;"},
        @{Ad='Tekrarlanan aktif bağlantı'; Hata=2601; SQL="INSERT dbo.MakineRoleBaglantilari(role_kart_id,kanal_no,makine_id) SELECT TOP 1 role_kart_id,kanal_no,makine_id FROM dbo.MakineRoleBaglantilari WHERE aktif_mi=1;"}
    )
    foreach($test in $senaryolar) {
        $komut=$c.CreateCommand()
        $komut.CommandText='BEGIN TRAN;'+$test.SQL
        $hata=0
        try { $null=$komut.ExecuteNonQuery() }
        catch {
            $e=$_.Exception
            while($e.InnerException){$e=$e.InnerException}
            if($e -isnot [System.Data.SqlClient.SqlException]){throw}
            $hata=$e.Number
        }
        finally {
            $komut.CommandText='IF @@TRANCOUNT>0 ROLLBACK;'
            $null=$komut.ExecuteNonQuery()
            $komut.Dispose()
        }
        if($hata -ne $test.Hata){throw ($test.Ad+': Beklenen SQL kısıtı doğrulanamadı. Hata: '+$hata)}
        $sayac++
        Write-Output ('OK: '+$test.Ad)
    }
    Write-Output "$sayac SQL tablo kısıtı doğrulandı."
}
finally { $c.Dispose() }
