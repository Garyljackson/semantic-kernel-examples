# Information
A collection of Semantic Kernel examples

# Instructions

## Provision infrastructure
Bicep templates and modules have been provided to create the required infrastructure  

To run the template you will need the [Azure CLI and the bicep extension](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/install#azure-cli)

The Bicep template creates the following  
- An Azure Open AI resource
- Deploys a completion model: GPT4 (1106-Preview)
- Deploys a embedding model: text-embedding-ada-002
- Outputs the endpoint and api key in the output JSON
  - properties/outputs/endpoint
  - properties/outputs/apiKey

To provision the infrastructure, open a terminal and run the following commands

```powershell
cd .\infra\

az login
az group create --location australiaeast --name semantic-kernel-examples
az deployment group create --resource-group semantic-kernel-examples --template-file ./main.bicep
```

## User Secret Config
The examples need an Azure Open AI resource endpoint and API key  
Get the values from the output of the infrastructure deploy, update the values below and run the following commands

Leave the GUID/id as is!

```powershell
dotnet user-secrets set "AzureOpenAi:Endpoint" "your endpoint value here" --id "ABC4010A-7C42-4B82-8BFF-EEB6B0FEAB07"

dotnet user-secrets set "AzureOpenAi:ApiKey" "your api key value here" --id "ABC4010A-7C42-4B82-8BFF-EEB6B0FEAB07"
```