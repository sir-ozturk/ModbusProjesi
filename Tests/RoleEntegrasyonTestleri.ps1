$ErrorActionPreference = 'Stop'
$projeDizini = Split-Path $PSScriptRoot -Parent
$testDizini = Join-Path ([IO.Path]::GetTempPath()) ('ModbusRoleTest-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $testDizini | Out-Null
Copy-Item -LiteralPath (Join-Path $projeDizini 'BusinessLayer\bin\Debug\BusinessLayer.dll') -Destination $testDizini
$derleyici = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
$testExe = Join-Path $testDizini 'RoleEntegrasyonTestleri.exe'
& $derleyici /nologo /target:exe "/out:$testExe" "/reference:$testDizini\BusinessLayer.dll" /reference:System.Configuration.dll /reference:System.Net.Http.dll (Join-Path $PSScriptRoot 'RoleEntegrasyonTestleri.cs')
if ($LASTEXITCODE -ne 0) { throw 'Test derlenemedi.' }
& $testExe
if ($LASTEXITCODE -ne 0) { throw 'Role entegrasyon testi basarisiz.' }
