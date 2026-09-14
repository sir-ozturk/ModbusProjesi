$ErrorActionPreference = 'Stop'
$projeDizini = Split-Path $PSScriptRoot -Parent
$ayar = [xml](Get-Content -LiteralPath (Join-Path $projeDizini 'ModbusProjesi/Web.config'))
$baglanti = New-Object System.Data.SqlClient.SqlConnection($ayar.configuration.connectionStrings.add.connectionString)
$islem = $null
try {
    $baglanti.Open()
    $islem = $baglanti.BeginTransaction()
    foreach ($dosya in @(
        'Migrations/DatabaseMigrations/Makineler_RELAY_CHANNEL_EKLE.sql',
        'SP/SP_Makineler/SP_Makineler_DOLDUR.sql',
        'SP/SP_Makineler/SP_Makineler_DASHBOARD_GETIR.sql',
        'SP/SP_MakineLoglari/MakineLoglari_ACIK_DURUS_INDEX_EKLE.sql'
    )) {
        $sql = Get-Content -LiteralPath (Join-Path $projeDizini $dosya) -Raw
        foreach ($parca in [regex]::Split($sql, '(?im)^\s*GO\s*$')) {
            if ([string]::IsNullOrWhiteSpace($parca)) { continue }
            $komut = $baglanti.CreateCommand()
            try {
                $komut.Transaction = $islem
                $komut.CommandText = $parca
                [void]$komut.ExecuteNonQuery()
            } finally { $komut.Dispose() }
        }
    }
    $islem.Commit()
    Write-Output 'Kanal alanı, makine sorguları ve açık duruş indeksi hazır.'
} catch {
    if ($null -ne $islem) { $islem.Rollback() }
    throw
} finally {
    if ($null -ne $islem) { $islem.Dispose() }
    $baglanti.Dispose()
}
