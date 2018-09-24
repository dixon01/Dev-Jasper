<#
    .SYNOPSIS
    This script deploys applications to the Demo system

    .DESCRIPTION
    This script deploys applications to the Demo system taking the last version successfully
    built by the TFS server and using the configuration files found in the repository.
    WARNING: This script must be run as Administrator! CenterModule is also required.

    .PARAMETER CenterSourceRoot
    Defined the root of the Center source directory (it could be int the Main, in one branch for a specific version or just replicated on a system).

    .PARAMETER SkipCommS
    If set, the Comm.S is not deployed

    .PARAMETER SkipItcsClients
    If set, the ItcsClients are not deployed
#>
param
(
    [Parameter(Position = 1)] [string] $CenterSourceRoot = $null,
    [Parameter(Position = 2)] [string] $DemoComMachine = "\\192.168.1.252",
    [switch] $SkipComms,
    [switch] $SkipItcsClients
)

begin
{
    if (-not($CenterSourceRoot))
    {
        if (-not($psgorba))
        {
            throw "If you don't specify the CenterSourceRoot then you must have the `$psgorba object added to your profile containing a `$psgorba.Center.Current property`
            pointing to the current Center source location that will be used as CenterSourceRoot"
        }

        $CenterSourceRoot = $psgorba.Center.Root
    }

    Import-Module CenterModule -ErrorAction Stop

    $ItcsClientProductDeployment = Join-Path $CenterSourceRoot "ItcsClient\Deploy\ProductDeployment"
    $commSExtra = Join-Path $CenterSourceRoot "CommS\Deploy\ProductDeployment\Demo"

    $productsRoot = Join-Path $DemoComMachine "Gorba\Center\Products"
}

process
{
    if($SkipComms)
    {
        Write-Host "Skipping Comm.S"
    }
    else
    {
        Publish-CommS -ProductsRoot $productsRoot -CommSExtra $commSExtra
    }

    if($SkipItcsClients)
    {
        Write-Host "Skipping Itcs clients"
    }
    else
    {
        $ItcsClients = ls $ItcsClientProductDeployment | %{ $_.Name }
        Publish-ItcsClients -ProductsRoot $productsRoot -ItcsClients $ItcsClients -ItcsClientProductDeployment $ItcsClientProductDeployment
    }
}

end
{
}