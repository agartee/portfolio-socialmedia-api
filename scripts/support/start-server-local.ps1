param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [string]$configuration = "Debug"
)

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$projectFile = "$rootDir\src\SocialMedia.WebAPI\SocialMedia.WebAPI.csproj"

. "$rootDir\scripts\build.ps1" -configuration $configuration
dotnet run --project $projectFile --launch-profile https
