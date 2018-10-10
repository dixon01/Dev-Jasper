<#
    .SYNOPSIS
    Verifies that all resource files corresponding to resource entities are available.

    .DESCRIPTION
    Queries the Resource service for all resource entities and verifies that each of them is available
    in the specified Resources folder.

    .PARAMETER ResourcesDirectory
    The directory where resources are stored as {Hash}.rx files.

    .PARAMETER CmdletsModulePath
    Path to the BackgroundSystem cmdlets module (.psd1 file).

    .PARAMETER CenterPortalAddress
    Path to the Center portal serving the configuration for the system.
    If not specified, the default local configuration will be used.
#>
param
(
    [Parameter(Mandatory = $true)]
    [string]
    [ValidateScript({ (Test-Path $_) -and (Get-ChildItem $_).PSIsContainer })]
    $ResourcesDirectory,
    
    [Parameter(Mandatory = $true)]
    [string]
    [ValidateScript({ Test-Path $_ })]
    $CmdletsModulePath,

    [Parameter()]
    [System.Uri]
    $CenterPortalAddress = "local"
)

Import-Module $CmdletsModulePath

Write-Host "Insert 'admin' password"
$password = Read-Host

$credentials = New-UserCredentials -Username "admin" -Password $password
$configuration = Get-BackgroundSystemConfiguration -CenterPortalAddress $CenterPortalAddress

$filter = New-ResourceFilter
$resources = Get-Resource -Filter $filter -UserCredentials $credentials -Configuration $configuration
$unavailableResources = 0
foreach ($resource in $resources)
{
    $expectedResourcePath = Join-Path $ResourcesDirectory "$($resource.Hash).rx"
    if (-not (Test-Path $expectedResourcePath))
    {
        $unavailableResources++
        Write-Warning "Resource $($resource.Id) not found at '$($expectedResourcePath)'"
    }
}

if ($unavailableResources -gt 0)
{
    Write-Error "$($unavailableResources) resource(s) unavailable"
}
else
{
    Write-Host "All resources available"
}