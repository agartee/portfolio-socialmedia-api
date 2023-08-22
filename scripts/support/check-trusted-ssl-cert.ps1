$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$pfxPath = "$($rootDir)\.ssl\localhost.pfx"

Write-Host ""
Write-Host "To run the application over HTTPS (local or via Docker), ensure a" `
  "local dev certificate is installed:" -ForegroundColor Blue
Write-Host "dotnet dev-certs https --check --trust" -ForegroundColor Magenta

Write-Host ""

Write-Host "To create a new trusted local dev certificate:" -ForegroundColor Blue
Write-Host "dotnet dev-certs https" `
  "--export-path $pfxPath" `
  "--password ""<PASSWORD>"" --trust" `
  -ForegroundColor Magenta

Write-Host ""
