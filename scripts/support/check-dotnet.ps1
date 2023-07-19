$minVer = "7.0.0"

$minMajorVer = $minVer.Split('.')[0]
$maxMajorVer = [int]$minMajorVer + 1
$maxVer = "$($maxMajorVer).0.0"

Invoke-Expression "dotnet --version" -OutVariable succOut 2>&1 | Out-Null

try {
  if (-not $succOut) {
    throw [System.InvalidOperationException] `
      ".NET CLI installation not found (minimum: $($minMajorVer).x.x)."
  }

  $currentVer = $succOut[0]
  if ([System.Version] $currentVer -lt [System.Version] $minVer `
      -or [System.Version] $currentVer -gt [System.Version] $maxVer) {
    
    throw [System.InvalidOperationException] `
    ("Current .NET version not supported (found: $($currentVer);" `
        + " required: $($minMajorVer).x.x).")
  }

  Write-Host (".NET installation found: $($currentVer)" `
      + " (required: $($minMajorVer).x.x).") -ForegroundColor Green
}
catch [System.Exception] {
  Write-Host $_.Exception.Message -ForegroundColor Red
  exit 1
}
