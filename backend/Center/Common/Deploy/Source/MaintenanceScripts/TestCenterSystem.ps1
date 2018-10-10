<#
    .SYNOPSIS
    This script performs a simple checkup of a local system.

    .DESCRIPTION
    This script performs a simple checkup of the system.
    The system verifies access to all databases and, if the flag -SkipServices is not set, also verifies access to services.

    .REMARKS
    The script only works locally for a default setup of the system!

    .PARAMETER SkipServices
    If set, checking for services will not be done

    .PARAMETER ServerInstance
    Specifies the Sql server instance. By default, local (.). Use ".\SqlExpress" if Sql express is installed.
#>
param
(
    [switch] $SkipServices,
    [Parameter(Position = 1)] $ServerInstance = "."
)

begin
{
    Add-PSSnapin SqlServerCmdletSnapin100 -ErrorAction Stop
    if (!$SkipServices)
    {
        Write-Verbose "Importing CenterCmdlets module"
        Import-Module CenterCmdlets -ErrorAction Stop
    }
}

process
{
    try
    {
        Write-Host "Trying to access Gorba_CenterOnline database..."
        $sql = "SELECT COUNT(*) FROM [Units]"
        Invoke-Sqlcmd -ServerInstance $ServerInstance -Database "Gorba_CenterOnline" -Username "gorba_center_online" -Password "gorba" -Query $sql -ErrorAction Stop | Out-Null
        Write-Host -BackgroundColor Green -ForegroundColor DarkBlue "..Gorba_CenterOnline ok!"
        
        Write-Host "Trying to access Gorba_CenterControllers database..."
        $sql = "SELECT COUNT(*) FROM [System.Activities.DurableInstancing].[Instances]"
        Invoke-Sqlcmd -ServerInstance $ServerInstance -Database "Gorba_CenterControllers" -Username "gorba_center_controllers" -Password "gorba" -Query $sql -ErrorAction Stop | Out-Null
        Write-Host -BackgroundColor Green -ForegroundColor DarkBlue "..Gorba_CenterControllers ok!"
        
        Write-Host "Trying to access Gorba_CenterControllersMetabase database..."
        $sql = "SELECT COUNT(*) FROM [ActivityInstanceControllers]"
        Invoke-Sqlcmd -ServerInstance $ServerInstance -Database "Gorba_CenterControllersMetabase" -Username "gorba_center_controllers_metabase" -Password "gorba" -Query $sql -ErrorAction Stop | Out-Null
        Write-Host -BackgroundColor Green -ForegroundColor DarkBlue "..Gorba_CenterControllersMetabase ok!"

        if (!$SkipServices)
        {
            Write-Host "Testing UnitService"
            $s = New-BasicHttpBindingProxyConfiguration http://localhost/BackgroundSystem/UnitService.svc
            $f = New-FilterBase -Take 1
            $units = Get-Unit -ProxyConfiguration $s -Filter $f | Measure-Object
            if ($units.Count -ne 1)
            {
                Write-Warning "No error, but no units returned. The system may be not initialized with default data"
            }
            else
            {
                Write-Host -BackgroundColor Green -ForegroundColor DarkBlue "..UnitService ok!"
            }
        }
    }
    catch
    {
        Write-Error "Error occurred. Your configuration may be wrong (did you also specify the right ServerInstance?)"
        Write-Error "Exception: $($_.Exception)"
    }
}