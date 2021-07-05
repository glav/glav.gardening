"Stopping all jobs (including DAPR services)"
Get-Job | Stop-Job
"Removing all stopped jobs"
 Get-Job | Remove-Job
