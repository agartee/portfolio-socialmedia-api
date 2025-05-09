Param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g., Release, Debug)")]
  [string]$configuration = "Debug",

  [Parameter(Mandatory = $false, HelpMessage = "Do not perform build on projects before running tests")]
  [switch]$noBuild,

  [Parameter(Mandatory = $false)]
  [Alias("h")]
  [switch]$help
)

if ($help) {
  Write-Output @"

Runs tests based on the provided parameters and optional configuration.

Usage: test.ps1 [-configuration <value>] [-noBuild]

Options:
-configuration|-c       Specifies the configuration name for running the tests.
                        Common values are "Release" or "Debug". If not 
                        specified, the default is "Debug".
-noBuild                When set, the script will not perform a build on the 
                        projects before running the tests. Use this to run tests
                        on the previously built assemblies.
"@
  exit
}

$rootDir = (get-item $PSScriptRoot).Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$solutionFile = "$($config.solutionFile)"
$excludePatterns =  ($config.test.exclusions | ForEach-Object { $_.exclude } | ForEach-Object { $_ }) -join ","
$coverageDir = "$rootDir\.test-coverage"
$coverageReportDir = "$rootDir\.test-coverage-report"
$status = 0

if (Test-Path "$coverageDir") {
  Remove-Item "$coverageDir" -Recurse -Force
}

if (Test-Path "$coverageReportDir") {
  Remove-Item "$coverageReportDir" -Recurse -Force
}

$excludePatterns = """$excludePatterns"""
$dotnetTestArgs = @(
  $solutionFile,
  "-c", $configuration,
  "--results-directory", $coverageDir,
  "--collect", "XPlat Code Coverage",
  "/p:CollectCoverage=true",
  "/p:CoverletOutputFormat=cobertura",
  "/p:Exclude=$excludePatterns"
)

if ($noBuild) {
  $dotnetTestArgs += "--no-build"
}

dotnet test @dotnetTestArgs

if ($status -eq 0) {
  $status = $LASTEXITCODE
}

reportgenerator `
  -reports:$coverageDir/**/coverage.cobertura.xml `
  -targetdir:./.test-coverage-report

exit $status
