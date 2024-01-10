param location string
param openAiName string

module azureOpenAi '../modules/cognitive-services-account.bicep' = {
  name: 'azureOpenAiModule'
  params: {
    accountName: openAiName
    kind: 'OpenAI'
    location: location
    skuName: 'S0'
  }
}

module completionModel '../modules/model-deployment.bicep' = {
  name: 'completionModelModule'
  params: {
    cognitiveServicesAccountName: azureOpenAi.outputs.name
    modelName: 'gpt-4'
    modelVersion: '1106-Preview'
  }
}

module textEmbeddingModel '../modules/model-deployment.bicep' = {
  name: 'textEmbeddingModelModule'
  params: {
    cognitiveServicesAccountName: azureOpenAi.outputs.name
    modelName: 'text-embedding-ada-002'
    modelVersion: '2'
  }
  dependsOn: [
    completionModel
  ]
}

output endpoint string = azureOpenAi.outputs.endpoint
output apiKey string = azureOpenAi.outputs.apiKey
