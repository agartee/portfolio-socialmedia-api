$ccrVersion = "v1.3.0"

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$binDir = "$rootDir\.bin"

if ((Test-Path $binDir\ccr.exe)) {
  Write-Host "Test coverage reporter tool found. Skipping download." -ForegroundColor Green
}
else {
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
