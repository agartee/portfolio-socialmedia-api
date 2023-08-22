param(
  [Parameter(Mandatory = $false, HelpMessage = "Configuration name (e.g. Release, Debug)")]
  [Alias("c")]
  [string]$configuration = "Debug"
)

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$config = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$imageName = $config.docker.imageName
$containerName = $config.docker.containerName
$tagName = $config.docker.tagName
$userSecretsId = $config.userSecretsId

docker container rm "$containerName" --force 2>&1 | Out-Null

$pfxPath = Get-Content "$rootDir\.env" | Select-String '^SSL_PFX_PATH=' `
| ForEach-Object { $_.ToString().Split('=')[1] }

$pfxDir = Split-Path -Path "$pfxPath" -Parent
$pfxFile = Split-Path -Path "$pfxPath" -Leaf

$pfxPassword = Get-Content "$rootDir\.env" | Select-String '^SSL_PFX_PASSWORD=' `
| ForEach-Object { $_.ToString().Split('=')[1] }

$secretsDir = "$env:APPDATA\Microsoft\UserSecrets\$userSecretsId"
  
docker container run `
  --name "$containerName" `
  --publish 5000:80 --publish 5001:443 `
  --env "ASPNETCORE_ENVIRONMENT=Development" `
  --env "ASPNETCORE_URLS=https://+:443;http://+:80" `
  --env "ASPNETCORE_Kestrel__Certificates__Default__Path=/https/$($pfxFile)" `
  --env "ASPNETCORE_Kestrel__Certificates__Default__Password=$($pfxPassword)" `
  --volume "$($secretsDir):/root/.microsoft/usersecrets/$($userSecretsId):ro" `
  --volume "$($pfxDir):/https:ro" `
  --detach `
  "$($imageName):$($tagName)"

if ($LASTEXITCODE -eq 0) {
  & "$rootDir\scripts\support\wait-for-healthy-container.ps1" -c $containerName
}
