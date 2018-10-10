<#
	.SYNOPSIS
	Performs the setup of Center 2.0 on the icenter-dev cloud service.

	.DESCRIPTION
	Performs the setup of Center 2.0 on the icenter-dev cloud service.
	This script is only a 'wrapper' to call the script 01_SetupSystem-20.ps1.
#>
param
(
	[Parameter(Mandatory = $true)]
	[string]
	[ValidateScript({ Test-Path $_ })]
	$Path,

	[Parameter(Mandatory = $true)]
	[string]
	$DatabaseUser,

	[Parameter(Mandatory = $true)]
	[string]
	$DatabasePassword,

	[Parameter(Mandatory = $true)]
	[string]
	$StorageAccessKey
)

begin
{
	$scriptPath = Join-Path $PSScriptRoot "..\01_SetupSystem-2_0.ps1"
	if (-not(Test-Path $scriptPath))
	{
		throw "Script '$($scriptPath)' not found"
	}

	$serviceName = "icenter-dev"
	$databaseServerName = "zbz0us8d5e"
	$databaseName = "icenter-dev"
	$databaseConnectionString = "Server=tcp:$($databaseServerName).database.windows.net,1433;Database=$($databaseName);User ID=$($databaseUser);Password=$($databasePassword);Trusted_Connection=False;Encrypt=True;Connection Timeout=30;"
	$storageName = "icenterdev"
	$storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=$($storageName);AccountKey=REvQZGa/Ec8cwwqwEEszEc6LcVIK+0pwa14e8umAgas4IR/pmKXUbmOKWl7GYtAo8cW8WI5itkOpBQkdSdwfLA=="
}

process
{
	& $scriptPath -Path $Path -ServiceName $serviceName -DatabaseServerName $databaseServerName -DatabaseName $databaseName -DatabaseUser $DatabaseUser -DatabasePassword $DatabasePassword -StorageConnectionString $storageConnectionString
}

end
{
}