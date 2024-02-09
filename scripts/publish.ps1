param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g., Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Release",

  [Parameter(Mandatory = $false)]
  [Alias("h")]
  [switch]$help
)

if ($help) {
  Write-Output @"

Initiates a publish of the project to the ".publish/" directory.

Usage: publish.ps1 [-configuration <value>]

Options:
-configuration|-c   Specifies the configuration name for the build. Common 
                    values are "Release" or "Debug". If not specified, the 
                    default is "Debug".
"@ 
  exit
}

$rootDir = (get-item $PSScriptRoot).Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$projectFile = Join-Path -Path "$rootDir" -ChildPath $config.webApp.projectFile
$publishDir = "$rootDir\.publish"

if (Test-Path -Path "$publishDir") {
  Remove-Item -Recurse -Force "$publishDir"
}

dotnet publish $projectFile --configuration "$configuration" `
  --no-restore --no-build --output "$publishDir" /p:UseAppHost=false

Write-Host ""
Write-Host "Publish directory contents:" -ForegroundColor Blue
Get-ChildItem "$publishDir"
