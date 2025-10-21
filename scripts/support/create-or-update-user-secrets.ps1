$csprojPath = "C:\Users\agart\Code\portfolio-socialmedia-api\src\SocialMedia.WebAPI\SocialMedia.WebAPI.csproj"

$requiredKeys = @(
  "ConnectionStrings:database",
  "SomeService:ApiKey"
)

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
  $file   = Join-Path $folder "secrets.json"
  return $file
}

function Read-SecretsJson {
  param(
    [string]
    $SecretsPath
  )

  if (-not (Test-Path -LiteralPath $SecretsPath)) {
    return @{}
  }

  $raw = Get-Content -LiteralPath $SecretsPath -Raw
  if ([string]::IsNullOrWhiteSpace($raw)) {
    return @{}
  }

  try {
    $obj = ConvertFrom-Json $raw
  } catch {
    throw "Failed to parse JSON from '$SecretsPath': $($_.Exception.Message)"
  }

  $table = @{}
  if ($null -ne $obj) {
    foreach ($p in $obj.PSObject.Properties) {
      if ($null -eq $p.Value) {
        $table[$p.Name] = ""
      } else {
        $table[$p.Name] = [string]$p.Value
      }
    }
  }
  return $table
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
    [string[]]$RequiredKeys
  )

  $updated = $false
  foreach ($key in $RequiredKeys) {
    $hasValue =
      ($Existing.ContainsKey($key)) -and
      (-not [string]::IsNullOrWhiteSpace($Existing[$key]))

    if ($hasValue) {
      continue
    }

    $value = Read-Host -Prompt "Enter value for '$key'"
    if ($null -eq $value) { $value = "" }
    $Existing[$key] = [string]$value
    $updated = $true
  }

  return $updated
}

try {
  $userSecretsId = Get-UserSecretsIdFromCsproj -ProjectPath $csprojPath
  $secretsPath = Get-SecretsJsonPath -UserSecretsId $userSecretsId
  $secrets = Read-SecretsJson -SecretsPath $secretsPath
  $changed = Prompt-ForMissingSecrets -Existing $secrets -RequiredKeys $requiredKeys

  if ($changed) {
    Write-SecretsJson -Secrets $secrets -SecretsPath $secretsPath
  }
}
catch {
  Write-Error $_.Exception.Message
  exit 1
}
