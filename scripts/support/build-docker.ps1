param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [string]$configuration = "Debug"
)

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$imageName = "socialmedia-api"
$tagName = "dev"

docker image build `
  --tag "$($imageName):$($tagName)" `
  --build-arg CONFIG=$configuration `
  "$rootDir"
