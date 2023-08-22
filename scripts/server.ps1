[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(ParameterSetName = "local", HelpMessage = "Build application with local .NET CLI.")]
  [switch]$local,
  [Parameter(ParameterSetName = "docker", HelpMessage = "Build docker image using Docker build image and publish local image.")]
  [switch]$docker,
  [Alias("c")]
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [string]$configuration = "Debug"
)

$rootDir = (get-item $PSScriptRoot).Parent.FullName

if ($PSCmdlet.ParameterSetName -eq "default" -or $local) {
  & "$rootDir\scripts\support\start-server-local.ps1" -configuration "$configuration"
}

if ($docker) {
  & "$rootDir\scripts\build.ps1" -docker -configuration "$configuration"
  & "$rootDir\scripts\support\start-server-docker.ps1" -configuration "$configuration"
}
