param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [string]$configuration = "Debug"
)

$imageName = "socialmedia-api"
$tagName = "dev"

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName

docker image build `
  --tag "$($imageName):$($tagName)" `
  --build-arg CONFIG=$configuration `
  "$rootDir"
