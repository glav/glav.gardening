[CmdletBinding(SupportsShouldProcess = $true)]
Param (
  [Parameter(Mandatory = $true)] 
  [string] $SubscriptionId,

  [Parameter(Mandatory = $true)]
  [string] $ResourceGroupName="gardening" ,

  [string] $Location = "Australia East",
  
  [Parameter(Mandatory = $true)] 
  [ValidatePattern("[a-zA-Z0-9]{1,5}")]
  [string] $Environment,

  [Int16] $ClusterNodeCount = 1,
  [Int16] $DaysToLive = 14,
  [string] $Purpose='personal-use'
  
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

function Ensure-KeyVaultSecretExists([string] $secretName, [string]$defaultValue)
{
  Write-Host "Ensuring a secret entry exists in keyvault [$KeyVaultInstanceName] for $secretName..."
    Get-OrSetKeyVaultGeneratedSecret -keyVaultName $KeyVaultInstanceName -secretName "$secretName" `
    -generator { return "$defaultValue" } `
  | Out-Null
  
  $secretValue = Get-AzKeyVaultSecret -VaultName $KeyVaultInstanceName -Name $secretName -AsPlainText -ErrorAction SilentlyContinue
  if ($secretValue -eq $defaultValue) {
    Write-Host "> IMPORTANT NOTE: $secretName is not a valid connection string or secret. Please ensure that secret [$secretName] in KeyVault [$KeyVaultInstanceName] is set to the correct value otherwise connection errors can occur."
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

  if ($context.Subscription.Id -ne $SubscriptionId) {
    Set-AzContext -SubscriptionId $SubscriptionId
  }

  ################################################################################
  ## ARM deployment
  ################################################################################

  Write-Host "Beginning Infrastructure deploymment. Ensuring Resource Group '$ResourceGroupName' exists"
  $rgResult = (az group create -n $rg -l $Location) | ConvertFrom-Json

  Write-Host " .. Setting expiresOn, Environment and Usage tags"
  $expiry = ((Get-Date).AddDays($DaysToLive)).ToString('yyyy-MM-dd')
  az tag update --operation replace --resource-id $rgResult.id --tags "expiresOn=$expiry" "Environment=$Environment" "Usage=$Purpose"

  $aksClusterName = "aks-gardening-$Location"
  Write-Host " .. Ensuring AKS Cluster [$aksClusterName] is created"
  $aksResult = (az aks create --resource-group $ResourceGroupName --name $aksClusterName --node-count $ClusterNodeCount --enable-addons monitoring,http_application_routing --generate-ssh-keys --enable-aad --enable-azure-rbac --load-balancer-managed-outbound-ip-count 1) | ConvertFrom-Json
  if ($null -eq $aksResult)
  {
     Write-Host "Error creating/updating AKS Cluster"
     throw "An error occurred deploying AKS Cluster"
  }

  Write-Host "*******"
  Write-Host "NOTE: To get credentials of the cluster for using kubectl, use the following line:"
  Write-Host "      az aks get-credentials -n '$aksClusterName' -g '$ResourceGroupName'"
  Write-Host "*******"
  
#  $bidsMarketApimTemplatePath = Join-Path $PSScriptRoot "api-management-template-bids-market-v2.json"
#  $functionsBicepTemplatePath = Join-Path $PSScriptRoot "5ms-functions-bids-market-v2-template.bicep"
#  $functionsArmTemplatePath = Join-Path $PSScriptRoot "5ms-functions-bids-market-v2-template.json"

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