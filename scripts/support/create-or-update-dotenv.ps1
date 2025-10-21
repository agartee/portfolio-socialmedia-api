$rootDir   = (Get-Item $PSScriptRoot).Parent.Parent.FullName
$config  = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$dotEnvPath = Join-Path $rootDir ".env"

$dotEnvSpec = @()
if ($config.dotEnv) { $dotEnvSpec = @($config.dotEnv) }

function Get-DotEnvEntry {
  param([object]$Spec)

  $key     = if ($Spec.secret)  { [string]$Spec.secret }  elseif ($Spec.Secret)  { [string]$Spec.Secret }  else { $null }
  $default = if ($Spec.default) { [string]$Spec.default } elseif ($Spec.Default) { [string]$Spec.Default } else { $null }

  return @{ Key = $key; Default = $default }
}

function Test-NeedsQuotes {
  param([string]$Value)
  return ($Value -match '\s|=')
}

function Read-DotEnvFile {
  param([string]$Path)

  $map   = @{}
  $lines = @()

  if (-not (Test-Path -LiteralPath $Path)) {
    return @{ Map = $map; Lines = $lines }
  }

  $raw   = Get-Content -LiteralPath $Path -Raw -ErrorAction Stop
  $lines = $raw -split "`r?`n"

  foreach ($line in $lines) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    if ($line.TrimStart().StartsWith("#"))   { continue }

    $equalsIndex = $line.IndexOf("=")
    if ($equalsIndex -lt 1) { continue }

    $key = $line.Substring(0, $equalsIndex).Trim()
    $val = $line.Substring($equalsIndex + 1)

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

function Merge-DotEnvLines {
  param(
    [hashtable]$Map,
    [string[]]  $ExistingLines,
    [string[]]  $ManagedKeys
  )

  $managedSet = New-Object 'System.Collections.Generic.HashSet[string]'
  foreach ($k in $ManagedKeys) { $null = $managedSet.Add($k) }

  $output = New-Object System.Collections.Generic.List[string]
  $seen   = New-Object 'System.Collections.Generic.HashSet[string]'

  foreach ($line in $ExistingLines) {
    $trim = $line.Trim()

    # Pass-through comments/blank/malformed
    if ($trim -eq "" -or $trim.StartsWith("#") -or ($line.IndexOf("=") -lt 1)) {
      $output.Add($line) | Out-Null
      continue
    }

    $idx = $line.IndexOf("=")
    $key = $line.Substring(0, $idx).Trim()

    # Replace only managed keys; preserve others as-is
    if ($managedSet.Contains($key)) {
      if (-not $seen.Contains($key)) {
        $value       = $Map[$key]
        $needsQuotes = Test-NeedsQuotes -Value $value
        $formatted   = if ($needsQuotes) { "$key=""$value""" } else { "$key=$value" }
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
      $value       = $Map[$k]
      $needsQuotes = Test-NeedsQuotes -Value $value
      $formatted   = if ($needsQuotes) { "$k=""$value""" } else { "$k=$value" }
      $output.Add($formatted) | Out-Null
    }
  }

  return ,$output.ToArray()  # unary comma ensures array output
}

function Write-DotEnvFile {
  param([string[]]$Lines, [string]$Path)

  $dir = Split-Path -Parent $Path
  if ($dir -and -not (Test-Path -LiteralPath $dir)) {
    New-Item -ItemType Directory -Path $dir -Force | Out-Null
  }

  [System.IO.File]::WriteAllText($Path, ($Lines -join [Environment]::NewLine))
}

function Prompt-ForMissingDotEnv {
  param(
    [hashtable]$Map,
    [object[]]  $DotEnvSpec
  )

  $anyChanged = $false

  foreach ($spec in $DotEnvSpec) {
    $entry = Get-DotEnvEntry -Spec $spec
    $key   = $entry.Key
    $def   = $entry.Default

    if ([string]::IsNullOrWhiteSpace($key)) { continue }

    $alreadySet = $Map.ContainsKey($key) -and (-not [string]::IsNullOrWhiteSpace($Map[$key]))
    if ($alreadySet) { continue }

    $defaultHint = if ($def) { " [default: $def]" } else { "" }
    $userInput   = Read-Host -Prompt "Enter value for '.env' key '$key'$defaultHint"

    $valueToStore =
      if ([string]::IsNullOrWhiteSpace($userInput) -and $def) { $def }
      else { [string]$userInput }

    $Map[$key] = $valueToStore
    $anyChanged = $true
  }

  return $anyChanged
}

try {
  $envState = Read-DotEnvFile -Path $dotEnvPath
  $envMap   = $envState.Map
  $oldLines = $envState.Lines

  $managedKeys = @()
  foreach ($spec in $dotEnvSpec) {
    $entry = Get-DotEnvEntry -Spec $spec
    if (-not [string]::IsNullOrWhiteSpace($entry.Key)) {
      $managedKeys += $entry.Key
    }
  }

  $changed = Prompt-ForMissingDotEnv -Map $envMap -DotEnvSpec $dotEnvSpec

  if ($changed -or -not (Test-Path -LiteralPath $dotEnvPath)) {
    $newLines = Merge-DotEnvLines -Map $envMap -ExistingLines $oldLines -ManagedKeys $managedKeys
    Write-DotEnvFile -Lines $newLines -Path $dotEnvPath
  }
}
catch {
  Write-Error $_.Exception.Message
  exit 1
}
