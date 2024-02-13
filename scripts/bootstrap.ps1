[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(ParameterSetName = "local", HelpMessage = "Bootstrap for local development.")]
  [Alias("l")]
  [switch]$local,

  [Parameter(ParameterSetName = "ci", HelpMessage = "Bootstrap for continuous integration server.")]
  [switch]$ci,

  [Parameter(Mandatory = $false)]
  [Alias("h")]
  [switch]$help
)

if ($help) {
  Write-Output @"

Checks for applications dependencies needed to contribute to this application 
and initializes a .env file if none exists.

Usage: bootstrap.ps1 [[-local] | [-ci]]

Options:
-ci         Bootstraps for a continuous integration server.
-local|-l   Bootstraps for local development. This is the default script 
            behavior.
"@ 
  exit
}

$rootDir = (get-item $PSScriptRoot).Parent.FullName

. "$rootDir\scripts\support\check-dotnet.ps1"
. "$rootDir\scripts\support\check-sqlpackage.ps1"
. "$rootDir\scripts\support\check-ccr.ps1"

if ($PSCmdlet.ParameterSetName -eq "default" -or $local) {
  . "$rootDir\scripts\support\check-docker.ps1"
  . "$rootDir\scripts\support\check-trusted-ssl-cert.ps1" 
  . "$rootDir\scripts\support\create-dotenv.ps1"
}
