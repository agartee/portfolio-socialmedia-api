#!/usr/bin/env bash

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

case "$(uname -s)" in
	Linux)
		RED='\e[31m'
		GREEN='\e[32m'
		NC='\e[0m'
		;;
	Darwin)
		RED='\033[31m'
		GREEN='\033[32m'
		NC='\033[m'
		;;
esac

paramSet="local"

while (( "$#" )); do
  case "$1" in
    -l|--local)
      paramSet="local"
      shift
      ;;
    -c|--ci)
      paramSet="ci"
      shift
      ;;
    *)
      echo "Unknown parameter passed: $1"
      exit 1
  esac
done

bash $rootDir/scripts/support/check-dotnet.sh
bash $rootDir/scripts/support/check-ccr.sh

if [ "$paramSet" = "local" ]; then
  bash $rootDir/scripts/support/check-docker.sh
  bash $rootDir/scripts/support/check-ssl.sh
fi
