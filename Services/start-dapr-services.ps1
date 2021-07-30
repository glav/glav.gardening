"Starting DAPR instances of services"

$scriptPath = $MyInvocation.MyCommand.Path
$currentDir = $pwd

$parentDir = [System.IO.Path]::GetDirectoryName($scriptPath)
$sanitiserSvcDir = $parentDir + "\Glav.DataSanitiser.Service"
$infoGatherrerSvcDir = $parentDir + "\Glav.InfoGatheringController.Service"

"Starting DataSanitiser service on port 5001, DAPR sidecar port: 3500"
Set-Location $sanitiserSvcDir
Start-Job -Name "sanitisersvc" -ScriptBlock {
	dapr run --app-id sanitiser --app-port 5001 --app-ssl --dapr-http-port 3500 dotnet run -- --urls='https://localhost:5001/;http://localhost:5000/'
} > $null

"Starting InformationGatherer service on port 5003, DAPR sidecar port: 3501"
Set-Location $infoGatherrerSvcDir
Start-Job -Name "infogatheringsvc" -ScriptBlock {
	dapr run --app-id infogathering --app-port 5003 --app-ssl --dapr-http-port 3501 dotnet run -- --urls='https://localhost:5003/;http://localhost:5002/'
} > $null

Set-Location $currentDir