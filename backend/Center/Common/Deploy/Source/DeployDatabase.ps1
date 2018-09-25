<#
	.Authors
	Francesco Leonetti (LEF, francesco.leonetti@gorba.com)
	
	.Version
	0.1.0
	Created: 23.03.12 (LEF)
	Last updated: 23.03.12 (LEF)
	
	.Synopsis
	This script invokes deploys the database to the given instance of sql server. By default, it uses the "." instance.
	The user is prompted for login and password.
	
	.Remarks
	.NET >=4 is required.
	The files are searched by default in the script directory.
	Powershell tools by LEF are required. They must be in a path known by Powershell.

    .PARAMETER ServerInstance
    Name of the server where the database must be deployed.
    Default value is 'localhost'

    .PARAMETER Username
    Username used to get the network credential.
    Default value is 'sa'

    .PARAMETER Scripts
    Array of strings containing the script to deploy the database.
    Default value is "CreateScripts/DropDatabase.sql", "CreateScripts/CreateDatabase.sql"
#>
param
(
	[string] $ServerInstance = "localhost"
	, [string] $Username = "sa"
	, [Array] $Scripts = @("CreateScripts/DropDatabase.sql", "CreateScripts/CreateDatabase.sql")
)

function GetScriptPath()
{
    <#
        .SYNOPSIS
        Gets the path where the script is executed
    #>
	$Invocation = (Get-Variable MyInvocation -Scope 1).Value
	Split-Path $Invocation.MyCommand.Path
}

[System.Reflection.Assembly]::LoadWithPartialName("System.Xml.Linq") | Out-Null
[System.Reflection.Assembly]::LoadWithPartialName("System.Xml.XPath") | Out-Null
[System.Management.Automation.PSCredential] $credential = Get-Credential -Credential $Username

#Region Load sql snapin

if (!(Get-PSSnapin | ?{$_.name -eq 'SqlServerProviderSnapin100'})) 
{ 
	if(Get-PSSnapin -registered | ?{$_.name -eq 'SqlServerProviderSnapin100'}) 
	{ 
	   add-pssnapin SqlServerProviderSnapin100 
	   write-host "Loading SqlServerProviderSnapin100 in session" 
	} 
	else 
	{ 
	   write-host "SqlServerProviderSnapin100 is not registered with the system." -Backgroundcolor Red –Foregroundcolor White 
	   break 
	} 
} 
else 
{ 
  write-host "SqlServerProviderSnapin100 is already loaded" 
}  

# Load SqlServerCmdletSnapin100 

if (!(Get-PSSnapin | ?{$_.name -eq 'SqlServerCmdletSnapin100'})) 
{ 
	if(Get-PSSnapin -registered | ?{$_.name -eq 'SqlServerCmdletSnapin100'}) 
	{ 
	   add-pssnapin SqlServerCmdletSnapin100 
	   write-host "Loading SqlServerCmdletSnapin100 in session" 
	} 
	else 
	{ 
	   write-host "SqlServerCmdletSnapin100 is not registered with the system." 
	   break 
	} 
} 
else 
{ 
  write-host "SqlServerCmdletSnapin100 is already loaded" 
}

#Endregion

$scriptPath = GetScriptPath

# Get-Credential prepends backslash if the domain is not specified. In this case, I will remove it.
$networkCredential = $credential.GetNetworkCredential()

foreach($script in $Scripts)
{
	$path = Join-Path $scriptPath $script
	Invoke-SqlCmd -InputFile $path -ServerInstance $ServerInstance -Username $networkCredential.UserName -Password $networkCredential.Password
}