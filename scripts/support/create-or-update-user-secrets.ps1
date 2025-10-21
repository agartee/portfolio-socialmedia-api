$rootDir = (Get-Item $PSScriptRoot).Parent.Parent.FullName
$config  = Get-Content -Raw -Path "$rootDir\scripts\.project-settings.json" | ConvertFrom-Json
$csprojPath = Join-Path $rootDir $config.webApp.projectFile

$secretsSpec = @()
if ($config.secrets) { $secretsSpec = @($config.secrets) }

function Get-UserSecretsIdFromCsproj {
  param([Parameter(Mandatory)][string]$ProjectPath)

  if (-not (Test-Path -LiteralPath $ProjectPath)) {
    throw "Project file not found: $ProjectPath"
  }

  try {
    [xml]$xml = Get-Content -LiteralPath $ProjectPath -Raw
  } catch {
    throw "Failed to load XML from '$ProjectPath': $($_.Exception.Message)"
  }

  $pgTarget = $null
  foreach ($pg in $xml.Project.PropertyGroup) {
    if (-not $pg.HasAttribute('Condition')) {
      $pgTarget = $pg
      break
    }
  }

  if (-not $pgTarget) {
    throw "No unconditioned <PropertyGroup> found in $ProjectPath."
  }

  $node = $pgTarget.SelectSingleNode('UserSecretsId')
  if ($node -and -not [string]::IsNullOrWhiteSpace($node.InnerText)) {
    return $node.InnerText.Trim()
  }

  throw "No <UserSecretsId> found in expected <PropertyGroup> of $ProjectPath."
}

function Get-SecretsJsonPath {
  param([string]$UserSecretsId)

  if ($env:OS -and $env:OS -match 'Windows_NT') {
    $base = Join-Path $env:APPDATA "Microsoft\UserSecrets"
  } else {
    $base = Join-Path $HOME ".microsoft/usersecrets"
  }

  $folder = Join-Path $base $UserSecretsId
  Join-Path $folder "secrets.json"
}

function Read-SecretsJson {
  param([string]$SecretsPath)

  if (-not (Test-Path -LiteralPath $SecretsPath)) { return @{} }

  $raw = Get-Content -LiteralPath $SecretsPath -Raw
  if ([string]::IsNullOrWhiteSpace($raw)) { return @{} }

  try { $obj = ConvertFrom-Json $raw }
  catch { throw "Failed to parse JSON from '$SecretsPath': $($_.Exception.Message)" }

  $table = @{}
  if ($null -ne $obj) {
    foreach ($p in $obj.PSObject.Properties) {
      $table[$p.Name] = if ($null -eq $p.Value) { "" } else { [string]$p.Value }
    }
  }
  $table
}

function Write-SecretsJson {
  param(
    [hashtable]$Secrets,
    [string]$SecretsPath
  )

  $dir = Split-Path -Parent $SecretsPath
  if (-not (Test-Path -LiteralPath $dir)) {
    New-Item -ItemType Directory -Path $dir -Force | Out-Null
  }

  $psobj = New-Object PSObject
  foreach ($k in $Secrets.Keys) {
    $null = $psobj | Add-Member -MemberType NoteProperty -Name $k -Value $Secrets[$k] -Force
  }

  $json = $psobj | ConvertTo-Json -Depth 4

  $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
  [System.IO.File]::WriteAllText($SecretsPath, $json, $utf8NoBom)
}

function Prompt-ForMissingSecrets {
  param(
    [hashtable]$Existing,
    [object[]]$SecretsSpec 
  )

  $updated = $false

  foreach ($spec in $SecretsSpec) {
    $key     = if ($spec.secret) { [string]$spec.secret } elseif ($spec.Secret) { [string]$spec.Secret } else { $null }
    $default = if ($spec.default) { [string]$spec.default } elseif ($spec.Default) { [string]$spec.Default } else { $null }

    if ([string]::IsNullOrWhiteSpace($key)) { continue } # skip malformed entries

    $hasValue = ($Existing.ContainsKey($key)) -and (-not [string]::IsNullOrWhiteSpace($Existing[$key]))
    if ($hasValue) { continue }

    $suffix = if ($null -ne $default -and $default -ne "") { " [default: $default]" } else { "" }

    $userInput = Read-Host -Prompt "Enter value for '$key'$suffix"

    if ([string]::IsNullOrWhiteSpace($userInput) -and $null -ne $default) {
      $Existing[$key] = $default
    } else {
      $Existing[$key] = [string]$userInput
    }

    $updated = $true
  }

  return $updated
}

try {
  $userSecretsId = Get-UserSecretsIdFromCsproj -ProjectPath $csprojPath
  $secretsPath   = Get-SecretsJsonPath -UserSecretsId $userSecretsId
  $secrets       = Read-SecretsJson -SecretsPath $secretsPath

  $changed = Prompt-ForMissingSecrets -Existing $secrets -SecretsSpec $secretsSpec

  if ($changed) {
    Write-SecretsJson -Secrets $secrets -SecretsPath $secretsPath
  }
}
catch {
  Write-Error $_.Exception.Message
  exit 1
}
