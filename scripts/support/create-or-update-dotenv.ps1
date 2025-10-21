$rootDir    = (Get-Item $PSScriptRoot).Parent.Parent.FullName
$config  = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json

$dotEnvPath = Join-Path $rootDir ".env"
$dotEnvSpec = @()
if ($config.dotEnv) { $dotEnvSpec = @($config.dotEnv) }

function Read-DotEnv {
  param([Parameter(Mandatory)][string]$DotEnvPath)

  $map   = @{}
  $lines = @()

  if (Test-Path -LiteralPath $DotEnvPath) {
    $raw   = Get-Content -LiteralPath $DotEnvPath -Raw -ErrorAction Stop
    $lines = $raw -split "`r?`n"

    foreach ($line in $lines) {
      if ([string]::IsNullOrWhiteSpace($line)) { continue }
      if ($line.TrimStart().StartsWith("#"))   { continue }

      $eq = $line.IndexOf("=")
      if ($eq -lt 1) { continue }

      $key = $line.Substring(0, $eq).Trim()
      $val = $line.Substring($eq + 1)

      # strip matching surrounding quotes
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
  }

  $obj = New-Object PSObject
  $obj | Add-Member NoteProperty Map   $map
  $obj | Add-Member NoteProperty Lines $lines
  $obj | Add-Member NoteProperty ManagedKeys @()
  return $obj
}

function Get-DotEnvEntry {
  param([object]$Spec)
  $key     = if ($Spec.secret)  { [string]$Spec.secret }  elseif ($Spec.Secret)  { [string]$Spec.Secret }  else { $null }
  $default = if ($Spec.default) { [string]$Spec.default } elseif ($Spec.Default) { [string]$Spec.Default } else { $null }
  return @{ Key = $key; Default = $default }
}

function Prompt-ForMissingEnvVars {
  param(
    [Parameter(Mandatory)][object]   $Existing,
    [Parameter(Mandatory)][object[]] $DotEnvSpec
  )

  $managed = @()
  foreach ($s in $DotEnvSpec) {
    $entry = Get-DotEnvEntry -Spec $s
    if (-not [string]::IsNullOrWhiteSpace($entry.Key)) { $managed += $entry.Key }
  }
  $Existing.ManagedKeys = $managed

  $map = $Existing.Map
  $changed = $false

  foreach ($spec in $DotEnvSpec) {
    $entry = Get-DotEnvEntry -Spec $spec
    $key   = $entry.Key
    $def   = $entry.Default

    if ([string]::IsNullOrWhiteSpace($key)) { continue }

    $alreadySet = $map.ContainsKey($key) -and (-not [string]::IsNullOrWhiteSpace($map[$key]))
    if ($alreadySet) { continue }

    $hint      = if ($def) { " [default: $def]" } else { "" }
    $userInput = Read-Host -Prompt "Enter value for '.env' key '$key'$hint"

    $valueToStore =
      if ([string]::IsNullOrWhiteSpace($userInput) -and $def) { $def }
      else { [string]$userInput }

    $map[$key] = $valueToStore
    $changed   = $true
  }

  return $changed
}

function Write-DotEnv {
  param(
    [Parameter(Mandatory)][object] $EnvVars,
    [Parameter(Mandatory)][string] $DotEnvPath
  )

  $map         = $EnvVars.Map
  $existing    = $EnvVars.Lines
  $managedKeys = $EnvVars.ManagedKeys

  function Needs-Quotes([string]$v) { return ($v -match '\s|=') }

  $managedSet = New-Object 'System.Collections.Generic.HashSet[string]'
  foreach ($k in $managedKeys) { $null = $managedSet.Add($k) }

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
        $value     = $map[$key]
        $formatted = if (Needs-Quotes $value) { "$key=""$value""" } else { "$key=$value" }
        $output.Add($formatted) | Out-Null
        $null = $seen.Add($key)
      }
    } else {
      $output.Add($line) | Out-Null
    }
  }

  foreach ($k in $managedKeys) {
    if (-not $seen.Contains($k)) {
      $value     = $map[$k]
      $formatted = if (Needs-Quotes $value) { "$k=""$value""" } else { "$k=$value" }
      $output.Add($formatted) | Out-Null
    }
  }

  $dir = Split-Path -Parent $DotEnvPath
  if ($dir -and -not (Test-Path -LiteralPath $dir)) {
    New-Item -ItemType Directory -Path $dir -Force | Out-Null
  }

  [System.IO.File]::WriteAllText($DotEnvPath, ($output -join [Environment]::NewLine))
}

try {
  $envVars = Read-DotEnv -DotEnvPath $dotEnvPath
  $changed = Prompt-ForMissingEnvVars -Existing $envVars -DotEnvSpec $dotEnvSpec

  if ($changed -or -not (Test-Path -LiteralPath $dotEnvPath)) {
    Write-DotEnv -EnvVars $envVars -DotEnvPath $dotEnvPath
  }
}
catch {
  Write-Error $_.Exception.Message
  exit 1
}
