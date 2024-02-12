[CmdletBinding()]
Param(
  [Parameter(Mandatory = $false)]
  [Alias("h")]
  [switch]$help
)

if ($help) {
  Write-Output @"

Cleans up temporary directories and files from the project.

Usage: clean.ps1
"@ 
  exit
}

$rootDir = (get-item $PSScriptRoot).Parent.FullName

$binDir = "$rootDir\.bin"
if (Test-Path -Path $binDir) {
  Write-Host "Removing $binDir..." -ForegroundColor Blue
  Remove-Item -Recurse -Force "$binDir"
}

$publishDir = "$rootDir\.publish"
if (Test-Path -Path $publishDir) {
  Write-Host "Removing $publishDir..." -ForegroundColor Blue
  Remove-Item -Recurse -Force "$publishDir"
}

$testCoverageDir = "$rootDir\.test-coverage"
if (Test-Path -Path $testCoverageDir) {
  Write-Host "Removing $testCoverageDir..." -ForegroundColor Blue
  Remove-Item -Recurse -Force "$testCoverageDir"
}

$sslDir = "$rootDir\.ssl"
if (Test-Path -Path $sslDir) {
  Write-Host "Removing $sslDir..." -ForegroundColor Blue
  Remove-Item -Recurse -Force "$sslDir"
}
