param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Release"
)

$rootDir = (get-item $PSScriptRoot).Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\scripts.json" | ConvertFrom-Json
$projectFile = Join-Path -Path $rootDir -ChildPath $config.webAppProjectFile
$publishDir = "$rootDir\.publish"

if (Test-Path -Path $publishDir) {
  Remove-Item -Recurse -Force $publishDir
}

dotnet publish $projectFile --configuration "$configuration" `
  --no-restore --no-build --output "$publishDir" /p:UseAppHost=false

Write-Host ""
Write-Host "Publish directory contents:" -ForegroundColor Blue
Get-ChildItem "$publishDir"
