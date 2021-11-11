param environment string


resource stateStorage 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: 'coresa${environment}${uniqueString(environment)}'
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: false
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
  }
}
