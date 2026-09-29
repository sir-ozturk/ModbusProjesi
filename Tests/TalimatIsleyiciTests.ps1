$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskDirectory = Join-Path ([IO.Path]::GetTempPath()) ('ModbusTalimatTest-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $taskDirectory | Out-Null
Copy-Item -LiteralPath (Join-Path $taskRoot 'BusinessLayer/bin/Debug/BusinessLayer.dll') -Destination $taskDirectory
$taskCompiler = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/2019/Community/MSBuild/Current/Bin/Roslyn/csc.exe'
if (!(Test-Path -LiteralPath $taskCompiler)) { throw 'Visual Studio 2019 C# compiler not found.' }
$taskExe = Join-Path $taskDirectory 'TalimatIsleyiciTests.exe'
& $taskCompiler /nologo /target:exe "/out:$taskExe" "/reference:$taskDirectory/BusinessLayer.dll" /reference:System.Core.dll /reference:System.Data.dll /reference:System.Xml.dll (Join-Path $PSScriptRoot 'TalimatIsleyiciTests.cs') (Join-Path $taskRoot 'MakineDurdurmaUygulamasi/MakineDurdurmaUygulamasi/TalimatIsleyici.cs')
if ($LASTEXITCODE -ne 0) { throw 'Talimat tests could not compile.' }
& $taskExe
if ($LASTEXITCODE -ne 0) { throw 'Talimat tests failed.' }
