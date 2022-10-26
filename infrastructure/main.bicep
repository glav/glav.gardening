param location string = resourceGroup().location
@allowed([
  'dev'
  'prd'
])
param environment string = 'dev'

param eventGridTopicName string = 'glavgardenevents'
param storageAccountName string = 'gardeningsa'

param aksNodeCount int = 1
param aksClusterName string = 'aksgardening'
param aksNodeVmSize string = 'Standard_DS2_v2'

param dbName string
param dbContainerName string
param dbPrimaryRegion string =  resourceGroup().location
param throughput int = 400

module logAnalytics 'log-analytics.bicep' = {
  name: 'loganalytics'
}
module aksClusterResource 'akscluster.bicep' = {
  name: 'aksClusterResource'
  params: {
    aksClusterName: aksClusterName
    aksNodeVmSize: aksNodeVmSize
    aksNodeCount: aksNodeCount
    environment: environment
    location: location
  }
}

module storageAndQueuesResource 'storagequeues.bicep' = {
  name: 'storageAndQueues'
  params: {
    environment: environment
    eventGridTopicName: eventGridTopicName
    location: location
    storageAccountName: storageAccountName
  }
}

module cosmosDbResource 'cosmosdb.bicep' = {
  name: 'dbStore'
  params: {
    containerName: dbContainerName
    databaseName: dbName
    primaryRegion: dbPrimaryRegion
    throughput: throughput
    location: location
  }
}

output eventGridTopicId string = storageAndQueuesResource.outputs.eventGridTopicId
output eventGridEndpoint string = storageAndQueuesResource.outputs.eventGridEndpoint
output storageQueueId string = storageAndQueuesResource.outputs.storageQueueId


