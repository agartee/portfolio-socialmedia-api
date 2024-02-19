$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName

MSBuild.exe $rootDir\SocialMedia.sln /p:Configuration=Database
