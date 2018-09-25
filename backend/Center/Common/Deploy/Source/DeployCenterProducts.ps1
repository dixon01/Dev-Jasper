<#
#>
param
(
    [Parameter(Mandatory = $true)] [string] $ProductsRoot,
    [string[]] $ItcsClients
)

begin
{
    function Get-ScriptDirectory
    {
        $Invocation = (Get-Variable MyInvocation -Scope 1).Value
        Split-Path $Invocation.MyCommand.Path
    }

    function Publish-ItcsClients
    {
        param
        (
            [string] $ProductsRoot,
            [string[]] $ItcsClients,
            [string] $ItcsClientProductDeployment
        )

        process
        {
            $ItcsClientsRoot = Join-Path $ProductsRoot "ItcsClient"

            foreach($ItcsClient in $ItcsClients)
            {
                Write-Host "Processing Itcs client '$($ItcsClient)'"
                $ItcsClientExtra = Join-Path $ItcsClientProductDeployment $ItcsClient
                if(-not(Test-Path $ItcsClientExtra))
                {
                    Write-Warning "ItcsClient deployment folder '$($ItcsClientExtra)' not found. No extra folder will be specified"
                    $ItcsClientExtra = $null
                }

                $ItcsClientDestination = Join-Path $ItcsClientsRoot $ItcsClient
                if(-not(Test-Path $ItcsClientDestination))
                {
                    New-Item $ItcsClientDestination -ItemType Directory | Out-Null
                }

                Write-Host "Publishing '$($ItcsClient)' to destination '$($ItcsClientDestination)'. This could require some time"
                Publish-LastCenterProductBuild -BuildDefinition "Center_ItcsClient" -Destination $ItcsClientDestination -Extra $ItcsClientExtra
            }
        }
    }

    Import-Module CenterModule

    $scriptDirectory = Get-ScriptDirectory
    $ItcsClientProductDeployment = Join-Path $scriptDirectory "..\..\..\ItcsClient\Deploy\ProductDeployment"
}

process
{
    if ($ItcsClients -and ($ItcsClients.Count -gt 0))
    {
        Write-Host "Specified $($ItcsClients.Count) Itcs clients"
        Publish-ItcsClients -ProductsRoot $ProductsRoot -ItcsClients $ItcsClients -ItcsClientProductDeployment $ItcsClientProductDeployment
    }
    else
    {
        Write-Host "No ItcsClient specified"
    }
}

end
{
}