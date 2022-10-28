param location string
param environment string
param aksClusterName string

var acrName = '${aksClusterName}-${environment}-registry'

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' = {
  name: acrName
  location: location
  sku: {
    name: 'Basic'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    adminUserEnabled: true
  }
}

output registryName string = containerRegistry.name
output registryId string = containerRegistry.id
output registryManagedIdentityId string = containerRegistry.identity.principalId
