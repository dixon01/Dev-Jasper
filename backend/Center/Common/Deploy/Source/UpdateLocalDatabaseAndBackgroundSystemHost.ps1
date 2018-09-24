<#
	.SYNOPSIS
	This script generates a local copy of the database and rebuilds the background system solution.
	It requires $psgorba (object that contains the local TFS path pointing to the root project) to be run without the TfsPath parameter.
	
	It works only on PowerShell 3.0!
	
	.PARAMETER TfsPath
	Local path that points to the workspace for the whole Gorba project e.g. "C:\Tfs".
	
	.PARAMETER SkipBuild
	Set to true, only a local copy of the database is generated. The background system solution will not be built.
#>
param
(
	[string] $TfsPath,
	[switch] $SkipBuild
)

begin
{
	function Get-ScriptDirectory
	{
		$Invocation = (Get-Variable MyInvocation -Scope 1).Value
		Split-Path $Invocation.MyCommand.Path
	}

	if(!$TfsPath)
	{
		if(!$psgorba)
		{
			throw "The psgorba object is required to run this script without the TfsPath parameter"
		}
		
		$TfsPath = $psgorba.Tfs
	}
}

process
{
	$CommonDeployPath = Join-Path $TfsPath "Main\Center\Common\Deploy\Source"
	$localDatabaseScript = Join-Path $CommonDeployPath "ExportDatabase\LocalExportCenterDatabase.ps1"
	$exportDatabaseScript = Join-Path $CommonDeployPath "ExportDatabase\ExportCenterDatabase.ps1"

	Write-Host "Exporting the local database... This could take several seconds"
	$output = & $localDatabaseScript
	Write-Host "Exporting the database... This could take several minutes"
	$output = & $exportDatabaseScript -UseLocalDatabase
		
	if (!$SkipBuild)
	{
		$SolutionPath = Join-Path $TfsPath  "Main\Center\Host\Gorba.Center.Host.BackgroundSystem.sln"
		
		$msbuild = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"

		Write-Host "Building the solution... This could take several minutes"
		$output = & $msbuild $SolutionPath --% /p:Configuration=Release /p:Platform="Any CPU"
		if(!$?)
		{
			Write-Host $output
			Write-Error "The build failed. Check the previous output."
            exit
		}
		
		Write-Host "Solution successfully built"
	}
}

end
{
}