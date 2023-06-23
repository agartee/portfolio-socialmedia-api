param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Debug"
)

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.settings.json" | ConvertFrom-Json
$imageName = $config.docker.imageName
$tagName = $config.docker.tagName

docker image build `
  --tag "$($imageName):$($tagName)" `
  --build-arg "CONFIG=$configuration" `
  "$rootDir"
