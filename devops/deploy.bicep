param location string = resourceGroup().location
param skuName string
param environmentName string
param skuNameST string

var resourceSuffix = 'do24-${take(uniqueString(resourceGroup().id), 6)}-${environmentName}'
var storageAccountName = 'stdevops24${uniqueString(resourceGroup().id)}'
var keyVaultName = 'kv-${resourceSuffix}'
var appServicePlanNameFunc = 'planFunc-${resourceSuffix}'
var appServicePlanNameWeb = 'planWeb-${resourceSuffix}'
var webAppName = 'webApp-${resourceSuffix}'
var sqlServerName = 'sql-swe-${resourceSuffix}'
var sqlDbName = 'sqlDb-swe-${resourceSuffix}'
var sqlAdminLogin = 'sqlAdmin'
var sqlAdminPassword = 'Bytmig123'
var applicationInsightsName = functionAppName
var workspaceName = 'log-${resourceSuffix}'
var functionAppName = 'func-${resourceSuffix}'
//var principalId = '6d2058c3-446d-4993-9592-1623bd657fa1'
//--------------------------------------------------------------------------------------------------
//App Service Plan & Web App
resource appServicePlanWeb 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanNameWeb
  location: location
  sku: {
    name: skuName
    tier: 'Basic'
    size: skuName
    family: 'B'
    capacity: 1
  }
  properties: {
    reserved: false
    maximumElasticWorkerCount: 1
  }
}

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: webAppName
  kind: 'web'
  location: location
  properties: {
  reserved: false
    serverFarmId: appServicePlanWeb.id
    httpsOnly: true
    siteConfig: {
      ftpsState: 'Disabled'
      minTlsVersion: '1.3'
      http20Enabled: true
      netFrameworkVersion: 'v8.0'
      appSettings: [
        {
           name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
           value: '~2'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsight.properties.InstrumentationKey
        }
        {
          name: 'ConnectionStrings:DbConnection'
          value: 'Server=${sqlServerName}.database.windows.net;Initial Catalog=${sqlDbName};User ID=${sqlAdminLogin};Password=${sqlAdminPassword};Encrypt=True;'
        }
      ]
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}



//--------------------------------------------------------------------------------------------------
//Application insight

resource applicationInsight 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
    WorkspaceResourceId: workspace.id
    IngestionMode:'LogAnaltics'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

resource workspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name:workspaceName
  location:location
  properties: {
    sku:{
      name: 'PerGB2018'
    }
    retentionInDays:30
    workspaceCapping:{
      dailyQuotaGb: environmentName == 'test' ? 1 : 2
    }
  }

}

//--------------------------------------------------------------------------------------------------
//Storage account
resource StorageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
name: storageAccountName
location:location
sku: {name: skuNameST}
kind: 'StorageV2'
properties:{
  minimumTlsVersion:'TLS1_2'
  supportsHttpsTrafficOnly:true
}
}

//--------------------------------------------------------------------------------------------------
//Sql DataBase & Sql Server
resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-11-01' = {
  parent: sqlServer
  name: sqlDbName
  location: 'Sweden Central'
  sku: {    
    name: 'Basic'
    tier: 'Basic'
  }
}

resource sqlAllowAzureIPs 'Microsoft.Sql/servers/firewallRules@2021-11-01' = {
  name: 'AllowWindowsAzureIps'
  parent:sqlServer
  properties: {
    startIpAddress:'84.218.33.158'
    endIpAddress: '84.218.33.158'
  }

}


resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: sqlServerName
  location: 'Sweden Central'
  tags: {
    displayName: 'SQL Server'
  }
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
  }
}



//--------------------------------------------------------------------------------------------------
//App service plan & Function app
resource appServicePlanFunc 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanNameFunc
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'
    capacity: 0
  }
  properties: {
    reserved: false
    maximumElasticWorkerCount: 1
  }
}


resource functionApp 'Microsoft.Web/sites@2023-12-01' = {
name:functionAppName
location: location
kind:'functionapp'
properties:{
  serverFarmId:appServicePlanFunc.id
  siteConfig:{
    appSettings:[{
      name:'AzureWebJobsStorage'
      value: StorageAccount.properties.primaryEndpoints.blob
    }
    {
      name: 'FUNCTIONS_EXTENSION_VERSION'
        value: '~4'
    }
    {
      name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
      value: applicationInsight.properties.InstrumentationKey
    }
    {
      name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
      value: '~2'
    }

    ]
  }

}
}

//--------------------------------------------------------------------------------------------------
//Keyvault


module KeyVaultModule 'keyvault.bicep' = {
  name:'KeyVaultModule'
  params:{
    keyVaultName: keyVaultName
    location:location
    //principalId: principalId
    
  }
}

output appName string = webAppName
