param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Debug"
)

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\scripts.json" | ConvertFrom-Json
$projectFile = Join-Path -Path $rootDir -ChildPath $config.webAppProjectFile

dotnet run --project $projectFile --launch-profile https --configuration $configuration
