param location string
param environment string
param aksClusterName string
param aksNodeVmSize string
param aksNodeCount int
param logAnalyticsWorkspaceResourceId string

var envSpecificAksClusterName = '${aksClusterName}${environment}'

resource AksClusterResource 'Microsoft.ContainerService/managedClusters@2021-09-01' = {
  name: envSpecificAksClusterName
  location: location
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
    addonProfiles: {
      httpApplicationRouting: {
        enabled: true
      }
      extensionManager: {
        enabled: true
      }
      omsagent: {
        enabled: true
        config: {
          logAnalyticsWorkspaceResourceID: logAnalyticsWorkspaceResourceId
        }
      }

    }
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
    enableNodePublicIP: false
    mode: 'System'
    enableEncryptionAtHost: false
    enableUltraSSD: false
    osType: 'Linux'
    osSKU: 'Ubuntu'
    enableFIPS: false
  }
}




