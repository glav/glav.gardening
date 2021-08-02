"Starting DAPR instances of services"

$scriptPath = $MyInvocation.MyCommand.Path
$currentDir = $pwd

$parentDir = [System.IO.Path]::GetDirectoryName($scriptPath)
$sanitiserSvcDir = $parentDir + "\Glav.DataSanitiser.Service"
$infoGathererSvcDir = $parentDir + "\Orchestration\Glav.InfoGatheringController.Service"
$gardenOrgAgentDir = $parentDir + "\Agents\Glav.Gardening.Services.Agents.GardenOrg"

"Starting DataSanitiser service on port 5001, DAPR sidecar port: 3500"
Set-Location $sanitiserSvcDir
Start-Job -Name "sanitisersvc" -ScriptBlock {
	dapr run --app-id sanitiser --app-port 5001 --app-ssl --dapr-http-port 3500 dotnet run -- --urls='https://localhost:5001/;http://localhost:5000/'
}# > $null

"Starting InformationGatherer service on port 5003, DAPR sidecar port: 3501"
Set-Location $infoGathererSvcDir
Start-Job -Name "infogatheringsvc" -ScriptBlock {
	dapr run --app-id infogathering --app-port 5003 --app-ssl --dapr-http-port 3501 dotnet run -- --urls='https://localhost:5003/;http://localhost:5002/'
}# > $null

"Starting GardenOrg agent  on port 5005, DAPR sidecar port: 3502"
Set-Location $gardenOrgAgentDir
Start-Job -Name "infogatheringsvc" -ScriptBlock {
	dapr run --app-id gardenorgagent --app-port 5005 --app-ssl --dapr-http-port 3502 dotnet run -- --urls='https://localhost:5005/;http://localhost:5004/'
}# > $null

Set-Location $currentDir