# Runs all database scripts in order with sqlcmd (Windows authentication).
# Usage:  .\setup.ps1                      (default instance: localhost)
#         .\setup.ps1 -Server ".\SQLEXPRESS"
param([string]$Server = "localhost")

$ErrorActionPreference = "Stop"
$scripts = "01_create_database.sql", "02_tables.sql", "03_stored_procedures.sql", "04_seed_data.sql"

foreach ($s in $scripts) {
    Write-Host "Running $s on $Server ..."
    sqlcmd -S $Server -E -b -i (Join-Path $PSScriptRoot $s)
    if ($LASTEXITCODE -ne 0) { throw "$s failed" }
}
Write-Host "Done. Database EmployeeDemo is ready."
