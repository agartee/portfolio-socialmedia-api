[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(ParameterSetName = "local", HelpMessage = "Bootstrap for local development.")]
  [switch]$local,
  [Parameter(ParameterSetName = "ci", HelpMessage = "Bootstrap for continuous integration server.")]
  [switch]$ci
)

$rootDir = (get-item $PSScriptRoot).Parent.FullName

# **************************************************************************************
# Check .NET Installation
# **************************************************************************************
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

# **************************************************************************************
# Check Docker Installation
# **************************************************************************************
if ($PSCmdlet.ParameterSetName -eq "default" -or $local) {
  try {
    Invoke-Expression "docker --version" `
      -ErrorVariable errOut -OutVariable succOut 2>&1 | Out-Null

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
}

# **************************************************************************************
# Check Test Coverage Reporter Tool
# **************************************************************************************
$binDir = "$rootDir\.bin"

if ((Test-Path $binDir\ccr.exe)) {
  Write-Host "Test coverage reporter tool found. Skipping download." -ForegroundColor Green
}
else {
  $ccrVersion = "v1.2.0"
  $toolSrc = "https://github.com/agartee/cobertura-console-reporter/releases/download/$($ccrVersion)/ccr_windows_$($ccrVersion)_amd64.zip"
  $toolDest = "$binDir\ccr.zip"

  If (!(test-path -PathType container $binDir)) {
    New-Item -ItemType Directory -Path $binDir | Out-Null
  }

    (New-Object System.Net.WebClient).DownloadFile($toolSrc, $toolDest)
  Expand-Archive $toolDest -DestinationPath $binDir
  Remove-Item $toolDest

  Write-Host "Test coverage console tool downloaded to $binDir."  -ForegroundColor Green
}

# **************************************************************************************
# Reminder for Development SSL Certificate
# **************************************************************************************
if ($PSCmdlet.ParameterSetName -eq "default" -or $local) {
  $pfxPath = "$($rootDir)\.ssl\localhost.pfx"

  Write-Host ""
  Write-Host "To run the application over HTTPS (local or via Docker), ensure a" `
    "local dev certificate is installed:" -ForegroundColor Blue
  Write-Host "$ dotnet dev-certs https --check --trust" -ForegroundColor Magenta

  Write-Host ""

  Write-Host "To create a new trusted local dev certificate:" -ForegroundColor Blue
  Write-Host "$ dotnet dev-certs https" `
    "--export-path $pfxPath" `
    "--password <PASSWORD> --trust" `
    -ForegroundColor Magenta

  Write-Host ""
}
