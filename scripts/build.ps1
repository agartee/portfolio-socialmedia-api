[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(ParameterSetName = "local", HelpMessage = "Build application with local .NET CLI.")]
  [switch]$local,
  [Parameter(ParameterSetName = "docker", HelpMessage = "Build docker image using Docker build image and publish local image.")]
  [switch]$docker,
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [string]$configuration = "Debug"
)

$rootDir = (get-item $PSScriptRoot).Parent.FullName

# **************************************************************************************
# Local
# **************************************************************************************
if ($PSCmdlet.ParameterSetName -eq "default" -or $local) {
  $solutionFile = "$rootDir\SocialMedia.sln"

  dotnet build "$solutionFile" --configuration $configuration
}

# **************************************************************************************
# Docker
# **************************************************************************************
if ($docker) {
  $imageName = "socialmedia-api"
  $tagName = "dev"

  docker image build `
    --tag "$($imageName):$($tagName)" `
    --build-arg CONFIG=$configuration `
    "$rootDir"
}
