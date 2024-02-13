#!/bin/bash

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

if ! type -t sqlpackage &> /dev/null; then 
	dotnet tool install -g microsoft.sqlpackage
	echo -e "${GREEN}.NET SqlPackage installed.${NO_COLOR}"
    exit 1
fi

echo -e "${GREEN}SqlPackage installation found.${NO_COLOR}" 
