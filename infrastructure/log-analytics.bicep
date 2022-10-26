param location string = resourceGroup().location
@allowed([
  'dev'
  'prd'
])
param environment string = 'dev'

var laName = 'log-${environment}-glavgarden'

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: laName
  location: location
  properties: {
    sku: {
      name: 'Standard'
    }
  }
}
