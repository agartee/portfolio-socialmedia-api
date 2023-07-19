#!/usr/bin/env bash

minVer="7.0.0"

minMajorVer=$(echo $minVer | cut -d. -f1)
maxMajorVer=$(($(echo $minVer | cut -d. -f1) + 1))
maxVer="${maxMajorVer}.0.0"
currentVer=$(dotnet --version 2> /dev/null)

case "$(uname -s)" in
	Linux)
		RED="\e[31m"
		GREEN="\e[32m"
		NO_COLOR="\e[0m"
		;;
	Darwin)
		RED="\033[31m"
		GREEN="\033[32m"
		NO_COLOR="\033[m"
		;;
esac

if [ -z "$currentVer" ]
then 
    echo -e "${RED}.NET installation not found (minimum: ${minVer}).${NO_COLOR}"
    exit 1
fi

if $(dpkg --compare-versions "${currentVer}" "lt" "${minVer}") || $(dpkg --compare-versions "${currentVer}" "ge" "${maxVer}")
then 
    echo -e "${RED}Current .NET version not supported (found: ${currentVer}; required: ${minMajorVer}.x.x).${NO_COLOR}"
    exit 1
fi

echo -e "${GREEN}.NET installation found: ${currentVer} (found: ${currentVer}; required: ${minMajorVer}.x.x)${NO_COLOR}"
