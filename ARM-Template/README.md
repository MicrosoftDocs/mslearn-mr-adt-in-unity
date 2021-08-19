# Blade Infra 
## Required Installs
Azure CLI version 2.20.0 or higher: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli

**Note: The ARM template is in the Bicep format, and if an earlier version of Azure CLI is used, you might get an “invalid JSON” error.**

## CLI
### 1. Clone this Repo

### 2. Set some vars on the CLI for use
* Projectname: the overall unique name for your resources
* appreg: the name of the application registration for your hololens app
* username: your UPN in Azure


```
projectname=adamsblade
appreg=adamsbladeappreg
```

### 3. Create App Registration
`az ad sp create-for-rbac --name ${appreg} --skip-assignment`


Save Output for later

### 4. Get ObjectID of App Registation and User and assign to envvar
```
objectid=$(az ad sp list --display-name ${appreg} --query [0].objectId --output tsv)
userid=$(az ad signed-in-user show --query objectId -o tsv)
```
you can echo this `echo $objectid` and `echo $userid` to check it has a value


### 5. Create Resource Group
`az group create --name ${projectname}-rg --location eastus`

### 6. Deploy ARM template to Resource Group
`az deployment group create -f azuredeploy.bicep -g ${projectname}-rg --parameters projectName=${projectname} userId=${userid} appRegObjectId=${objectid} --query "properties.outputs.importantInfo.value"`
- Denote the output values here for the Azure Digital Twins URL and FunctionApp SignalR URL. These will be used later

*There's also an optional two-step approach here*

##### Create Deployment:
`az deployment group create -f azuredeploy.bicep -g ${projectname}-rg --parameters projectName=${projectname} userId=${userid} appRegObjectId=${objectid} `

##### Query Outputs:
`az deployment group show -n azuredeploy -g ${projectname}-rg --query properties.outputs.importantInfo.value`

### 7. Get IotHub Connection String
`az iot hub connection-string show --resource-group ${projectname}-rg`
- Save this for later use

### 7. Add values to the Device Sim and Test
