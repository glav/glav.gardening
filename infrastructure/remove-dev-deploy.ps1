[CmdletBinding(SupportsShouldProcess = $true)]
Param(
    
)

$ErrorActionPreference = "Stop"

$privateParamsFile = "$PSScriptRoot\deploy-local.private.json"
if (-not (Test-Path $privateParamsFile)) {
    Copy-Item "$PsScriptRoot\deploy-local.private.template.json" $privateParamsFile
    Write-Warning "Local deployment configuration file does not exist, creating from template..."
    Write-Warning "Please update information in $privateParamsFile"
    exit 1
}

$fields = ConvertFrom-Json ([System.IO.File]::ReadAllText($privateParamsFile))
$fieldsAsHashTable = @{ }
$fields.PSObject.Properties | ForEach-Object { $fieldsAsHashTable[$_.Name] = $_.Value }

$rg = "$($fields.ResourceGroupName)-$($fields.Environment)"

Write-Warning "Removing Resource group [$rg]"
az group delete -n $rg -y
