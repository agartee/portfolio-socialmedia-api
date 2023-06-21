$minDotnetVersion = "7.0.0"
try {
  Invoke-Expression "dotnet --version" `
    -ErrorVariable errOut -OutVariable succOut 2>&1 | Out-Null

  if (-not $succOut) {
    throw [System.InvalidOperationException] `
      ".NET CLI installation not found (minimum: $($minDotnetVersion))."
  }

  $currentDotnetVersion = $succOut[0]
  if ([System.Version] $currentDotnetVersion -lt [System.Version] $minDotnetVersion) {
    throw [System.InvalidOperationException] `
    ("Current .NET version not supported (found: $($currentDotnetVersion);" `
        + " minimum: $($minDotnetVersion)).")
  }

  Write-Host (".NET installation found: $($currentDotnetVersion) (minimum:" `
      + "$($minDotnetVersion)).") -ForegroundColor Green
}
catch [System.Exception] {
  Write-Host $_.Exception.Message -ForegroundColor Red
  exit
}
