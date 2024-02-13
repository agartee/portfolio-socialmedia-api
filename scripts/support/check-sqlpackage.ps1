Invoke-Expression "sqlpackage" -OutVariable succOut 2>&1 | Out-Null

try {
  if (-not $succOut) {
    throw [System.InvalidOperationException] "SqlPackage installation not found."
  }

  Write-Host ("SqlPackage installation found.") `
    -ForegroundColor Green
}
catch [System.Exception] {
  Write-Host $_.Exception.Message -ForegroundColor Red
  exit 1
}
