param location string = resourceGroup().location
@allowed([
  'dev'
  'prd'
])
param environment string = 'dev'

var laName = 'log-${environment}-glavgarden'
var appInsightsName = 'appinsights-${environment}-glavgarden'

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: laName
  location: location
  properties: {
    sku: {
      name: 'Standard'
    }
  }
}

resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

