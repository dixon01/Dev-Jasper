<#
	.Authors
	Francesco Leonetti (LEF, francesco.leonetti@gorba.com)
	
	.Version
	0.1.0
	Created: 06.12.11 (LEF)
	Last updated: 10.09.12 (LEF)
	
	.Synopsis
	This script generates the sql script needed to recreate a database, including database version info in the DatabaseVersionSet table.
	
	.Remarks
	This script must run under .NET 4.0
	To add new version and comments, use the DatababaseVersion and DatabaseDescription parameters

    .Example    
    How to export a database:
    [Relative or absolute path]\ExportDatabase.ps1 -DatabaseServer ".\SqlExpress" -UseLocalSecurity -TargetScriptPath "path_to_output.sql" -SkipVersioning
    How to export a database locally:
    PS [Relative or absolute path]> .\ExportDatabase.ps1 -DatabaseServer "localhost" -UseLocalSecurity -TargetScriptPath "path_to_output.sql" -SkipVersioning 
#>
param
(
	[Parameter(Mandatory = $true)] $DatabaseServer = "192.168.1.158",
	[Parameter()] $DatabaseUser = "sa",
	[Parameter()] $DatabasePassword,
	[Parameter()] $DatabaseName = "Gorba_CenterOnline",
	[Parameter(Mandatory = $true)] $TargetScriptPath,
	[Parameter()] [System.Version] $DatabaseVersion,
	[Parameter()] [string] $DatabaseVersionDescription,
	[Parameter()] [string] $DatabaseVersionDate,
	[Parameter()] [string] $TargetDatabaseName,
	[Parameter()] [switch] $UseLocalSecurity,
    [string] $DatabaseLogin = "gorba_center_online",
    [string] $DatabaseLoginPassword = "gorba",
    [string[]] $AdditionalRoles = @("db_executor"),
    [string[]] $AdditionalSchemas = @(),
    [switch] $SkipVersioning
)

#region Helper functions.

function GetScriptDirectory()
{
	$Invocation = (Get-Variable MyInvocation -Scope 1).Value
	Split-Path $Invocation.MyCommand.Path -Parent
}

#endregion

$TargetScriptDir = Split-Path $TargetScriptPath -Parent
	
	$header = "USE [master]
GO

IF NOT EXISTS(SELECT name FROM sys.sql_logins WHERE name = '$($DatabaseLogin)')
BEGIN
    CREATE LOGIN [$($DatabaseLogin)] WITH PASSWORD=N'$($DatabaseLoginPassword)', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
END


"
	Write-Host "Writing to $($TargetScriptPath)"
	Clear-Content $TargetScriptPath
	
	$TempScriptPath = [System.IO.Path]::GetTempFileName()
	
	$header | Out-File -FilePath $TargetScriptPath -Encoding Unicode
	
	##################################################
	# Loading the Microsoft.SqlServer.Smo assembly
	##################################################
	
	Write-Output "Loading the required assemblies"
	
	[reflection.assembly]::LoadWithPartialName("Microsoft.SqlServer.ConnectionInfo, Version=10.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91") | Out-Null
	[reflection.assembly]::LoadWithPartialName("Microsoft.SqlServer.Smo, Version=10.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91") | Out-Null
	
	Add-Type -AssemblyName "Microsoft.SqlServer.Management.Sdk.Sfc, Version=10.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
	if($UseLocalSecurity)
	{
	    $serverConnection = New-Object "Microsoft.SqlServer.Management.Common.ServerConnection"($DatabaseServer)
		$serverConnection.LoginSecure = $true
	}
	else
	{
	    $serverConnection = New-Object "Microsoft.SqlServer.Management.Common.ServerConnection"($DatabaseServer, $DatabaseUser, $DatabasePassword)
        Write-Host "Connecting as $($DatabaseUser)"
	}
	
	Write-Debug "Connecting to server $($DatabaseServer) with username $($DatabaseUser)"
	$s = new-object ('Microsoft.SqlServer.Management.Smo.Server') $serverConnection
	Write-Debug "Searching database $($DatabaseName)"
	$db = $s.Databases[$DatabaseName]
	Write-Debug "There are $($db.Tables.Count) tables in the database"
	[Microsoft.SqlServer.Management.Smo.SqlSmoObject[]] $smobjs = @($db)
	$scrp = new-object ('Microsoft.SqlServer.Management.Smo.Scripter') ($s)


    if ($AdditionalRoles -and $AdditionalRoles.Count -gt 0)
    {
        $roles = $db.Roles | ? { $AdditionalRoles -contains $_.Name }
        if ($roles.count)
        {
            Write-Host "Adding $($roles.Count) roles: $roles"
        }
        else
        {
            Write-Host "Adding 1 role: $roles"
        }
        $smobjs += $roles
    }

    if ($AdditionalSchemas -and $AdditionalSchemas.Count -gt 0)
    {
        $schemas = $db.Schemas | ? { $AdditionalSchemas -contains $_.Name }
        if ($schemas.count)
        {
            Write-Host "Adding $($schemas.Count) schemas"
        }
        else
        {
            Write-Host "Adding 1 schema"
        }
        $smobjs += $schemas
    }
	
	$dbUser = $db.Users[$DatabaseLogin]
	Write-Debug "DB user: $($dbUser)"
	 
	$scrp.Options.AppendToFile = $true
	$scrp.Options.ClusteredIndexes = $True
	$scrp.Options.DriAll = $True
	$scrp.Options.ScriptDrops = $false
	$scrp.Options.IncludeHeaders = $true
	$scrp.Options.IncludeDatabaseContext = $true
	$scrp.Options.ToFileOnly = $true
	$scrp.Options.Indexes = $true
	$scrp.Options.WithDependencies = $false
	$scrp.Options.NoFileGroup = $true
	$scrp.Options.IncludeDatabaseRoleMemberships = $true
	$scrp.Options.Permissions = $true
	$scrp.Options.FileName = $TempScriptPath
	$smobjs += $dbUser
	$scrp.Script($smobjs)
	
	Write-Debug "First script"
	
	$scrp.Options.WithDependencies = $true
	$scrp.Options.AppendToFile = $true
	
	[Microsoft.SqlServer.Management.Smo.SqlSmoObject[]] $smobjs = @()
	$smobjs += $db.Tables | ? {-not($_.IsSystemObject)}
    $views = $db.Views | ? {-not($_.IsSystemObject)}
    if($views)
    {
	    $smobjs += $views
    }
	
	$functions = $db.UserDefinedFunctions | ? {-not($_.IsSystemObject)}
	if ($functions)
	{
		$smobjs += $functions
	}
	
	$procedures = $db.StoredProcedures | ? {-not($_.IsSystemObject)}
	if ($procedures)
	{
		$smobjs += $procedures
	}
	
	$tableTypes = $db.UserDefinedTableTypes | ? {-not($_.IsSystemObject)}
	if ($tableTypes)
	{
		$smobjs += $tableTypes
	}

    	$scrp.Script($smobjs)

	if(!$SkipVersioning)
{
	$scrp = new-object 'Microsoft.SqlServer.Management.Smo.Scripter' ($s)
	$scrp.Options.ScriptSchema = $false
	$scrp.Options.ScriptData = $true
	$dbVersionTable = $db.Tables["DatabaseVersionSet"]
	Write-Host $dbVersionTable.Urn.GetType()
	$urns = @($dbVersionTable.Urn)
	foreach ($line in $scrp.EnumScript($urns))
	{
		Write-Verbose "Adding the new line $($line) to the script"
		Add-Content $TempScriptPath "`n" -Encoding Unicode
		Add-Content $TempScriptPath $line -Encoding Unicode
		Add-Content $TempScriptPath "GO" -Encoding Unicode
	}
	
	if ($DatabaseVersion)
	{
		Write-Verbose "Updating database version"
		$newVersion = "
GO
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version $($DatabaseVersion)'
  ,@description = '$($DatabaseVersionDescription)'
  ,@versionMajor = $($DatabaseVersion.Major)
  ,@versionMinor = $($DatabaseVersion.Minor)
  ,@versionBuild = $($DatabaseVersion.Build)
  ,@versionRevision = $($DatabaseVersion.Revision)
  ,@dateCreated = '$($DatabaseVersionDate)'
GO"

		Add-Content $TempScriptPath "`n$($newVersion)" -Encoding Unicode
	} # if ($DatabaseVersion)
}
	
$content = Get-Content $TempScriptPath

if($TargetDatabaseName)
{
	$content | %{ $_ -replace "\[$($DatabaseName)\]", "[$($TargetDatabaseName)]" } | Set-Content $TempScriptPath -Encoding Unicode
	$content = Get-Content $TempScriptPath
}

Add-Content $TargetScriptPath $content -Encoding Unicode

Write-Host "Database generated"