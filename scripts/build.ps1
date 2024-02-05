[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(ParameterSetName = "local", HelpMessage = "Build application with local .NET CLI.")]
  [Alias("l")]
  [switch]$local,
  [Parameter(ParameterSetName = "docker", HelpMessage = "Build docker image using Docker build image and publish local image.")]
  [Alias("d")]
  [switch]$docker,
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g., Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Debug",
  [Parameter(Mandatory = $false, HelpMessage = "Assembly version (e.g., 1.2.3.4)")]
  [Alias("v")]
  [string]$version = "1.0.0"
)

$rootDir = (get-item $PSScriptRoot).Parent.FullName

if ($PSCmdlet.ParameterSetName -eq "default" -or $local) {
  & "$rootDir\scripts\support\build-local.ps1" `
    -configuration "$configuration" `
    -version "$version" `
}

if ($docker) {
  & "$rootDir\scripts\support\build-docker.ps1" `
    -configuration "$configuration" `
    -version "$version" `
}
