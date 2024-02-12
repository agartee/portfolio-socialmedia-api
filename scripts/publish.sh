#!/bin/bash

show_help() {
  cat << EOF

Initiates a publish of the project to the ".publish/" directory.

Usage: publish.ps1 [-configuration <value>]

Options:
-configuration|-c   Specifies the configuration name for the build. Common 
                    values are "Release" or "Debug". If not specified, the 
                    default is "Debug".
EOF
}

rootDir=$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)
config=$(cat "$rootDir/scripts/.project-settings.json")
projectFile="$rootDir/$(echo "$config" | jq -r '.webAppProjectFile')"
configuration="Release"
publishDir="$rootDir/.publish"

case "$(uname -s)" in
	Linux)
    RED="\e[31m"
    BLUE="\e[34m"
		NO_COLOR="\e[0m"
		;;
	Darwin)
    RED="\033[31m"
    BLUE="\033[34m"
		NO_COLOR="\033[m"
		;;
esac

while (( "$#" )); do
  case "$1" in
    -h|-help)
      show_help
      exit 0
      ;;
    -c|-configuration)
      if [ -n "$2" ] && [ ${2:0:1} != "-" ]; then
        configuration=$2
        shift 2
      else
        echo "${RED}Error: Argument for $1 is missing${NO_COLOR}" >&2
        exit 1
      fi
      ;;
    -*|--*=)
      echo "${RED}Error: Unsupported flag $1${NO_COLOR}" >&2
      exit 1
      ;;
    *) # preserve positional arguments
      PARAMS="$PARAMS $1"
      shift
      ;;
  esac
done

if [ -d "$publishDir" ]; then
  rm -rf "$publishDir"
fi

dotnet publish "$projectFile" --configuration "$configuration" \
  --no-restore --no-build --output "$publishDir" /p:UseAppHost=false

echo -e "\n${BLUE}Publish directory contents:${NO_COLOR}"
echo ""
ls -l "$publishDir"
