Param (
  [string] $ResourceGroupName = "gardening" ,
  [string] $AksClusterName = 'aksgardening'
)

################################################
#### Note: This assumes that the requisite features have been enabled on your subscription

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


az aks get-credentials -n $AksClusterName -g $ResourceGroupName --overwrite-existing

Write-Host "Adding Az CLI K8s extention..."
az extension add --name k8s-extension
az extension update --name k8s-extension

Write-Host "Installing the Dapr extension into Resource Group [$ResourceGroupName], Cluster [$AksClusterName]..."
az k8s-extension create --cluster-type managedClusters --cluster-name $AksClusterName --resource-group $ResourceGroupName --name DaprExtension --extension-type Microsoft.Dapr --auto-upgrade-minor-version true
