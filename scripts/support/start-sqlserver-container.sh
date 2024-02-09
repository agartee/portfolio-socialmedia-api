#!/bin/bash

if [ -z "$1" ]; then
    echo "Error: Docker container name is mandatory."
    exit 1
fi

containerName="$1"

runningContainer=$(docker ps --filter "name=sqlserver" --format "{{.Names}}")

if [ "$runningContainer" == "$containerName" ]; then
    echo "The '$containerName' container is already running."
    exit 0

imageName="mcr.microsoft.com/mssql/server"

rootDir=$(dirname $(dirname $(realpath $0)))
password=$(grep "^DB_PASSWORD=" "$rootDir/.env" | cut -d'=' -f2)

docker pull $imageName > /dev/null 2>&1
docker container rm $containerName --force > /dev/null 2>&1

echo "Starting SQL Server Docker container..."

docker run \
    -e "ACCEPT_EULA=Y" \
    -e "MSSQL_SA_PASSWORD=$password" \
    -p 1433:1433 --name $containerName -d \
    --health-cmd "/opt/mssql-tools/bin/sqlcmd -U sa -P $password -Q \"SELECT 1\"" \
    --health-interval=2s \
    $imageName
