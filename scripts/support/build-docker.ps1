param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g., Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Debug",
  [Parameter(Mandatory = $false, HelpMessage = "Assembly version (e.g., 1.2.3.4)")]
  [Alias("v")]
  [string]$version = "1.0.0"
)

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$imageName = $config.docker.imageName
$tagName = $config.docker.tagName

docker image build `
  --tag "$($imageName):$($tagName)" `
  --build-arg "CONFIG=$configuration" `
  --build-arg "VERSION=$version" `
  --build-arg "MSG=hello" `
  "$rootDir"
