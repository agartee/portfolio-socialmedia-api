param(
    [Parameter(Mandatory = $true, HelpMessage = "Docker container name")]
    [string]$containerName
)

$runningContainer = docker ps --filter "name=$($containerName)" --format "{{.Names}}"
if ($runningContainer -eq $containerName) {
    Write-Output "The 'sqlserver' container is already running."
    exit
}

$imageName = "mcr.microsoft.com/mssql/server"

$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName
$password = Get-Content "$rootDir\.env" | Select-String "^DB_PASSWORD=" `
    | ForEach-Object { $_.ToString().Split('=')[1] }

docker pull $imageName 2>&1 | Out-Null
docker container rm $containerName --force 2>&1 | Out-Null

Write-Host "Starting SQL Server Docker container..." -ForegroundColor Blue

docker run `
    -e "ACCEPT_EULA=Y" `
    -e "MSSQL_SA_PASSWORD=$($password)" `
    -p 1433:1433 --name $containerName -d `
    --health-cmd "/opt/mssql-tools/bin/sqlcmd -U sa -P $($password) -Q \`"SELECT 1\`"" `
    --health-interval=2s `
    $imageName
