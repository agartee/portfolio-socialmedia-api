#!/usr/bin/env bash

rootDir=$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)

configuration="Release"
projectFile="$rootDir/src/SocialMedia.WebAPI/SocialMedia.WebAPI.csproj"
publishDir="$rootDir/.publish"

case "$(uname -s)" in
	Linux)
    BLUE='\e[34m'
		GREEN='\e[32m'
		NC='\e[0m'
		;;
	Darwin)
    BLUE='\033[34m'
		GREEN='\033[32m'
		NC='\033[m'
		;;
esac

while (( "$#" )); do
  case "$1" in
    -c|--configuration)
      if [ -n "$2" ] && [ ${2:0:1} != "-" ]; then
        configuration=$2
        shift 2
      else
        echo "Error: Argument for $1 is missing" >&2
        exit 1
      fi
      ;;
    *)
      echo "Unknown parameter passed: $1"
      exit 1
  esac
done

if [ -d "$publishDir" ]; then
  rm -rf "$publishDir"
fi

dotnet publish "$projectFile" --configuration "$configuration" \
  --no-restore --no-build --output "$publishDir" /p:UseAppHost=false

echo -e "\n${BLUE}Publish directory contents:${NC}"
echo ""
ls -l "$publishDir"
