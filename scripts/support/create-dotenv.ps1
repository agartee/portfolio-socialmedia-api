$rootDir = (get-item $PSScriptRoot).Parent.Parent.FullName

function PromptForEnvVar {
  param (
    [parameter(Mandatory = $true)]
    [String]
    $key,
    [parameter(Mandatory = $true)]
    [String]
    $prompt,
    [String]
    $defaultValue
  )

  $defaultValuePrompt = if ([string]::IsNullOrWhiteSpace($defaultValue)) { "" }
  else { " [$($defaultValue)]" }

  $varValue = Read-Host -Prompt "$($prompt)$($defaultValuePrompt)"
  if ([string]::IsNullOrWhiteSpace($varValue)) {
    $varValue = $defaultValue
  }

  return "$($key)=$($varValue)"
}

$envPath = "$rootDir\.env"
if (!(Test-Path -Path $envPath)) {

  Write-Host "Generating .env file..." -ForegroundColor Blue

  New-Item -Path $envPath -ItemType File | Out-Null

  Add-Content -Path $envPath -Value (PromptForEnvVar `
      -Key "SSL_PFX_PATH" `
      -Prompt "SSL PFX file path?" `
      -DefaultValue "$($rootDir)\.ssl\localhost.pfx")

  Add-Content -Path $envPath -Value (PromptForEnvVar `
      -Key "SSL_PFX_PASSWORD" `
      -Prompt "SSL PFX file password?")
}
