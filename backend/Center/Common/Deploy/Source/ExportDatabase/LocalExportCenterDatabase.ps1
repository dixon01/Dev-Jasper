<#
	.SYNOPSIS
	This script uses a temp database generate SQL scripts from the original one.
    The database is exported to the CreateDatabase.sql script.
	
	.REMARKS
	This script should be run under an account with proper permissions to drop and create databases
	This script requires the Sql Server Snapin (it must be installed).
	If either this file or the CreateDatabase.sql is moved, the relative path for the CreateDatabase.sql script must be changed!
	It requires PowerShell Community extensions (http://pscx.codeplex.com/)
	It requires Sql Managent Studio 2008 R2
#>
param
(
	[string] $LocalServerInstance = ".",
	[string] $DatabaseName = "Gorba_CenterOnline",
	[string] $TargetDatabaseName = "Gorba_CenterOnline_Temp",
	[switch] $PromptCredentials # not used now, but useful for future usages
)

begin
{
	function Get-ScriptDirectory()
	{
		$Invocation = (Get-Variable MyInvocation -Scope 1).Value
		Split-Path $Invocation.MyCommand.Path -Parent
	}

	Write-Host "Loading the required modules"
	
	Get-PSSnapin -Registered SqlServerCmdletSnapin100 -ErrorAction Stop | Add-PSSnapin -ErrorAction SilentlyContinue

	if($PromptCredentials)
	{
		# Ask for credentials (and use them later when invoking commands)
	}
	
	$scriptDirectory = Get-ScriptDirectory
	$dropDatabase = Join-Path $scriptDirectory "../CreateScripts/DropDatabase.sql"
	$tempFile = [System.IO.Path]::GetTempFileName()
	$scriptFile = [System.IO.Path]::GetTempFileName()
	$exportDatabaseScript = Join-Path $scriptDirectory "ExportDatabase.ps1"
}

process
{
	$tempFileDropScript = [System.IO.Path]::GetTempFileName()
	
	# change the -UseLocalDatabase if using the promptcredentials
	& $exportDatabaseScript -DatabaseServer $LocalServerInstance -DatabaseName $DatabaseName -TargetScriptPath $tempFile -TargetDatabaseName $TargetDatabaseName -UseLocalSecurity
	
	Write-Host "Replacing the database name in the drop script... This could take several seconds"
	(Get-Content $dropDatabase) | Foreach-Object {$_ -replace "\[$($DatabaseName)\]", "[$($DatabaseName)_Temp]"} | Set-Content $tempFileDropScript -Encoding Unicode
	
	
	Write-Host "Replacing the database name... This could take several seconds"
	(Get-Content $tempFile) | Foreach-Object {$_ -replace "\[$($DatabaseName)\]", "[$($DatabaseName)_Temp]"} | Set-Content $scriptFile -Encoding Unicode
	
	# the following command uses the default database defined for the user, usually master (the one we need)
	Write-Host "Executing the drop database script..."
	Invoke-SqlCmd -ServerInstance $LocalServerInstance -InputFile $tempFileDropScript
	Write-Host "Executing the create database script..."
	Invoke-SqlCmd -ServerInstance $LocalServerInstance -InputFile $scriptFile
	Write-Host $tempFile
}

end
{
	rm $tempFile
	rm $scriptFile
	rm $tempFileDropScript
}