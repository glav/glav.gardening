param environment string

module core 'core.bicep' = {
  name: 'coreInfra'
  params: {
    environment: environment
  }
}
