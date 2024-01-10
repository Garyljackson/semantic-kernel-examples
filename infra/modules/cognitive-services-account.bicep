param accountName string
param location string
param kind string
param skuName string
param tags object = {}

resource cognitiveServicesAccount 'Microsoft.CognitiveServices/accounts@2023-10-01-preview' = {
  name: accountName
  location: location
  sku: {
    name: skuName
  }
  kind: kind
  properties: {
    networkAcls: {
      defaultAction: 'Allow'
      virtualNetworkRules: []
      ipRules: []
    }
    publicNetworkAccess: 'Enabled'
  }
  tags: union(tags, { ServiceType: 'Cognitive Services Account' })
}

output name string = cognitiveServicesAccount.name
