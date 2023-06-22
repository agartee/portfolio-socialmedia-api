#!/usr/bin/env bash

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
pfxPath="${rootDir}/.ssl/localhost.pfx"

case "$(uname -s)" in
	Linux)
		BLUE="\e[34m"
		MAGENTA="\e[35m"
		NO_COLOR="\e[0m"
		;;
	Darwin)
		BLUE="\033[34m"
		MAGENTA="\033[35m"
		NO_COLOR="\033[m"
		;;
esac

echo ""
echo -e "${BLUE}To run the application over HTTPS (local or via Docker), ensure a local dev certificate is installed:${NC}"
echo -e "${MAGENTA}dotnet dev-certs https --check --trust${NC}"
echo ""

echo -e "${BLUE}To create a new trusted local dev certificate:${NC}"
echo -e "${MAGENTA}dotnet dev-certs https --export-path ${pfxPath} --password \"<PASSWORD>\" --trust${NC}"
echo ""
