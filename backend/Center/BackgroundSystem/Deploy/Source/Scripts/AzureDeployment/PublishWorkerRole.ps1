<#
	.SYNOPSIS
	Publishes the BackgroundSystem worker role to Azure.

	.PARAMETER DeploymentConfiguration
	The required configuration.

	.PARAMETER DeploymentConfigurationName
	The name of the configuration to use, that is the folder in /Configurations containing the serialized DeploymentConfiguration and the cloud service config file.

	.PARAMETER DeploymentLabel
	The label to assign to the deployment. Leave null to assign a default label (format taken from the configuration; default format otherwise)

	.PARAMETER BuildConfiguration
	The configuration (Debug or Release) used to build the package.

	.PARAMETER PackageLocation
	Optional parameter to specify the path of the package to deploy.

	.PARAMETER PublishScriptPath
	Optional parameter to specify the path to the PublishToAzure script.

	.PARAMETER ServiceDefinitionPath
	Optional parameter to specify the path to the service definition file.

	.PARAMETER WorkerRole
	Optional parameter to specify the name of the worker role as it is in the service definition.

	.PARAMETER NoTfs
	Optional switch to specify that tf command is not available.

	.PARAMETER Force
	Flag to define a silent installation.

	.REMARKS
	The cloud service MUST exist before using this script.
    If the service definition defined the 'Https' certificate, then the service configuration (.cscfg file) must declare
    a valid value. The script will have the following behavior:
    - if the HttpsCertificatePath is specified in the configuration file, it has the highest priority and it's uploaded
      if needed to the service. WARNING: it will ask for password. In this case, the thumbprint value in the cscfg file
      could also be left empty
    - if the thumbprint attribute is empty on the Certificate element of the configuration, a new certificate is generated,
      added to the My local certificate store for Current user and set on the configuration file. The password used will
      be the HttpsCertificatePassword value specified in the cscfg file if present, otherwise the predefined
      'Center.Portal.Https'
    - if the thumbprint has a value, the certificate with that value is searched in the My local certificate store for
      Current user. If not found, an exception is thrown. The password used to access the certificate will be the
      HttpsCertificatePassword value specified in the cscfg file if present, otherwise the predefined
      'Center.Portal.Https'

    Whenever the cscfg file should be updated by the script, the IsReadOnly flag of the file will be checked. If set,
    the script will try to use the 'tf' command to checkout the file, assuming that it's running in a TFS workspace. If
    the attempt will fail, or if the -NoTfs flag is specified, the script will throw an exception.
    The script will try to add the generated certificate to TFS unless the -NoTfs flag is specified.
	
	.EXAMPLE

	Build and deploy the Portal only

	$packageLocation = Resolve-Path "path_to_package.cspkg"
	$serviceDefinitionPath = Resolve-Path "path_to_servicedefinition.csdef"
	$project = Resolve-Path "path_to_project.ccproj"
	PublishWorkerRole.ps1 -DeployConfigurationName "icenter-lef-portal" -PackageLocation $packageLocation -ServiceDefinitionPath $serviceDefinitionPath -WorkerRole "Portal.WorkerRole" -Build -ProjectToBuild $project
#>
[CmdletBinding(DefaultParameterSetName = "Configuration")]
param
(
	[Parameter(ParameterSetName = "Configuration")]
	$DeploymentConfiguration,

	[Parameter(Mandatory = $true, ParameterSetName = "Name")]
	[string]
	$DeploymentConfigurationName,

	[Parameter()]
	[string]
	$DeploymentLabel,

	[Parameter()]
	[string]
	[ValidateSet( "Debug", "Release" )]
	$BuildConfiguration = "Release",

	[Parameter()]
	[string]
	$PackageLocation,

	[Parameter()]
	[string]
	$PublishScriptPath,

	[Parameter()]
	[string]
	$ServiceDefinitionPath,

	[Parameter()]
	[string]
	[ValidateSet("BackgroundSystem.WorkerRole", "Portal.WorkerRole")]
	$WorkerRole,

	[Parameter()]
	[string]
	$ProjectToBuild,

	[Parameter()]
	[switch]
	$Build,

	[Parameter()]
	[switch]
	$NoTfs,

	[Parameter()]
	[switch]
	$Force
)

begin
{
	function Invoke-MsBuild
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$Path,

			[Parameter(Mandatory = $true)]
			[string]
			[ValidateSet("Debug", "Release")]
			$Configuration
		)

		process
		{
			Write-Host "Building the project..."
			$msbuild = Resolve-Path "C:\Program Files (x86)\MSBuild\12.0\bin\amd64\msbuild.exe" -ErrorAction Stop
			
			$result = & $msbuild /t:CorePublish "/p:Configuration=$($Configuration)" $Path
			if ($LASTEXITCODE -ne 0)
			{
				throw "Error while building the project"
			}
		}
	}

	function Get-RelativePath
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$Path
		)

		process
		{
			$fullPath = Join-Path $PSScriptRoot $Path

			try
			{
				$result = Resolve-Path $fullPath -ErrorAction Stop
				return $result
			}
			catch
			{
				Write-Error ("Can't find relative path '{0}'. Maybe the scripts have been moved without updating relative paths." -f $fullPath)
				return
			}
		}
	}

	$deploymentModulePath = Resolve-Path (Join-Path $PSScriptRoot "AzureDeploymentModule")
	Import-Module $deploymentModulePath
	
	Initialize-DeploymentConfiguration
}

process
{
	if ($Build)
	{
		if (-not($ProjectToBuild))
		{
			$ProjectToBuild = Join-Path $PSScriptRoot "..\..\..\..\Source\AzureCloudService\BackgroundSystem.AzureCloudService.ccproj"
			$ProjectToBuild = Resolve-Path $ProjectToBuild -ErrorAction Stop
		}

		Invoke-MsBuild -Path $ProjectToBuild -Configuration $BuildConfiguration
	}

	switch ($PSCmdlet.ParameterSetName)
	{
		"Configuration" { if (-not($DeploymentConfiguration)) { $DeploymentConfiguration = Get-DeploymentConfiguration -PackageLocation $PackageLocation } }
		"Name" { $DeploymentConfiguration = Get-DeploymentConfiguration -DeploymentConfigurationName $DeploymentConfigurationName -PackageLocation $PackageLocation }
	}

	if (-not($PublishScriptPath))
	{
		$PublishScriptPath = Get-RelativePath "..\..\..\..\..\Common\Deploy\Source\PublishToAzure.ps1"
	}

	if (-not(Test-Path $PublishScriptPath))
	{
		throw "Can't find the publish script path '$($PublishScriptPath)'"
	}

	if (-not($PackageLocation))
	{
		Write-Host $DeploymentConfiguration.Configuration.ServiceName
		Write-Host "Resolving path '$($DeploymentConfiguration.Configuration.PackageLocation)'"
		$PackageLocation = Resolve-Path $DeploymentConfiguration.Configuration.PackageLocation -ErrorAction SilentlyContinue
	}
	else
	{
		Write-Host "Resolving path '$($PackageLocation)'"
		$PackageLocation = Resolve-Path $PackageLocation -ErrorAction SilentlyContinue
	}

	if (-not(Test-Path $PackageLocation))
	{
		throw "Can't find the package path '$($PackageLocation)'"
	}

	Write-Host "DeploymentConfiguration: $($DeploymentConfiguration)"
	$cloudConfigLocation = Join-Path $DeploymentConfiguration.Path "ServiceConfiguration.$($($DeploymentConfiguration.Name)).cscfg"
	Write-Verbose "ServiceName: $($DeploymentConfiguration.Configuration.ServiceName)"
	Write-Verbose "StorageAccountName: $($DeploymentConfiguration.Configuration.StorageAccountName)"
	Write-Verbose "PackageLocation: $($PackageLocation)"
	Write-Verbose "CloudConfigLocation: $($cloudConfigLocation)"
	if (-not($DeploymentLabel))
	{
		if ($DeploymentConfiguration.Configuration.DeploymentLabelFormat)
		{
			Write-Verbose "No deployment label specified. Using the formatted one from the configuration"
			$DeploymentLabel = $DeploymentConfiguration.Configuration.DeploymentLabelFormat -f [System.DateTime]::Now
		}
		else
		{
			Write-Verbose "No deployment label specified and no valid format in the configuration. Using default Deployment format"
			$DeploymentLabel = "Center_{0}_{1:yyyMMdd-HH\:mm\:ss}" -f ($env:COMPUTERNAME, [System.DateTime]::Now)
		}
	}
	
	Write-Verbose "DeploymentLabel: $($DeploymentLabel)"
	Write-Verbose "Selected subscription: $($DeploymentConfiguration.Configuration.SubscriptionName)"
	Write-Verbose "Slot: $($DeploymentConfiguration.Configuration.Slot)"

	if (-not($ServiceDefinitionPath))
	{
		if ($DeploymentConfiguration.Configuration.ServiceDefinitionPath)
		{
			Write-Host "Resolving path '$($DeploymentConfiguration.Configuration.ServiceDefinitionPath)'"
			if (Test-Path $DeploymentConfiguration.Configuration.ServiceDefinitionPath)
			{
				$ServiceDefinitionPath = Resolve-Path $DeploymentConfiguration.Configuration.ServiceDefinitionPath
			}
			else
			{
				$p = Join-Path $PSScriptRoot "Configurations"
				$p = Get-ChildItem -Directory -Path $p | Select-Object -First 1 -ExpandProperty FullName
				$p = Join-Path $p $DeploymentConfiguration.Configuration.ServiceDefinitionPath
				$ServiceDefinitionPath = Resolve-Path $p -ErrorAction SilentlyContinue
			}
		}
		else
		{
			Write-Host "ServiceDefinitionPath not specified. Default path will be used."
		}
	}
	else
	{
		Write-Host "Resolving path '$($ServiceDefinitionPath)'"
		$ServiceDefinitionPath = Resolve-Path $ServiceDefinitionPath -ErrorAction SilentlyContinue
	}

	if ($DeploymentConfiguration.Configuration.WorkerRole)
	{
		$WorkerRole = $DeploymentConfiguration.Configuration.WorkerRole
	}
	else
	{
		$WorkerRole = "BackgroundSystem.WorkerRole"
	}

	Write-Host "Testing the configuration"
	$test = Test-DeploymentConfiguration -Path $cloudConfigLocation -ServiceDefinitionPath $ServiceDefinitionPath -WorkerRole $WorkerRole
	if (-not($test))
	{
		Write-Error "Invalid configuration. Please check your configuration and the service definition"
		return
	}

	Write-Host "Verifying Https configuration and certificate"
	if (Test-HttpsCertificateRequired -Path $cloudConfigLocation -WorkerRole $WorkerRole -DeploymentConfiguration $DeploymentConfiguration.Configuration)
	{
		Write-Host "Certificate Https is required in configuration"
		if (-not(Test-HttpsCertificate -Path $cloudConfigLocation -WorkerRole $WorkerRole -DeploymentConfiguration $DeploymentConfiguration.Configuration))
		{
			if (-not(Publish-HttpsCertificate -Path $cloudConfigLocation -WorkerRole $WorkerRole -DeploymentConfiguration $DeploymentConfiguration.Configuration -NoTfs:$NoTfs))
			{
				Write-Error "Error while publishing the certificate"
				return
			}
		}
	}

	$currentAccount = Get-AzureAccount
	if (-not($currentAccount))
	{
		$accountError = $null
		$currentAccount = Add-AzureAccount -ErrorAction SilentlyContinue -ErrorVariable accountError
		if ($accountError)
		{
			throw "Can't load the Azure account"
		}
	}

	$subscriptionError = $null
	Select-AzureSubscription $DeploymentConfiguration.Configuration.SubscriptionName -ErrorAction SilentlyContinue -ErrorVariable $subscriptionError

	if ($subscriptionError)
	{
		Write-Error "Error selecting the Azure subscription"
		throw "Can't proceed without Azure subscription"
	}

	$subscription = Get-AzureSubscription -SubscriptionName $DeploymentConfiguration.Configuration.SubscriptionName

	if (-not($subscription))
	{
		throw "can't select the subscription '$($DeploymentConfiguration.Configuration.SubscriptionName)'. This could be because a wrong account is selected or because the name is not correct"
	}

	if (-not($subscription.CurrentStorageAccountName))
	{
		Write-Warning "Current storage account name not set for configuration $($DeploymentConfiguration.Configuration.SubscriptionName). Setting it to value '$($DeploymentConfiguration.Configuration.StorageAccountName)'"
		Set-AzureSubscription -SubscriptionName $DeploymentConfiguration.Configuration.SubscriptionName -CurrentStorageAccountName $DeploymentConfiguration.Configuration.StorageAccountName
	}

	& $PublishScriptPath -ServiceName $DeploymentConfiguration.Configuration.ServiceName -StorageAccountName $DeploymentConfiguration.Configuration.StorageAccountName -PackageLocation $PackageLocation -CloudConfigLocation $cloudConfigLocation -DeploymentLabel $DeploymentLabel -SelectedSubscription $DeploymentConfiguration.Configuration.SubscriptionName -Slot $DeploymentConfiguration.Configuration.Slot -Force:$Force -AlwaysDeleteExistingDeployments
}

end
{
}