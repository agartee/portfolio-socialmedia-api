param(
  [Parameter(Mandatory = $false)]
  [Alias("h")]
  [switch]$help
)

if ($help) {
  Write-Output @"

Initiates a publish of the database DACPAC files to the ".publish-sql/" directory.

Usage: publish.ps1
"@ 
  exit
}

$rootDir = (get-item $PSScriptRoot).Parent.FullName
$publishDir = "$($rootDir)\.publish-sql"

if(-not (Test-Path -Path $publishDir)) {
    New-Item -ItemType Directory -Path $publishDir
}

Get-ChildItem -Path "$($rootDir)" -Filter *.dacpac -Recurse `
    | Where-Object { $_.DirectoryName -match "bin\\Database$" } `
    | Copy-Item -Destination $publishDir -Force
