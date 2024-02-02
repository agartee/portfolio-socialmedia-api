param(
    [Parameter(Mandatory = $true, HelpMessage = "Path to JSON file")]
    [string]$path,
    [Parameter(Mandatory = $true, HelpMessage = "JSON property name")]
    [string]$key,
    [Parameter(Mandatory = $true, HelpMessage = "JSON property value")]
    [string]$value
)

function Add-OrUpdateJsonProperty {
    param(
        [PSCustomObject]$JsonObject,
        [string]$PropertyKey,
        [string]$PropertyValue
    )
    
    if ($null -eq $JsonObject.$PropertyKey) {
        $JsonObject | Add-Member -MemberType NoteProperty -Name $PropertyKey -Value $PropertyValue
    } else {
        $JsonObject.$PropertyKey = $PropertyValue
    }
}

if (Test-Path $path) {
    $jsonContent = Get-Content -Path $path -Raw | ConvertFrom-Json
} else {
    $jsonContent = @{}
}

if (-not [string]::IsNullOrWhiteSpace($key)) {
    Add-OrUpdateJsonProperty -JsonObject $jsonContent -PropertyKey $key -PropertyValue $value
}

$jsonContent | ConvertTo-Json -Depth 100 | Set-Content -Path $path
