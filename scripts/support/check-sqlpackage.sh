#!/bin/bash

case "$(uname -s)" in
	Linux)
		GREEN="\e[32m"
		NO_COLOR="\e[0m"
		;;
	Darwin)
		GREEN="\033[32m"
		NO_COLOR="\033[m"
		;;
esac

if ! type -t sqlpackage &> /dev/null; then 
	dotnet tool install -g microsoft.sqlpackage
    exit 0
fi

echo -e "${GREEN}SqlPackage installation found.${NO_COLOR}" 
