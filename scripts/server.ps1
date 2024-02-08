[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(ParameterSetName = "local", HelpMessage = "Build application with local .NET CLI.")]
  [switch]$local,

  [Parameter(ParameterSetName = "docker", HelpMessage = "Build docker image using Docker build image and publish local image.")]
  [switch]$docker,

  [Alias("c")]
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g., Release, Debug)")]
  [string]$configuration = "Debug",

  [Parameter(Mandatory = $false)]
  [Alias("h")]
  [switch]$help
)

if ($help) {
  Write-Output @"

Initiates a build of the project.

Usage: server.ps1 [[-local] | [docker]] [-configuration <value>]

Options:
-local|-l           Starts the application locally. This is the default script 
                    behavior.
-docker|-d          Builds the application Docker image and starts it up in a 
                    new container.
-configuration|-c   Specifies the configuration name for the build. Common 
                    values are "Release" or "Debug". If not specified, the 
                    default is "Debug".
"@
  exit
}

$rootDir = (get-item $PSScriptRoot).Parent.FullName

if ($PSCmdlet.ParameterSetName -eq "default" -or $local) {
  & "$rootDir\scripts\support\start-server-local.ps1" -configuration "$configuration"
}

if ($docker) {
  & "$rootDir\scripts\build.ps1" -docker -configuration "$configuration"
  & "$rootDir\scripts\support\start-server-docker.ps1" -configuration "$configuration"
}
