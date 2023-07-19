[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(ParameterSetName = "local", HelpMessage = "Bootstrap for local development.")]
  [Alias("l")]
  [switch]$local,
  [Parameter(ParameterSetName = "ci", HelpMessage = "Bootstrap for continuous integration server.")]
  [switch]$ci
)

$rootDir = (get-item $PSScriptRoot).Parent.FullName

. "$rootDir\scripts\support\check-dotnet.ps1"
. "$rootDir\scripts\support\check-ccr.ps1"

if ($PSCmdlet.ParameterSetName -eq "default" -or $local) {
  . "$rootDir\scripts\support\check-docker.ps1"
  . "$rootDir\scripts\support\check-ssl.ps1" 
  . "$rootDir\scripts\support\create-dotenv.ps1"
}
