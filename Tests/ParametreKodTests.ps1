$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskDirectory = Join-Path ([IO.Path]::GetTempPath()) ('ModbusParametreTest-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $taskDirectory | Out-Null
$taskCompiler = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/2019/Community/MSBuild/Current/Bin/Roslyn/csc.exe'
$taskExe = Join-Path $taskDirectory 'ParametreKodTests.exe'
& $taskCompiler /nologo /target:exe "/out:$taskExe" /reference:System.Core.dll /reference:System.Data.dll /reference:System.Xml.dll (Join-Path $PSScriptRoot 'ParametreKodTests.cs') (Join-Path $taskRoot 'BusinessLayer/Work/ParametreKontrolleri.cs')
if ($LASTEXITCODE -ne 0) { throw 'Parametre testleri derlenemedi.' }
& $taskExe
if ($LASTEXITCODE -ne 0) { throw 'Parametre testleri başarısız.' }
