<#
	.SYNOPSIS
	Performs the setup of Center 2.0 on the icenter-dev cloud service.
#>
param
(
)

begin
{
	$scriptPath = Join-Path $PSScriptRoot "..\03_UpdateSystem-2_0.ps1"
	if (-not(Test-Path $scriptPath))
	{
		throw "Script '$($scriptPath)' not found"
	}

	$path = "D:\Dev\Azure\Setup\MigrationTest\v2.2"
	$serviceName = "icenter-dev"
	$databaseServerName = "zbz0us8d5e"
	$databaseName = "icenter-dev"
	$databaseConnectionString = "Server=tcp:$($databaseServerName).database.windows.net,1433;Database=$($databaseName);User ID=icenter@zbz0us8d5e;Password=czPVYhbFAWCDrN9kgLqomUWeLBoGu2XT;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;"
	$storageName = "icenterdev"
	$storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=$($storageName);AccountKey=REvQZGa/Ec8cwwqwEEszEc6LcVIK+0pwa14e8umAgas4IR/pmKXUbmOKWl7GYtAo8cW8WI5itkOpBQkdSdwfLA=="
}

process
{
	& $scriptPath -Path $path -ServiceName $serviceName -DatabaseConnectionString $databaseConnectionString -DatabaseServerName $databaseServerName -DatabaseName $databaseName -StorageConnectionString $storageConnectionString
}

end
{
}