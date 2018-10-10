<#
	.SYNOPSIS
	This script clear logs of the system
#>
$Path = "C:\temp\Gorba\Center\Logs"

Write-Host "Clearing logs..."
rm "$($Path)\*" -Recurse -Force
Write-Host "Done"