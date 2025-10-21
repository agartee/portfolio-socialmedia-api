$rootDir = (Get-Item $PSScriptRoot).Parent.Parent.FullName
$config  = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$dotEnvPath = Join-Path $rootDir ".env"

$dotEnvSpec = @()
if ($config.dotEnv) { $dotEnvSpec = @($config.dotEnv) }

function Read-DotEnv {
  param([string]$Path)

  $map = @{}
  $lines = @()

  if (-not (Test-Path -LiteralPath $Path)) {
    return @{ Map = $map; Lines = $lines }
  }

  $content = Get-Content -LiteralPath $Path -Raw -ErrorAction Stop
  $lines = $content -split "`r?`n"

  foreach ($line in $lines) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    if ($line.TrimStart().StartsWith("#"))   { continue }

    $idx = $line.IndexOf("=")
    if ($idx -lt 1) { continue }

    $key = $line.Substring(0, $idx).Trim()
    $val = $line.Substring($idx + 1)

    # Strip matching surrounding quotes
    if ($val.Length -ge 2 -and (
        ($val.StartsWith('"') -and $val.EndsWith('"')) -or
        ($val.StartsWith("'") -and $val.EndsWith("'"))
      )) {
      $val = $val.Substring(1, $val.Length - 2)
    }

    if (-not [string]::IsNullOrWhiteSpace($key)) {
      $map[$key] = $val
    }
  }

  return @{ Map = $map; Lines = $lines }
}

function Write-DotEnv {
  param(
    [hashtable]$Map,
    [string]$Path,
    [string[]]$ManagedKeys
  )

  $existing = @()
  $managedSet = New-Object 'System.Collections.Generic.HashSet[string]'
  foreach ($k in $ManagedKeys) { $null = $managedSet.Add($k) }

  if (Test-Path -LiteralPath $Path) {
    $existing = Get-Content -LiteralPath $Path -ErrorAction Stop
  }

  $output = New-Object System.Collections.Generic.List[string]
  $seen   = New-Object 'System.Collections.Generic.HashSet[string]'

  foreach ($line in $existing) {
    $trim = $line.Trim()
    if ($trim -eq "" -or $trim.StartsWith("#") -or ($line.IndexOf("=") -lt 1)) {
      $output.Add($line) | Out-Null
      continue
    }

    $idx = $line.IndexOf("=")
    $key = $line.Substring(0, $idx).Trim()

    if ($managedSet.Contains($key)) {
      if (-not $seen.Contains($key)) {
        $value = $Map[$key]
        $needsQuotes = ($value -match '\s|=') # quote if contains space or '='
        $formatted = if ($needsQuotes) { "$key=""$value""" } else { "$key=$value" }
        $output.Add($formatted) | Out-Null
        $null = $seen.Add($key)
      }
      # Skip original line for managed key
    } else {
      $output.Add($line) | Out-Null
    }
  }

  foreach ($k in $ManagedKeys) {
    if (-not $seen.Contains($k)) {
      $value = $Map[$k]
      $needsQuotes = ($value -match '\s|=')
      $formatted = if ($needsQuotes) { "$k=""$value""" } else { "$k=$value" }
      $output.Add($formatted) | Out-Null
    }
  }

  $dir = Split-Path -Parent $Path
  if ($dir -and -not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }

  [System.IO.File]::WriteAllText($Path, ($output -join [Environment]::NewLine))
}

function Prompt-ForMissingDotEnv {
  param(
    [hashtable]$ExistingMap,
    [object[]]$DotEnvSpec
  )

  $updated = $false

  foreach ($spec in $DotEnvSpec) {
    $key     = if ($spec.secret)  { [string]$spec.secret }  elseif ($spec.Secret)  { [string]$spec.Secret }  else { $null }
    $default = if ($spec.default) { [string]$spec.default } elseif ($spec.Default) { [string]$spec.Default } else { $null }
    if ([string]::IsNullOrWhiteSpace($key)) { continue }

    $hasValue = $ExistingMap.ContainsKey($key) -and (-not [string]::IsNullOrWhiteSpace($ExistingMap[$key]))
    if ($hasValue) { continue }

    $suffix = if ($null -ne $default -and $default -ne "") { " [default: $default]" } else { "" }
    $userInput = Read-Host -Prompt "Enter value for '.env' key '$key'$suffix"

    if ([string]::IsNullOrWhiteSpace($userInput) -and $null -ne $default) { $ExistingMap[$key] = $default }
    else { $ExistingMap[$key] = [string]$userInput }

    $updated = $true
  }

  return $updated
}

try {
  $envRead = Read-DotEnv -Path $dotEnvPath
  $envMap  = $envRead.Map

  $managedEnvKeys = @()
  foreach ($spec in $dotEnvSpec) {
    $k = if ($spec.secret) { [string]$spec.secret } elseif ($spec.Secret) { [string]$spec.Secret } else { $null }
    if (-not [string]::IsNullOrWhiteSpace($k)) { $managedEnvKeys += $k }
  }

  $changedEnv = Prompt-ForMissingDotEnv -ExistingMap $envMap -DotEnvSpec $dotEnvSpec

  if ($changedEnv -or -not (Test-Path -LiteralPath $dotEnvPath)) {
    Write-DotEnv -Map $envMap -Path $dotEnvPath -ManagedKeys $managedEnvKeys
    Write-Host ".env updated at $dotEnvPath"
  } else {
    Write-Host "All .env keys already present. No changes made."
  }
}
catch {
  Write-Error $_.Exception.Message
  exit 1
}
