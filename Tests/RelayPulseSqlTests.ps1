param([string]$Sunucu='NEDEN10',[string]$Veritabani='DB_MODBUS_DonanimTest_Pulse_20260918')
$ErrorActionPreference='Stop'
if ($Veritabani -notmatch '^DB_MODBUS_DonanimTest_Pulse_[a-zA-Z0-9_]+$') { throw 'Ayrı pulse test veritabanı gereklidir.' }
$root = Split-Path $PSScriptRoot -Parent
Push-Location $root
try {
    # Saat dönüşümü ve prosedür kurulumunun tekrar çalıştırılabilirliği doğrulanır.
    foreach ($pass in 1..2) {
        foreach ($migration in @('003_RelayPulseGuvenligi.sql','004_RelayPulseTurkiyeSaati.sql','005_RelayPulseProsedurleri.sql')) {
            & sqlcmd -S $Sunucu -d $Veritabani -E -C -b -i (Join-Path $root ('SP/Donanim/'+$migration))
            if ($LASTEXITCODE -ne 0) { throw ('Test şeması uygulanamadı: '+$migration) }
        }
    }
} finally { Pop-Location }
$testDirectory = Join-Path ([IO.Path]::GetTempPath()) ('ModbusPulseSqlTest-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $testDirectory | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'BusinessLayer/bin/Debug/BusinessLayer.dll') -Destination $testDirectory
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework/v4.0.30319/csc.exe'
$exe = Join-Path $testDirectory 'RelayPulseSqlTests.exe'
& $compiler /nologo /target:exe "/out:$exe" "/reference:$testDirectory/BusinessLayer.dll" /reference:System.Core.dll /reference:System.Data.dll /reference:System.Configuration.dll (Join-Path $PSScriptRoot 'RelayPulseSqlTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'SQL pulse tests could not compile.' }
$builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
$builder['Data Source']=$Sunucu
$builder['Initial Catalog']=$Veritabani
$builder['Integrated Security']=$true
$builder['TrustServerCertificate']=$true
$builder['Connect Timeout']=3
$escaped=[Security.SecurityElement]::Escape($builder.ConnectionString)
[IO.File]::WriteAllText($exe+'.config', '<configuration><connectionStrings><add name="ModbusDb" connectionString="'+$escaped+'" /></connectionStrings></configuration>')
& $exe
if ($LASTEXITCODE -ne 0) { throw 'SQL pulse tests failed.' }
