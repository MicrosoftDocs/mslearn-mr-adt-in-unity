param projectName string = ''
param userId string = ''
param appRegObjectId string = ''
param utcValue string = utcNow()

var location = resourceGroup().location

var unique = substring(uniqueString(resourceGroup().id),3)
//var unique = ''

var iotHubName = '${projectName}Hub${unique}'
var adtName = '${projectName}adt${unique}'
var signalrName = '${projectName}signalr${unique}'
var serverFarmName = '${projectName}farm${unique}'
var storageName = '${projectName}${unique}'
var eventGridName = '${projectName}eg${unique}'
var funcAppName = '${projectName}funcapp${unique}'
var appInightsName = '${projectName}appinsight${unique}'
var eventGridIngestName =  '${projectName}egingest${unique}'
var eventGridCLTopicName = '${projectName}clt${unique}'
var fileContainerName =  'bladeremoteassets'
var ingestFuncName = 'telemetryfunction'

var identityName = '${projectName}scriptidentity'
var rgRoleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', '8e3af657-a8ff-443c-a75c-2fe8c4bcb635')
var rgRoleDefinitionName = guid(identity.id, rgRoleDefinitionId, resourceGroup().id)
var ADTroleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', 'bcd981a7-7f74-457b-83e1-cceb9e632ffe')
var ADTroleDefinitionName = guid(identity.id, ADTroleDefinitionId, resourceGroup().id)
var storageRoleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
var storageRoleDefinitionName = guid(identity.id, storageRoleDefinitionId, resourceGroup().id)
var ADTroleDefinitionAppName = guid(resourceId('Microsoft.ManagedIdentity/userAssignedIdentities', funcAppName), ADTroleDefinitionId, resourceGroup().id)
var ADTRoleDefinitionUserName = guid(resourceId('Microsoft.ManagedIdentity/userAssignedIdentities', userId), ADTroleDefinitionId, resourceGroup().id)
var ADTRoleDefinitionAppRegName = appRegObjectId



// create iot hub
resource iot 'microsoft.devices/iotHubs@2020-03-01' = {
  name: iotHubName
  location: location
  sku: {
    name: 'S1'
    capacity: 1
  }
  properties: {
    eventHubEndpoints: {
      events: {
        retentionTimeInDays: 1
        partitionCount: 4
      }
    }
  }
}

//create storage account (used by the azure function app)
resource storage 'Microsoft.Storage/storageAccounts@2018-02-01' = {
  name: storageName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    isHnsEnabled: false
  }
}

// Create Storage container (used for blade assets)
resource filecontainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-02-01' = {
  name: '${storage.name}/default/${fileContainerName}'
  properties: {
    publicAccess: 'Blob'
  }
  dependsOn: [
    storage
  ]
}

// create ADT instance
resource adt 'Microsoft.DigitalTwins/digitalTwinsInstances@2020-03-01-preview' = {
  name: adtName
  location: location
  tags: {}
  properties: {}
  dependsOn: [
    identity
  ]
}

// create signalr instance
resource signalr 'Microsoft.SignalRService/signalR@2020-07-01-preview' = {
  name: signalrName
  location: location
  sku: {
    name: 'Standard_S1'
    capacity: 1
    tier:  'Standard'
  }
  properties: {
    cors: {
      allowedOrigins: [
        '*'
      ]
    }
    features: [
      {
        flag: 'ServiceMode'
        value: 'Serverless'
      }
    ]
  }
}

// create App Plan - "server farm"
resource appserver 'Microsoft.Web/serverfarms@2019-08-01' = {
  name: serverFarmName
  location: location
  kind: 'functionapp'
  sku: {
    tier: 'Dynamic'
    name: 'B1'
  }
}

// create Function app for hosting the IoTHub ingress and SignalR egress
resource funcApp 'Microsoft.Web/sites@2019-08-01' = {
  name: funcAppName
  kind: 'functionapp'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    siteConfig: {
      appSettings: [
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageName};AccountKey=${listKeys(storageName, '2019-06-01').keys[0].value}'
        }
        {
          name: 'ADT_SERVICE_URL'
          value: 'https://${adt.properties.hostName}'
        }
        {
          name: 'SIGNALR_CONNECTION_STRING'
          value: 'Endpoint=https://${signalrName}.service.signalr.net;AccessKey=${listKeys(signalrName, providers('Microsoft.SignalRService', 'SignalR').apiVersions[0]).primaryKey};Version=1.0;'
        }
        {
          name: 'AzureSignalRConnectionString'
          value: 'Endpoint=https://${signalrName}.service.signalr.net;AccessKey=${listKeys(signalrName, providers('Microsoft.SignalRService', 'SignalR').apiVersions[0]).primaryKey};Version=1.0;'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }                
      ]
    }
    serverFarmId: appserver.id
    clientAffinityEnabled: false
  }
  dependsOn: [
    storage
    identity
    adt
    signalr
    appInsights
  ]
}

resource appInsights 'Microsoft.Insights/components@2015-05-01' = {
  name: appInightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

// Deploy function code from zip
resource ingestfunction 'Microsoft.Web/sites/extensions@2015-08-01' = {
  name: '${funcApp.name}/MSDeploy'
  properties: {
packageUri: 'https://github.com/MicrosoftDocs/mslearn-mr-adt-in-unity/raw/main/ARM-Template/functions/zipfiles/blade-functions.zip'
dbType: 'None'
    connectionString: ''
  }
  dependsOn: [
    funcApp
  ]
}

resource eventGridIngestTopic 'Microsoft.EventGrid/systemTopics@2020-04-01-preview' = {
  name: eventGridIngestName
  location: location
  properties: {
    source: iot.id
    topicType: 'microsoft.devices.iothubs'
  }
  dependsOn: [
    iot
    ingestfunction
    funcApp
  ]
}

resource eventGridChangeLogTopic 'Microsoft.EventGrid/topics@2020-10-15-preview' = {
  name: eventGridCLTopicName
  location: location
  sku: {
    name: 'Basic'
  }
  kind: 'Azure'
  identity: {
    type: 'None'
  }
  properties: {
    inputSchema: 'EventGridSchema'
    publicNetworkAccess: 'Enabled'
  }
}

resource eventGrid_IoTHubIngest 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2020-04-01-preview' = {
  name: '${eventGridIngestTopic.name}/${ingestFuncName}'
  properties: {
    destination: {
      endpointType: 'AzureFunction'
      properties: {
        resourceId: '${funcApp.id}/functions/${ingestFuncName}'
        maxEventsPerBatch: 1
        preferredBatchSizeInKilobytes: 64
      }
    }
    eventDeliverySchema: 'EventGridSchema'
    filter: {
      includedEventTypes: [
        'Microsoft.Devices.DeviceTelemetry'
      ]
    }
  }
  dependsOn: [
    eventGridIngestTopic
    iot
    ingestfunction
    funcApp
    signalr
    PostDeploymentscript
  ]
}

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: identityName
  location: location
}

// add RBAC role to resource group - 
resource rgroledef 'Microsoft.Authorization/roleAssignments@2018-09-01-preview' = {
  name: rgRoleDefinitionName
  properties: {
    roleDefinitionId: rgRoleDefinitionId
    principalId: reference(identityName).principalId
    principalType: 'ServicePrincipal'
  }
}

// add "Digital Twins Data Owner" role to ADT instance for our deployment
resource adtroledef 'Microsoft.Authorization/roleAssignments@2018-09-01-preview' = {
  name: ADTroleDefinitionName
  properties: {
    roleDefinitionId: ADTroleDefinitionId
    principalId: reference(identityName).principalId
    principalType: 'ServicePrincipal'
  }
}

// add "Storage Blob Data Contributor" role to RG for our deployment
resource storageroledef 'Microsoft.Authorization/roleAssignments@2018-09-01-preview' = {
  name: storageRoleDefinitionName
  properties: {
    roleDefinitionId: storageRoleDefinitionId
    principalId: reference(identityName).principalId
    principalType: 'ServicePrincipal'
  }
}

// add "Digital Twins Data Owner" permissions to teh system identity of the Azure Functions
resource adtroledefapp 'Microsoft.Authorization/roleAssignments@2018-09-01-preview' = {
  name: ADTroleDefinitionAppName
  properties: {
    roleDefinitionId: ADTroleDefinitionId
    principalId: reference(funcApp.id, '2019-08-01', 'Full').identity.principalId
    principalType: 'ServicePrincipal'
  }
  dependsOn: [ 
    funcApp
  ]
}

// // assign ADT data role owner permissions to Application
resource ADTRoleDefinitionUser 'Microsoft.Authorization/roleAssignments@2018-09-01-preview' = {
  name: ADTRoleDefinitionAppRegName
  properties: {
    roleDefinitionId: ADTroleDefinitionId
    principalId: userId
    principalType: 'User'
  }
}

// assign ADT data role owner permissions to App Registration
resource ADTRoleDefinitionAppReg 'Microsoft.Authorization/roleAssignments@2018-09-01-preview' = {
  name: ADTRoleDefinitionUserName
  properties: {
    roleDefinitionId: ADTroleDefinitionId
    principalId: appRegObjectId
    principalType: 'ServicePrincipal'
  }
}

// execute post deployment script
resource PostDeploymentscript 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: 'PostDeploymentscript'
  location: resourceGroup().location
  kind: 'AzureCLI'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identity.id}': {}
    }
  }
    properties: {
    forceUpdateTag: utcValue
    azCliVersion: '2.15.0'
    arguments: '${iot.name} ${adt.name} ${resourceGroup().name} ${location} ${eventGridChangeLogTopic.name} ${eventGridChangeLogTopic.id} ${funcApp.id} ${storage.name} ${fileContainerName}'
    primaryScriptUri: 'https://raw.githubusercontent.com/MicrosoftDocs/mslearn-mr-adt-in-unity/main/ARM-Template/postdeploy.sh'
    supportingScriptUris: []
    timeout: 'PT30M'
    cleanupPreference: 'OnExpiration'
    retentionInterval: 'P1D'
  }
  dependsOn: [
    rgroledef
    iot
  ]
}

output importantInfo object = {
  iotHubName: iotHubName
  signalRNegotiatePath: 'https://${funcApp.name}.azurewebsites.net/api/negotiate'
  adtHostName: 'https://${adt.properties.hostName}'
}
