<#
	.SYNOPSIS
	Publishes the BackgroundSystem worker role and uploads ClickOnce applications to Azure with latest versions from TFS.

	.PARAM DeploymentConfigurationName
	Optional parameter to specify name of the configuration to use, that is the folder in /Configurations
    containing the serialized DeploymentConfiguration and the cloud service config file.

	.PARAM DeploymentLabel
	The label to assign to the deployment. Leave null to assign a default label (format taken from the configuration; default format otherwise)

	.PARAM SkipDeployment
	Flag indicating whether to skip publishing of the worker role.

	.PARAM SkipClickOnce
	Flag indicating whether to skip publishing of the ClickOnce applications.

	.PARAM ClickOnceOfficial
	Flag to define that the links to the official ClickOnce applications should be published.

	.PARAM Force
	Flag to define a silent installation.
#>
param
(
	[Parameter()]
	[string]
	$DeploymentConfigurationName = "icenter-staging-production",

	[Parameter()]
	[string]
	$DeploymentLabel,

	[Parameter()]
	[switch]
	$SkipDeployment,

	[Parameter()]
	[switch]
	$SkipClickOnce,

	[Parameter()]
	[switch]
	$ClickOnceOfficial,

	[Parameter()]
	[switch]
	$Force
)

begin
{
	if ($SkipDeployment -and -$SkipClickOnce)
	{
		Write-Warning "Nothing to do."
		exit
	}

    $modulePath = Join-Path $PSScriptRoot "AzureDeploymentModule"
	Import-Module $modulePath

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
        $path = Get-TftBuildOutput -BuildDefinitionName Center_BackgroundSystem -TargetLocation "C:\Users\lef\Desktop\Target"
        $path = Join-Path $path "app.publish\BackgroundSystem.AzureCloudService.cspkg"
        if (-not(Test-Path $path))
        {
            throw "Cannot find package at '$($path)'"
        }

        $tempPath = Join-Path $env:TEMP "$([System.Guid]::NewGuid()).cspkg"
        Write-Host "Latest build package: $($path)"
        cp $path $tempPath
		& $publish -DeploymentConfiguration $deploymentConfiguration -PackageLocation $tempPath -DeploymentLabel $DeploymentLabel -Force:$Force
        rm $tempPath
	}

	if ($SkipClickOnce)
	{
		Write-Host "SkipClickOnce flag set. Everything is done."
		exit
	}

	& $clickOnce -DeployAll -Beta:(-not($ClickOnceOfficial)) -GetLatestBuild -StorageAccountName $deploymentConfiguration.Configuration.StorageAccountName -StorageAccountKey $deploymentConfiguration.Configuration.StorageAccountKey -Force:$Force
}

end
{

}