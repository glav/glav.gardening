[CmdletBinding(SupportsShouldProcess = $true)]
Param (
  [Parameter(Mandatory = $true)] 
  [string] $SubscriptionId,

  [Parameter(Mandatory = $true)] 
  [string] $TenantId,

  [Parameter(Mandatory = $true)] 
  [ValidatePattern("[a-zA-Z0-9]{1,5}")]
  [string] $Environment,

  [string] $ResourceGroupName = "gardening" ,

  [string] $Location = "Australia East",
  
  [Int16] $DaysToLive = 1,
  [string] $Purpose = 'personal-use',
  [string] $EventGridTopicName = 'glavgardenevents',
  [string] $StorageAccountName = 'gardeningsa',
  [string] $AksClusterName = 'aksgardening',
  [string] $AksNodeVmSize = 'Standard_DS2_v2',
  [int] $AksNodeCount = 1,

  [string]$DbName,
  [string]$DbContainerName,
  [string]$DbPrimaryRegion,
  [int]$DbThroughput
)

####################################################
## Some functions
####################################################

function Get-OrSetKeyVaultGeneratedSecret {
  param(
    [Parameter(Mandatory)]
    [string] $keyVaultName, 

    [Parameter(Mandatory)]
    [string] 
    $secretName,

    [Parameter(Mandatory)]
    [ValidateScript( { $_.Ast.ParamBlock.Parameters.Count -eq 0 })]
    [Scriptblock]
    $generator
  )
  $secret = Get-AzKeyVaultSecret -VaultName $keyVaultName -Name $secretName -ErrorAction SilentlyContinue
  if (-not $secret) {
    $secretValue = & $generator
    if ($secretValue -isnot [SecureString]) {
      $secretValue = $secretValue | ConvertTo-SecureString -AsPlainText -Force
    }
    $Expires = (Get-Date).AddYears(1).ToUniversalTime()
    Set-AzKeyVaultSecret -VaultName $keyVaultName -Name $secretName -SecretValue $secretValue -Expires $Expires -ErrorAction Stop | Out-Null
    $secret = Get-AzKeyVaultSecret -VaultName $keyVaultName -Name $secretName -ErrorAction SilentlyContinue
  }
  $password = $secret.SecretValue
  $password.MakeReadOnly()
  return $password
}

function Ensure-KeyVaultSecretExists([string] $secretName, [string]$defaultValue) {
  Write-Host "Ensuring a secret entry exists in keyvault [$KeyVaultInstanceName] for $secretName..."
  Get-OrSetKeyVaultGeneratedSecret -keyVaultName $KeyVaultInstanceName -secretName "$secretName" `
    -generator { return "$defaultValue" } `
  | Out-Null
  
  $secretValue = Get-AzKeyVaultSecret -VaultName $KeyVaultInstanceName -Name $secretName -AsPlainText -ErrorAction SilentlyContinue
  if ($secretValue -eq $defaultValue) {
    Write-Host "> IMPORTANT NOTE: $secretName is not a valid connection string or secret. Please ensure that secret [$secretName] in KeyVault [$KeyVaultInstanceName] is set to the correct value otherwise connection errors can occur."
  }
    
}

function ThrowIfNullResult($result, [string] $message) {
  if ($null -eq $message -or $message -eq "") {
    $message = "An error occurred performing a deployment or executing an action"
  }
  if ($null -eq $result) {
    Write-Host $message
    throw $message
  }

}
###############################################################################
## Start of script
###############################################################################
$rg = "$ResourceGroupName-$Environment"

try {

  if ($VerbosePreference -eq "SilentlyContinue" -and $Env:SYSTEM_DEBUG) {
    #haven't passed -Verbose but do have SYSTEM_DEBUG set, so upgrade verbosity
    $VerbosePreference = "Continue"
  }

  $PSBoundParameters | Format-Table | Out-String | Write-Verbose
        
  ###############################################################################
  ## Ensure we're using the expected subscription
  ###############################################################################

  $context = Get-AzContext
  if (-not $context) {
    throw "Execute Connect-AzAccount and try again"
  }
  Write-Host "Powershell Context:"
  Write-Host $context

  if ($context.Subscription.Id -ne $SubscriptionId) {
    Write-Host "Setting Powershell Subscription/Context to Subscription Id [$SubscriptionId]"
    $ctxtResult = Set-AzContext -Subscription $SubscriptionId -Tenant $TenantId
    ThrowIfNullResult -result $ctxtResult -message "Error setting powershell subscription/context to Subscription Id [$SubscriptionId]"
  }
  
  Write-Host "Setting AZ CLI Subscription/Context to Subscription Id [$SubscriptionId]"
  az account set --subscription $SubscriptionId

  ################################################################################
  ## ARM deployment
  ################################################################################

  Write-Host "Beginning Infrastructure deploymment. Ensuring Resource Group '$ResourceGroupName' exists"
  $rgResult = (az group create -n $rg -l $Location) | ConvertFrom-Json
  ThrowIfNullResult -result $rgResult -message "Error creating/updating resource group"

  Write-Host " .. Setting expiresOn, Environment and Usage tags"
  $expiry = ((Get-Date).ToString('yyyy-MM-dd')
  az tag update --operation replace --resource-id $rgResult.id --tags "expiresOn=$expiry" "Environment=$Environment" "Usage=$Purpose"

  Write-Host "Deploying infrastructure"

  $bicepTemplatePath = Join-Path $PSScriptRoot "main.bicep"
  $armTemplatePath = Join-Path $PSScriptRoot "main.json"
  Write-Host "Compiling functions infrastructure bicep template to: " $armTemplatePath
  az bicep build -f $bicepTemplatePath 

  # Package our parameters
  $armParams = @{
    "environment"        = $Environment;
    "eventGridTopicName" = $EventGridTopicName
    "storageAccountName" = $StorageAccountName
    "aksClusterName"     = $AksClusterName
    "aksNodeVmSize"      = $AksNodeVmSize
    "aksNodeCount"       = $AksNodeCount
    "dbName"             = $DbName
    "DbContainerName"    = $DbContainerName
    "dbPrimaryRegion"    = $DbPrimaryRegion
    "throughput"         = $DbThroughput
  }
 
  Write-Host "Beginning infrastructure ARM deployment"
  $armDeployResult = New-AzResourceGroupDeployment `
    -Name "glavgarden-$Environment" `
    -ResourceGroupName $rg `
    -TemplateFile $armTemplatePath `
    -TemplateParameterObject $armParams `
    -ErrorAction Continue

    # Cleanup
  Remove-Item $armTemplatePath -ErrorAction Ignore

  if (-not $armDeployResult -or $armDeployResult.ProvisioningState -ne "Succeeded") {
    Write-Host (ConvertTo-Json $armDeployResult)
  
    #regardless of why we didn't succeed, throw an error here
    throw "An error occurred deploying resources with ARM and we're not sure why. There's probably more error info in the output above."
  }
  
  $eventGridTopicId = $armDeployResult.Outputs["eventGridTopicId"].Value
  $eventGridTopicEndpoint = $armDeployResult.Outputs["eventGridEndpoint"].Value
  $queueid = $armDeployResult.Outputs["storageQueueId"].Value

  # Write-Host "Function deployment complete"
  Write-Host "--"
  Write-Host "Infrastructure Created."
  Write-Host "-> EventGrid Topic Id: $eventGridTopicId"
  Write-Host "-> EventGrid Endpoint: $eventGridTopicEndpoint"
  Write-Host "-> Storage Queue Id: $queueid"

  Write-Host "*******"
  Write-Host "NOTE: To get credentials of the cluster for using kubectl, use the following line:"
  Write-Host "      az aks get-credentials -n '$aksClusterName' -g '$rg'"
  Write-Host "*******"
  
  # $bidsMarketApimTemplatePath = Join-Path $PSScriptRoot "api-management-template-bids-market-v2.json"
  # $functionsBicepTemplatePath = Join-Path $PSScriptRoot "5ms-functions-bids-market-v2-template.bicep"
  # $functionsArmTemplatePath = Join-Path $PSScriptRoot "5ms-functions-bids-market-v2-template.json"

  ## Keyvault: MarketDbConnectionString
  #Ensure-KeyVaultSecretExists -secretName "MarketDbConnectionString" -defaultValue "ENTER-MARKETDB-CONNECTION-STRING"
  
  ## Keyvault: EnergyOfferApiPassword
  #Ensure-KeyVaultSecretExists -secretName "EnergyOfferApiPassword" -defaultValue "ENERGY-OFFER-API-PASSWORD"
  
  ## Keyvault: EnergyOfferDirectDbConnectionString
  #Ensure-KeyVaultSecretExists -secretName "EnergyOfferDirectDbConnectionString" -defaultValue "ENERGY-OFFER-DIRECT-DB-CONNECTION-STRING"
  

  
  # Write-Host "Compiling functions infrastructure bicep template to: " $functionsArmTemplatePath
  # az bicep build -f $functionsBicepTemplatePath 

  # $functionParams = @{
  #   "environment"                         = $Environment;
  #   "marketAppName"                       = $MarketAppName;
  #   "keyvaultName"                        = $KeyVaultInstanceName;
  #   "bidsAppName"                         = $BidsAppName;
  #   "energyOfferEndpoint"                 = $EnergyOfferEndpoint;
  #   "energyOfferSubmitBidUriFragment"     = $EnergyOfferSubmitBidUriFragment;
  #   "energyOfferBidStatusUriFragment"     = $EnergyOfferBidStatusUriFragment;
  #   "energyOfferApiUser"                  = $EnergyOfferApiUser;
  #   "duidsToQuery"                        = $DuidsToQuery;
  #   "shouldLogRequestsToApi"              = $ShouldLogRequestsToApi;
  #   "shouldLogRequestsToEnergyOffer"      = $ShouldLogRequestsToEnergyOffer;
  #   "shouldMockBidsetResponse"            = $ShouldMockBidsetResponse;
  #   "shouldMockBidSubmissionResponse"     = $ShouldMockBidSubmissionResponse;
  #   "hostingPlanName"                     = $HostingPlanName;
  #   "hostinPlanSkuTier"                   = $HostinPlanSkuTier;
  #   "hostinPlanSkuName"                   = $HostinPlanSkuName;
  #   "storageAccountType"                  = $StorageAccountType;
  #   "onPremisesVNETResourceId"            = $OnPremisesVNETResourceId;
  #   "applicationInsightsName"             = $ApplicationInsightsName;
  # }
 
  # Write-Host "Beginning functions infrastructure ARM deployment"
  # $functionsArmDeployResult = New-AzResourceGroupDeployment `
  #   -Name "5msMarketBidsFunctionsInfrav2" `
  #   -ResourceGroupName $ResourceGroupName `
  #   -TemplateFile $functionsArmTemplatePath `
  #   -TemplateParameterObject $functionParams `
  #   -ErrorAction Continue

  # if (-not $functionsArmDeployResult -or $functionsArmDeployResult.ProvisioningState -ne "Succeeded") {

  #   Write-Host (ConvertTo-Json $functionsArmDeployResult)
  
  #   #regardless of why we didn't succeed, throw an error here
  #   throw "An error occurred deploying resources with ARM and we're not sure why. There's probably more error info in the output above."
  # }
  
  # Write-Host "Function deployment complete"
   

}
catch {
  Write-Host "Caught an exception of type $($_.Exception.GetType())"
  Write-Host $_.Exception.Message
  if ($_.Exception.InnerException) {
    Write-Host "Caused by: $($_.Exception.InnerException.Message)"
  }

  throw
}