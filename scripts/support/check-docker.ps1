Invoke-Expression "docker --version" `
  -OutVariable succOut 2>&1 | Out-Null

try {
  if (-not $succOut) {
    throw [System.InvalidOperationException] "Docker installation not found."
  }

  Write-Host ("Docker installation found: $($succOut -replace "Docker version ")") `
    -ForegroundColor Green
}
catch [System.Exception] {
  Write-Host "Warning: $($_.Exception.Message)" -ForegroundColor Yellow
  Write-Host "Some scripts have an additional '-docker' switch that cannot be" `
    "utilized until Docker is installed." -ForegroundColor Yellow
}
