[System.Reflection.Assembly]::LoadWithPartialName("Gorba.Center.Common.ServiceModel")
Import-Module "OpenXmlPowerTools"

function Get-Tenant
{
	[CmdletBinding()]
	param
	(
		[Parameter(Mandatory = $true)] [string] $ExcelPath
	)
}

function Get-Unit
{
	[CmdletBinding()]
	param
	(
		[Parameter(Mandatory = $true)] [string] $ExcelPath
	)
}

function Export-Tenant
{
	[CmdletBinding()]
	param
	(
		[Parameter(Mandatory = $true)] [string] $ExcelPath
	)
}

Export-ModuleMember "Get-Unit","Get-Tenant"