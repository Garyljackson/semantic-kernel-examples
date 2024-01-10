az login
az group create --location australiaeast --name semantic-kernel-examples
az deployment group create --resource-group semantic-kernel-examples --template-file ./main.bicep