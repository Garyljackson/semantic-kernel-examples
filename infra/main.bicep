param location string = resourceGroup().location
param openAiName string = 'oai-${uniqueString(resourceGroup().id)}'

module app 'app/app.bicep' = {
  name: 'appModule'
  params: {
    location: location
    openAiName: openAiName
  }
}

output endpoint string = app.outputs.endpoint
output apiKey string = app.outputs.apiKey
