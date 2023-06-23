param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Debug"
)

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\scripts.json" | ConvertFrom-Json
$solutionFile = Join-Path -Path $rootDir -ChildPath $config.solutionFile

dotnet build "$solutionFile" --configuration "$configuration"
