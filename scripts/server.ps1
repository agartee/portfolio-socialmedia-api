[CmdletBinding(DefaultParameterSetName = 'default')]
param(
  [Parameter(ParameterSetName = "local", HelpMessage = "Build application with local .NET CLI.")]
  [switch]$local,
  [Parameter(ParameterSetName = "docker", HelpMessage = "Build docker image using Docker build image and publish local image.")]
  [switch]$docker,
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [string]$configuration = "Release"
)

$rootDir = (get-item $PSScriptRoot).Parent.FullName

# **************************************************************************************
# Local
# **************************************************************************************
if ($PSCmdlet.ParameterSetName -eq "default" -or $local) {
  $projectFile = "$rootDir\src\SocialMedia.WebAPI\SocialMedia.WebAPI.csproj"

  . "$rootDir\scripts\build.ps1" -configuration $configuration
  dotnet run --project $projectFile --launch-profile https
}

# **************************************************************************************
# Docker
# **************************************************************************************
if ($docker) {
  $imageName = "socialmedia-api"
  $containerName = "socialmedia-api"

  docker container rm "$containerName" --force 2>&1 | Out-Null
  . "$rootDir\scripts\build.ps1" -docker -configuration $configuration

  try {
    # note: the Docker container won't have access to the user's certificate store
    # so the certificate's PFX file must be mounted.
    $pfxPath = Get-Content "$rootDir\.env" | Select-String '^SSL_PFX_PATH=' `
    | ForEach-Object { $_.ToString().Split('=')[1] }
    $pfxDir = Split-Path -Path $pfxPath -Parent
    $pfxFile = Split-Path -Path $pfxPath -Leaf
    $pfxPassword = Get-Content "$rootDir\.env" | Select-String '^SSL_PFX_PASSWORD=' `
    | ForEach-Object { $_.ToString().Split('=')[1] }

    # note: secrets dir mounted for access to dev connection strings, etc.
    $secretsDir = "$env:APPDATA\Microsoft\UserSecrets\$userSecretsId"

    docker container run `
      --name $containerName `
      --publish 5000:80 --publish 5001:443 `
      --env "ASPNETCORE_ENVIRONMENT=Development" `
      --env "ASPNETCORE_URLS=https://+:443;http://+:80" `
      --env "ASPNETCORE_Kestrel__Certificates__Default__Path=/https/$($pfxFile)" `
      --env "ASPNETCORE_Kestrel__Certificates__Default__Password=$($pfxPassword)" `
      --volume "$($secretsDir):/root/.microsoft/usersecrets/socialmedia:ro" `
      --volume "$($pfxDir):/https:ro" `
      --detach `
      $imageName
  }
  catch [System.Exception] {
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit
  }
}
