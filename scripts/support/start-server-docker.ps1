param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [string]$configuration = "Debug"
)

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$imageName = "socialmedia-api"
$tagName = "dev"
$containerName = "socialmedia-api"
$userSecretsId = "agartee-socialmedia"

docker container rm "$containerName" --force 2>&1 | Out-Null
. "$rootDir\scripts\build.ps1" -docker -configuration $configuration

try {
  # note: the Docker container won't have access to the user's certificate store
  # so a PFX certificate file must be mounted.
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
    --volume "$($secretsDir):/root/.microsoft/usersecrets/$($userSecretsId):ro" `
    --volume "$($pfxDir):/https:ro" `
    --detach `
    "$($imageName):$($tagName)"
}
catch [System.Exception] {
  Write-Host $_.Exception.Message -ForegroundColor Red
  exit
}
