param eventGridTopicName string = 'glavgardenevents'
param storageAccountName string = 'gardeningsa'
param aksClusterName string = 'aksgardening'
param aksNodeVmSize string = 'Standard_DS2_v2'

@allowed([
  'dev'
  'prd'
])
param environment string = 'dev'
param aksNodeCount int = 1

var envSpecificEventGridTopicName = '${eventGridTopicName}${environment}'
var envSpecificStorageAccountName = '${storageAccountName}${environment}'
var envSpecificAksClusterName = '${aksClusterName}${environment}'
var envSpecificStorageAccountQueueName = 'gardeningeventq${environment}'

resource AksClusterResource 'Microsoft.ContainerService/managedClusters@2021-09-01' = {
  name: envSpecificAksClusterName
  location: resourceGroup().location
  sku: {
    name: 'Basic'
    tier: 'Free'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    dnsPrefix: envSpecificAksClusterName
    agentPoolProfiles: [
      {
        name: 'nodepool1'
        count: aksNodeCount
        vmSize: aksNodeVmSize
        mode: 'System'
      }
    ]
    // linuxProfile: {
    //   adminUsername: 'azureuser'
    //   ssh: {
    //     publicKeys: [
    //       {
    //         keyData: 'ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQChbJMxSGwgquOXePjclR12yRjQWy8VWZqzhtBvaC8TZpgsNDZH5nmfDcez3785df+nLRVsDonYL5Y4YkaDnFDWPLyJV9h6/3IdyKf+WbD8jnOpX6Zz1Vem5CHbYyCe34wXU8lZk0OBSrcRX11WNyy6bsUaO53sbiCOucW+4MRfyGdwgEWvR+/pSEIZn0muHrvfOuxpaAcX2KuhfvZx8T+TCptY18CzvpE6ZN4+6kyXwhwbiDKML6w17u5yRd9Nfw3EFjIK+v0SFoyWVUIQQWMFE6mSzwbA8VQj/xXk8lWjvVWUh+s7XqP4budh/eHTipYhHBs+xjWjxoKbCGkkhXbN'
    //       }
    //     ]
    //   }
    // }
    enableRBAC: true
    disableLocalAccounts: false
  }
}


resource StorageAccountResource 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: envSpecificStorageAccountName
  location: resourceGroup().location
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

resource AksClusterNodepoolResource 'Microsoft.ContainerService/managedClusters/agentPools@2021-09-01' = {
  parent: AksClusterResource
  name: 'nodepool1'
  properties: {
    count: 1
    vmSize: 'Standard_DS2_v2'
    osDiskSizeGB: 128
    osDiskType: 'Managed'
    kubeletDiskType: 'OS'
    maxPods: 110
    type: 'VirtualMachineScaleSets'
    enableAutoScaling: false
    powerState: {
      code: 'Running'
    }
    orchestratorVersion: '1.21.7'
    enableNodePublicIP: false
    mode: 'System'
    enableEncryptionAtHost: false
    enableUltraSSD: false
    osType: 'Linux'
    osSKU: 'Ubuntu'
    enableFIPS: false
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
  location: resourceGroup().location
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
