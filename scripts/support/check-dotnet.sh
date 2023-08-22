#!/bin/bash

minVer="7.0.0"

minMajorVer=$(echo $minVer | cut -d. -f1)
maxMajorVer=$(($(echo $minVer | cut -d. -f1) + 1))
maxVer="${maxMajorVer}.0.0"

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

sdks=$(dotnet --list-sdks 2>&1)

if [[ ! $sdks ]]; then
  echo -e "${RED}.NET CLI installation not found (target: ${minMajorVer}.x; min: ${minVer}).${NO_COLOR}" >&2
  exit 1
fi

hasTargetVer=false
while read -r sdk; do
  currentVer=${sdk%% *}
	if $(dpkg --compare-versions "${currentVer}" "ge" "${minVer}") && $(dpkg --compare-versions "${currentVer}" "lt" "${maxVer}"); then
    hasTargetVer=true
    break
  fi
done <<< "$sdks"

if [[ $hasTargetVer = false ]]; then
  echo -e "${RED}Target .NET version not found: ${minMajorVer}.x (minimum: ${minVer}).${NO_COLOR}" >&2
  exit 1
fi

echo -e "${GREEN}.NET installation found: ${currentVer} (target: ${minMajorVer}.x; min: ${minVer})${NO_COLOR}"
