Invoke-Expression "sqlpackage" -OutVariable succOut 2>&1 | Out-Null

if (-not $succOut) {
  dotnet tool install -g microsoft.sqlpackage
  exit
}

Write-Host ("SqlPackage installation found.") `
  -ForegroundColor Green
