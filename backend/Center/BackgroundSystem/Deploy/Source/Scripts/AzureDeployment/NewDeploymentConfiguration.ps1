<#
	.SYNOPSIS
	Creates a new configuration for deployment.

	.DESCRIPTION
	Creates a new configuration in the.\Configurations folder with the given settings.

	.PARAM Name
	The name of the configuration profile.

	.PARAM ServiceName
	The name of the cloud service where the application should be deployed.

	.PARAM StorageAccountName
	The name of the cloud storage used for deployment.

	.PARAM DatabaseConnectionString
	The connection string used by the application to connect to the database.

	.PARAM ResourceStorageConnectionString
	The connection string used for resource storage.

	.PARAM Slot
	The service slot used for deployment (Production or Staging).

	.PARAM SubscriptionName
	The name of the Azure subscription to be used for deployment.

	.PARAM DeploymentLabelFormat
	The optional format for the deployment label. Only a '{0}' parameter is supported and it would be replaced by the
	DateTime.Now value.

	.PARAM StorageAccountKey
	The key used to access the storage account.

	.PARAM PackageLocation
	The optional path to the package. If not set, it will be searched in the output of the cloud service project.

	.PARAM DiagnosticsConnectionString
	The connection string used to access the diagnostics storage.

	.PARAM NotificationsConnectionString
	The optional (MEDI) notifications connection string.

	.PARAM ActiveDirectoryTenant
	The identifier of the active directory tenant used for domain authentication.

	.PARAM ClientId
	The identifier of the client used for domain authentication.

	.PARAM ResourceUrl
	The resource user used for domain authentication.

	.PARAM ClickOnceBaseAddress
	The base address for ClickOnce resources.

	.PARAM ClickOnceUseBeta
	Flag indicating whether the Portal should provide links to beta applications.

	.PARAM CloudServiceHost
	The address used as host.

	.PARAM DisablePortal
	A flag indicating whether the portal should be disabled (starting only BackgroundSystem).

	.PARAM LogLevel
	The log level.

	.PARAM Force
	Flag used for silent deployment (all prompts will be disabled).


#>
param
(
	[Parameter(Mandatory = $true)]
	[string]
	$Name,

	[Parameter(Mandatory = $true)]
	[string]
	$ServiceName,

	[Parameter(Mandatory = $true)]
	[string]
	$StorageAccountName,

	[Parameter(Mandatory = $true)]
	[string]
	$DatabaseConnectionString,

	[Parameter(Mandatory = $true)]
	[string]
	$ResourceStorageConnectionString,

	[Parameter()]
	[string]
	[ValidateSet( "Staging", "Production" )]
	$Slot = "Staging",

	[Parameter()]
	[string]
	$SubscriptionName = "Visual Studio Professional with MSDN",

	[Parameter()]
	[string]
	$DeploymentLabelFormat,

	[Parameter()]
	[string]
	$StorageAccountKey,

	[Parameter()]
	[string]
	$PackageLocation,

	[Parameter()]
	[string]
	$DiagnosticsConnectionString,

	[Parameter()]
	[string]
	$NotificationsConnectionString,

	[Parameter()]
	[System.Guid]
	$ActiveDirectoryTenant,

	[Parameter()]
	[System.Guid]
	$ClientId,

	[Parameter()]
	[string]
	$ResourceUrl,

	[Parameter()]
	[string]
	$ClickOnceBaseAddress,

	[Parameter()]
	[switch]
	$ClickOnceUseBeta,

	[Parameter()]
	[string]
	$CloudServiceHost,

	[Parameter()]
	[switch]
	$DisablePortal,

	[Parameter()]
	[string]
	[ValidateSet("Off", "Trace", "Debug", "Info", "Warn", "Error", "Fatal")]
	$LogLevel = "Off",

	[Parameter()]
	[switch]
	$Force
)

begin
{
	function Read-Confirm
	{
		param
		(
		)

		process
		{
			$title = "Replacing Cloud configuration"
			$message = "The Cloud configuration already exists. Replace it?"

				$options = @(
					New-Object System.Management.Automation.Host.ChoiceDescription("&No", "No")
					New-Object System.Management.Automation.Host.ChoiceDescription("&Yes","Yes"))

			return $host.ui.PromptForChoice($title, $message, $options, 0)
		}
	}

	Import-Module ".\AzureDeploymentModule"
	Initialize-DeploymentConfiguration
}

process
{
	$error = $null
	$configPath = Join-Path $PSScriptRoot (Join-Path "Configurations" $Name)
	$deploymentConfigurationPath = Join-Path $configPath "DeploymentConfiguration.xml"
	if ((Test-Path $deploymentConfigurationPath) -and -not($Force))
	{
		throw "The configuration already exists. Use the force flag if you want to override the existing configuration"
	}

	if (-not(Test-Path $configPath))
	{
		New-Item -ItemType Directory $configPath | Out-Null
	}
	
	if ((Test-Path $deploymentConfigurationPath))
	{
		Remove-Item -Path $deploymentConfigurationPath -Force
	}

	$configuration = New-Object Gorba.PowerShell.Azure.DeploymentConfiguration
	$configuration.DeploymentLabelFormat = $DeploymentLabelFormat
	$configuration.ServiceName = $ServiceName
	$configuration.Slot = $Slot
	$configuration.StorageAccountKey = $StorageAccountKey
	$configuration.StorageAccountName = $StorageAccountName
	$configuration.SubscriptionName = $SubscriptionName

	$configuration.Save($deploymentConfigurationPath)
	
	Get-Content $deploymentConfigurationPath

	$cloudConfigPath = Join-Path $configPath "ServiceConfiguration.$($Name).cscfg"
	if (-not($Force) -and (Test-Path $cloudConfigPath))
	{
		$confirm = Read-Confirm
		if (-not($confirm))
		{
			exit
		}

		Write-Warning "Replacing the existing Cloud configuration"
	}

	$cloudConfig = @"
<?xml version="1.0" encoding="utf-8"?>
<ServiceConfiguration serviceName="AzureCloudService" xmlns="http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceConfiguration" osFamily="4" osVersion="*" schemaVersion="2014-06.2.4">
  <Role name="BackgroundSystem.WorkerRole">
	<Instances count="1" />
	<ConfigurationSettings>
	  <Setting name="BackgroundSystem.CenterDataContext" value="{0}" />
	  <Setting name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" value="{1}" />
	  <Setting name="BackgroundSystem.NotificationsConnectionString" value="{2}" />
	  <Setting name="ActiveDirectory.Tenant" value="{3}" />
	  <Setting name="ActiveDirectory.ClientId" value="{4}" />
	  <Setting name="Portal.EnableHttp" value="True" />
	  <Setting name="Portal.EnableHttps" value="False" />
	  <Setting name="Wcf.ConnectionLimit" value="512" />
	  <Setting name="ActiveDirectory.AuthorizationUrl" value="{5}" />
	  <Setting name="BackgroundSystem.StartPortalHost" value="{6}" />
	  <Setting name="Portal.ClickOnceBaseAddress" value="{7}" />
	  <Setting name="BackgroundSystem.Host" value="{8}" />
	  <Setting name="Center.ResourceStorageConnectionString" value="{9}" />
	  <Setting name="Portal.ClickOnceUseBeta" value="{10}" />
	  <Setting name="Diagnostics.LogLevel" value="{11}" />
	</ConfigurationSettings>
  </Role>
</ServiceConfiguration>
"@

	if (-not($ClickOnceBaseAddress))
	{
		Write-Warning "ClickOnceBaseAddress not set. Generating a default value from the StorageAccountName"
		$ClickOnceBaseAddress = "http://{0}.blob.core.windows.net/clickonce/" -f $StorageAccountName
	}

	if (-not($CloudServiceHost))
	{
		Write-Warning "CloudeServiceHost not set. Generating a default value from the ServiceName"
		$CloudServiceHost = "{0}.cloudapp.net" -f $ServiceName
	}

	if (-not($ActiveDirectoryTenant))
	{
		Write-Warning "ActiveDirectory tenant not set. Generating new guid"
		$ActiveDirectoryTenant = [System.Guid]::NewGuid()
	}

	if (-not($ClientId))
	{
		Write-Warning "ClientId tenant not set. Generating new guid"
		$ClientId = [System.Guid]::NewGuid()
	}

	if (-not($ResourceUrl))
	{
		Write-Warning "ResourceUrl not set"
	}

	$cloudConfig = $cloudConfig -f ($DatabaseConnectionString, $DiagnosticsConnectionString, $NotificationsConnectionString, $ActiveDirectoryTenant, $ClientId, $ResourceUrl, -not($DisablePortal), $ClickOnceBaseAddress, $CloudServiceHost, $ResourceStorageConnectionString, $ClickOnceUseBeta, $LogLevel)
	Set-Content -Path $cloudConfigPath $cloudConfig -Force
}

end
{
}