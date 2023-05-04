$rootDir = (get-item $PSScriptRoot).Parent.FullName

# **************************************************************************************
# Delete Bin Directory (External Tools)
# **************************************************************************************
$binDir = "$($rootDir)\.bin"

if (Test-Path -Path $binDir) {
  Remove-Item -Recurse -Force $binDir
}

# **************************************************************************************
# Delete Publish Output Directory
# **************************************************************************************
$publishDir = "$($rootDir)\.publish"

if (Test-Path -Path $publishDir) {
  Remove-Item -Recurse -Force $publishDir
}

# **************************************************************************************
# Delete Test-Coverage Directory
# **************************************************************************************
$testCoverageDir = "$($rootDir)\.test-coverage"

if (Test-Path -Path $testCoverageDir) {
  Remove-Item -Recurse -Force $testCoverageDir
}

# **************************************************************************************
# Delete SSL Directory
# **************************************************************************************
$sslDir = "$($rootDir)\.ssl"

if (Test-Path -Path $sslDir) {
  Remove-Item -Recurse -Force $sslDir
}

# **************************************************************************************
# Bootstrap
# **************************************************************************************
& "$($rootDir)\scripts\bootstrap.ps1"
