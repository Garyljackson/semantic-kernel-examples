param cognitiveServicesAccountName string
param modelName string
param modelVersion string
param tags object = {}

resource cognitiveServicesAccount 'Microsoft.CognitiveServices/accounts@2023-10-01-preview' existing = {
  name: cognitiveServicesAccountName
}

resource completionModel 'Microsoft.CognitiveServices/accounts/deployments@2023-10-01-preview' = {
  parent: cognitiveServicesAccount
  name: modelName
  sku: {
    name: 'Standard'
    capacity: 60
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: modelName
      version: modelVersion
    }
    versionUpgradeOption: 'OnceNewDefaultVersionAvailable'
    currentCapacity: 60
    raiPolicyName: 'Microsoft.Default'
  }
  tags: union(tags, { ServiceType: 'AI Model Deployment' })
}
