<#
	.SYNOPSIS
	Publishes the BackgroundSystem worker role and optionally uploads ClickOnce applications to Azure.

	.PARAM DeploymentConfigurationName
	Optional parameter to specify name of the configuration to use, that is the folder in /Configurations
    containing the serialized DeploymentConfiguration and the cloud service config file.

	.PARAM DeploymentLabel
	The label to assign to the deployment. Leave null to assign a default label (format taken from the configuration; default format otherwise)

	.PARAM BuildConfiguration
	The configuration (Debug or Release) used to build the package. Default value is Release.

	.PARAM PublishClickOnce
	Flag indicating whether to publish the ClickOnce applications.

	.PARAM ClickOnceBeta
	Flag to define that the beta ClickOnce applications should be published.

	.PARAM Force
	Flag to define a silent installation.
#>
param
(
	[Parameter()]
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
	[switch]
	$SkipDeployment,

	[Parameter()]
	[switch]
	$PublishClickOnce,

	[Parameter()]
	[switch]
	$ClickOnceBeta,

	[Parameter()]
	[switch]
	$Force
)

begin
{
	if ($SkipDeployment -and -not($PublishClickOnce))
	{
		Write-Warning "Nothing to do."
		exit
	}

	Import-Module ".\AzureDeploymentModule"

	$publish = Join-Path $PSScriptRoot "PublishWorkerRole.ps1"
	$clickOnce = Join-Path $PSScriptRoot "PublishClickOnce.ps1"
}

process
{
	$deploymentConfiguration = Get-DeploymentConfiguration -DeploymentConfigurationName $DeploymentConfigurationName
	if ($SkipDeployment)
	{
		Write-Host "Skipping deployment"
	}
	else
	{
		& $publish -DeploymentConfiguration $deploymentConfiguration -DeploymentLabel $DeploymentLabel -BuildConfiguration $BuildConfiguration -Force:$Force
	}

	if (-not($PublishClickOnce))
	{
		Write-Host "PublishClickOnce flag not set. Everything is done."
		exit
	}

	& $clickOnce -DeployAll -Beta -GetLatestBuild -StorageAccountName $deploymentConfiguration.Configuration.StorageAccountName -StorageAccountKey $deploymentConfiguration.Configuration.StorageAccountKey -Force:$Force
}

end
{

}