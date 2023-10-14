#!/bin/bash

if [[ $EUID -ne 0 ]]; then
   echo "This script must be run as root (e.g., sudo ./scriptname.sh)" 
   exit 1
fi

declare preferencesContent="Package: *net*
Pin: origin packages.microsoft.com
Pin-Priority: 1001"

if ! grep -q 'Package: \*net\*' /etc/apt/preferences 2>/dev/null; then
    echo -e "\n$preferencesContent" >> /etc/apt/preferences
    echo "Content appended to /etc/apt/preferences successfully!"
else
    echo "Content already exists in /etc/apt/preferences. Skipping..."
fi

declare repo_version=$(if command -v lsb_release &> /dev/null; then lsb_release -r -s; else grep -oP '(?<=^VERSION_ID=).+' /etc/os-release | tr -d '"'; fi)

wget https://packages.microsoft.com/config/ubuntu/$repo_version/packages-microsoft-prod.deb -O packages-microsoft-prod.deb

dpkg -i packages-microsoft-prod.deb

rm packages-microsoft-prod.deb

apt update
