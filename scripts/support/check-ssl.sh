#!/usr/bin/env bash

case "$(uname -s)" in
	Linux)
		BLUE='\e[34m'
		MAGENTA="\e[35m"
		NC='\e[0m'
		;;
	Darwin)
		BLUE='\033[34m'
		MAGENTA="\033[35m"
		NC='\033[m'
		;;
esac

rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
pfxPath="${rootDir}/.ssl/localhost.pfx"

echo ""
echo -e "${BLUE}To run the application over HTTPS (local or via Docker), ensure a local dev certificate is installed:${NC}"
echo -e "${MAGENTA}dotnet dev-certs https --check --trust${NC}"
echo ""

echo -e "${BLUE}To create a new trusted local dev certificate:${NC}"
echo -e "${MAGENTA}dotnet dev-certs https --export-path ${pfxPath} --password \"<PASSWORD>\" --trust${NC}"
echo ""
