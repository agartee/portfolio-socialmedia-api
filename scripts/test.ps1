Param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [string]$configuration = "Debug"
)

$exclusions = @{
  "SocialMedia.Domain"                = @(
    "SocialMedia.Domain.Models.*")

  "SocialMedia.Persistence.Auth0"     = @(
    "SocialMedia.Persistence.Auth0.Configuration.*"
    "SocialMedia.Persistence.Auth0.Models.*")

  "SocialMedia.Persistence.SqlServer" = @(
    "SocialMedia.Persistence.SqlServer.Migrations.*"
    "SocialMedia.Persistence.SqlServer.Models.*")

  "SocialMedia.WebAPI"                = @(
    "Program",
    "SocialMedia.WebAPI.Configuration.*",
    "SocialMedia.WebAPI.Formatters.*"
  )
}

$rootDir = (get-item $PSScriptRoot).Parent.FullName
$testProjects = Get-ChildItem -Path $rootDir\test -Filter *.csproj -Recurse -File | ForEach-Object { $_ }
$coverageDir = "$rootDir\.test-coverage"
$binDir = "$rootDir\.bin"
$status = 0

if (Test-Path $coverageDir) {
  Remove-Item $coverageDir -Recurse -Force
}

foreach ($testProject in $testProjects) {
  $projectName = ($testProject.Basename -replace ".Tests", "")

  if ($exclusions.ContainsKey($projectName)) {
    $exclude = ($exclusions[$projectName] | ForEach-Object { "[$projectName]" + $_ }) -join ","
  }

  Write-Host "Executing tests for $($testProject.Name)..." -ForegroundColor Blue

  dotnet test $testProject.FullName --no-build -c $configuration `
    --results-directory $coverageDir `
    --collect:"XPlat Code Coverage" `
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude="$exclude"

  if ($status -eq 0) {
    $status = $LASTEXITCODE
  }

  $coverageFile = Get-ChildItem -Path $coverageDir -Filter "coverage.cobertura.xml" `
    -Recurse -File | Sort-Object -Property LastWriteTime -Descending | Select-Object -First 1

  & $binDir\ccr.exe --coverage-file $coverageFile.FullName --package $projectName

  if ($exclude) {
    Write-Host "Coverage Exclusions:" -ForegroundColor Blue
    $exclusions[$projectName] | ForEach-Object { Write-Host "  $_" }
    Write-Host
  }
}

exit $status
