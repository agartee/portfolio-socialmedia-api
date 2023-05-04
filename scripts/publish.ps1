param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [string]$configuration = "Release"
)

$rootDir = (get-item $PSScriptRoot).Parent.FullName

$projectFile = "$rootDir\src\SocialMedia.WebAPI\SocialMedia.WebAPI.csproj"
$publishDir = "$($rootDir)\.publish"

if (Test-Path -Path $publishDir) {
  Remove-Item -Recurse -Force $publishDir
}

dotnet publish "$projectFile" --configuration $configuration `
  --no-restore --no-build --output "$publishDir" /p:UseAppHost=false

Write-Host ""
Write-Host "Publish directory contents:" -ForegroundColor Blue
Get-ChildItem $publishDir
