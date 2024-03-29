Param (
  [string] $ResourceGroupName = "gardening" ,
  [string] $AksClusterName = 'aksgardening',
  [Parameter(Mandatory = $true)] 
  [ValidatePattern("[a-zA-Z0-9]{1,5}")]
  [string] $Environment
  
)

function ThrowIfNullResult($result, [string] $message) {
    if ($null -eq $message -or $message -eq "") {
      $message = "An error occurred performing a deployment or executing an action"
    }
    if ($null -eq $result) {
      Write-Host $message
      throw $message
    }
  
  }

################################################
#### Note: This assumes that the requisite features have been enabled on your subscription
#### See: https://docs.dapr.io/developing-applications/integrations/azure/azure-kubernetes-service-extension/

# Register features
# az feature register --namespace "Microsoft.ContainerService" --name "AKS-ExtensionManager"
# az feature register --namespace "Microsoft.ContainerService" --name "AKS-Dapr"

# Query features to see if registration is complete
# az feature list -o table --query "[?contains(name, 'Microsoft.ContainerService/AKS-ExtensionManager')].{Name:name,State:properties.state}"
# az feature list -o table --query "[?contains(name, 'Microsoft.ContainerService/AKS-Dapr')].{Name:name,State:properties.state}"

# Update/Refresh feature registration
# az provider register --namespace Microsoft.KubernetesConfiguration
# az provider register --namespace Microsoft.ContainerService
################################################

$rg = "$ResourceGroupName-$Environment"
$aks = $AksClusterName + $Environment

$rgCheck = az group show -g $rg
ThrowIfNullResult -result $rgCheck -message "ResourceGroup [$rg] does not exist"
Write-Host "ResourceGroup [$rg] found."

$aksCheck=(az aks show -g $rg -n $aks)
ThrowIfNullResult -result $aksCheck -message "AKS Cluster [$aks] does not exist"
Write-Host "AKS Cluster [$aks] found."

az aks get-credentials -n $aks -g $rg --overwrite-existing

Write-Host "Adding Az CLI K8s extention..."
az extension add --name k8s-extension
az extension update --name k8s-extension

Write-Host "Installing the Dapr extension into Resource Group [$rg], Cluster [$aks]..."
az k8s-extension create --cluster-type managedClusters --cluster-name $aks --resource-group $rg --name DaprExtension --extension-type Microsoft.Dapr --auto-upgrade-minor-version true
