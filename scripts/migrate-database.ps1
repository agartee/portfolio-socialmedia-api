[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(Mandatory = $false, HelpMessage = "Database connection string")]
  [Alias("c")]
  [string]$connectionString
)

$rootDir = (get-item $PSScriptRoot).Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$startupProjectFile = Join-Path -Path "$rootDir" -ChildPath $config.webApp.projectFile
$projectFile = Join-Path -Path "$rootDir" -ChildPath $config.dbMigrations.projectFile

# note: --context is onmy required when multiple DbContext are in the project
dotnet ef database update `
    --connection $connectionString `
    --startup-project $startupProjectFile `
    --project $projectFile
