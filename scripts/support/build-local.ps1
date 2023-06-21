param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [string]$configuration = "Debug"
)

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$solutionFile = "$rootDir\SocialMedia.sln"

dotnet build "$solutionFile" --configuration $configuration
