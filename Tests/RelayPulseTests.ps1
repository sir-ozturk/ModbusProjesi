$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$testDirectory = Join-Path ([IO.Path]::GetTempPath()) ('ModbusPulseTest-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $testDirectory | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'BusinessLayer/bin/Debug/BusinessLayer.dll') -Destination $testDirectory
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework/v4.0.30319/csc.exe'
$exe = Join-Path $testDirectory 'RelayPulseTests.exe'
& $compiler /nologo /target:exe "/out:$exe" "/reference:$testDirectory/BusinessLayer.dll" /reference:System.Core.dll (Join-Path $PSScriptRoot 'RelayPulseTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'Pulse tests could not compile.' }
& $exe
if ($LASTEXITCODE -ne 0) { throw 'Pulse tests failed.' }
