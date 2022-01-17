[CmdletBinding(SupportsShouldProcess = $true)]
Param (
  [string] $EventGridTopicName="glavgardeneventsdev"
)

Write-Host "Attempting to get details for EventGrid topic: $EventGridTopicName"
$evgridtopic=(az eventgrid topic list --query "[? name=='$EventGridTopicName']") | ConvertFrom-Json
if (-not $evgridtopic) {
  throw "Error, no EventGrid topic found with name [$EventGridTopicName]"
}

Write-Host "Attempting to extract key for EventGrid topic: $EventGridTopicName"
$eventGridTopicKey = (az eventgrid topic key list -n $EventGridTopicName -g $evgridtopic.resourceGroup --query "key1" -o tsv)

$timeStr = $((get-date).ToUniversalTime().ToString('s'))
#Generate some data
$eventData = '[ {"id": "1", "eventType": "collectionStart", "subject": "gardening/collection", "eventTime": "'+ $timeStr+ '", "data":{ "startType": "manualscript"},"dataVersion": "1.0"} ]'

Write-Host "Sending sample message to EventGrid topic: $EventGridTopicName"
Invoke-WebRequest -Uri $evgridtopic.endpoint -Method Post -Headers @{"aeg-sas-key"="$eventGridTopicKey"}  -Body $eventData