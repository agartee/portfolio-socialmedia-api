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
$testProjects = Get-ChildItem -Path $rootDir\test -Filter *.csproj -Recurse -File | ForEach-Object { $_ }
$coverageDir = "$rootDir\.test-coverage"
$binDir = "$rootDir\.bin"
$status = 0
$exclusions = @{}

foreach ($exclusion in $config.test.exclusions) {
  $exclusions[$exclusion.project] = @($exclusion.exclude)
}

if (Test-Path "$coverageDir") {
  Remove-Item "$coverageDir" -Recurse -Force
}

foreach ($testProject in $testProjects) {
  $projectName = ($testProject.Basename -replace ".Tests", "")

  if ($exclusions.ContainsKey($projectName)) {
    $exclude = ($exclusions[$projectName] | ForEach-Object { "[$projectName]" + $_ }) -join ","
  }

  Write-Host "Executing tests for $($testProject.Name)..." -ForegroundColor Blue

  $dotnetTestArgs = @(
    $testProject.FullName,
    "-c", $configuration
    "--results-directory", $coverageDir,
    "--collect", "XPlat Code Coverage"
  )

  if ($noBuild) {
    $dotnetTestArgs += "--no-build"
  }

  & dotnet test $dotnetTestArgs `
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude="$exclude"
  
  if ($status -eq 0) {
    $status = $LASTEXITCODE
  }

  $coverageFile = Get-ChildItem -Path "$coverageDir" -Filter "coverage.cobertura.xml" `
    -Recurse -File | Sort-Object -Property LastWriteTime -Descending | Select-Object -First 1

  & $binDir\ccr.exe --coverage-file $coverageFile.FullName --package $projectName

  if ($exclude) {
    Write-Host "Coverage Exclusions:" -ForegroundColor Blue
    $exclusions[$projectName] | ForEach-Object { Write-Host "  $_" }
    Write-Host
  }
}

exit $status
