param location string
@allowed([
  'dev'
  'prd'
])
param environment string

param eventGridTopicName string
param storageAccountName string


var envSpecificEventGridTopicName = '${eventGridTopicName}${environment}'
var envSpecificStorageAccountName = '${storageAccountName}${environment}'
var envSpecificStorageAccountQueueName = 'gardeningeventq${environment}'


resource StorageAccountResource 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: envSpecificStorageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    publicNetworkAccess: 'Enabled'
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
    supportsHttpsTrafficOnly: true
    encryption: {
      services: {
        blob: {
          keyType: 'Account'
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
    accessTier: 'Hot'
    
  }
}

resource StorageAccountBlobResource 'Microsoft.Storage/storageAccounts/blobServices@2021-06-01' = {
  parent: StorageAccountResource
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
    deleteRetentionPolicy: {
      enabled: false
    }
  }
}

resource StorageAccountQueueResource 'Microsoft.Storage/storageAccounts/queueServices@2021-06-01' = {
  parent: StorageAccountResource
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}


resource StorageAccountEventQueue 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-06-01' = {
  parent: StorageAccountQueueResource
  name: envSpecificStorageAccountQueueName
  properties: {
    metadata: {}
  }
}

resource EventGridTopicResource 'Microsoft.EventGrid/topics@2021-12-01' = {
  name: envSpecificEventGridTopicName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    inputSchema: 'EventGridSchema'
    publicNetworkAccess: 'Enabled'
  }
}

resource EventGridSubscription 'Microsoft.EventGrid/eventSubscriptions@2021-12-01' = {
  name: '${envSpecificStorageAccountQueueName}subscription'
  scope: EventGridTopicResource
  properties: {
    destination: {
      endpointType: 'StorageQueue'
      properties: {
        queueMessageTimeToLiveInSeconds: 3600
        queueName: envSpecificStorageAccountQueueName
        resourceId: StorageAccountResource.id
      }
    }
    eventDeliverySchema: 'EventGridSchema'
    filter: {
      isSubjectCaseSensitive: false
    }
  }
  dependsOn: [
    StorageAccountEventQueue
  ]
}

output eventGridTopicId string = EventGridTopicResource.id
output eventGridEndpoint string = EventGridTopicResource.properties.endpoint
output storageQueueId string = StorageAccountResource.id
