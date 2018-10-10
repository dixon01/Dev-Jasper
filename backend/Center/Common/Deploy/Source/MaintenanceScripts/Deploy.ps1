<#
    .SYNOPSIS
    This script copies files needed for deployment. Optionally, it can use the ArchiveLogs script to archive existing logs, and reset IIS and/or Itcs clients.

    .DESCRIPTION
    This script copies files needed for deployment. It restarts IIS and/or Itcs clients if the skip switches are not specified. Optionally, it can use the ArchiveLogs script to archive existing logs.
    Is copies all files within the current directory (excluding *.ps1 scripts) into the Products folder (..\Products relative to this script).
    It is assumed that directories have the predefined structure (Products are available at ..\Products, logs are available at ..\Logs and
    the archive script is available at ..\Logs\ArchiveLogs.ps1).
    Logs archived to ..\Logs\Archive (which is created if it doesn't exist). Logs are archived before the copy of files and
    stopping IIS and Itcs clients. At the end of the script IIS and Itcs clients will be restarted.
    This script is intended to be in the folder D:\Gorba\Center\Deployment within a standard installation.
    WARNING: If the ArchiveLogs is run skipping the reset of some running applications, it is possible that a process blocks the archiving procedure leading to unexpected errors/results.
    Optionally HTTP GET requests are sent to http://localhost/BackgroundSystem and http://localhost/Center.

    .PARAM SkipIISReset
    When this switch is passed IIS is not reset.

    .PARAM SkipCommSReset
    When this switch is passed CommS os stopped at the beginning of the script and restarted at the end after CommSRestartDelay seconds.

    .PARAM SkipItcsClientsReset
    When this switch is passed all ItcsClients are stopped at the beginning of the script and restarted at the end after ItcsClientsRestartDelay seconds.

    .PARAM ArchiveLogs
    When this switch is passed the logs are archived using the ArchiveLogs script.

    .PARAM Ping
    When this switch is passed HTTP GET requests are sent to http://localhost/BackgroundSystem and http://localhost/Center.
	
	.PARAM Interactive
	If the flag is set, the user will be prompted for each application to start. All Delays are ignored.

    .PARAM CommSRestartDelay
    Time to wait before restarting CommS (by default, 0s)

    .PARAM IISRestartDelay
    Time to wait before restarting IIS (by default, 0s)

    .PARAM ItcsClientsRestartDelay
    Time to wait before restarting ItcsClients (they are all started in sequence after the delay). By default, 0s.
#>
param
(
    [Parameter()] [switch] $SkipIISReset,
    [Parameter()] [switch] $SkipCommSReset,
    [Parameter()] [switch] $SkipItcsClientsReset,
    [Parameter()] [switch] $ArchiveLogs,
    [Parameter()] [switch] $Ping,
    [Parameter()] [switch] $Interactive,
    [Parameter()] [System.TimeSpan] $CommSRestartDelay = [System.TimeSpan]::Zero,
    [Parameter()] [System.TimeSpan] $IISRestartDelay = [System.TimeSpan]::Zero,
    [Parameter()] [System.TimeSpan] $ItcsClientsRestartDelay = [System.TimeSpan]::Zero
)

begin
{
    function Get-ScriptDirectory
	{
        <#
            .SYNOPSIS
            Gets the path where the script is executed
        #>
		process
        {
            $Invocation = (Get-Variable MyInvocation -Scope 1).Value
		    Split-Path $Invocation.MyCommand.Path
        }
	}

    function Ping-System
    {
        <#
            .SYNOPSIS
            Executes an HTTP GET request to the specified address.

            .PARAM Address
            The address for the HTTP GET request.
        #>
        param
        (
            [Parameter(Mandatory = $true, Position = 1)] [System.Uri] $Address
        )

        process
        {
            Write-Host "Pinging '$($Address)'..."
            $request = [System.Net.WebRequest]::Create($Address)
            [System.Net.HttpWebResponse] $response = $request.GetResponse()
            Write-Host "Ping response status code: '$($response.StatusCode)'"
        }
    }

    function Start-ItcsClient
    {
        param
        (
            [Parameter(Mandatory = $true)] [string] $itcsClientPath
        )

        begin
        {
            $directory = Split-Path $itcsClientPath -Parent
        }

        process
        {
            Write-Host "Starting Itcs client '$($itcsClientPath)' with working directory '$($directory)'"
            Start-Process -FilePath $itcsClientPath -WorkingDirectory $directory -WindowStyle Minimized
        }
    }

    $scriptDirectory = Get-ScriptDirectory
    $productsDirectory = Resolve-Path(Join-Path $scriptDirectory "..\Products")

    if (-not(Test-Path($productsDirectory)))
    {
        throw "Products directory '$($productsDirectory)' was not found"
    }

    if ($ArchiveLogs)
    {
        $logsDirectory = Join-Path $scriptDirectory "..\Logs"
        $logDirectories = @(ls $logsDirectory -Include "BackgroundSystem", "Online")
        foreach($dir in (ls "$($logsDirectory)\ItcsClient\*" | ?{ $_.PSIsContainer }))
        {
            $logDirectories += $dir
        }

        $archiveLogsDirectory = Join-Path $logsDirectory "Archive"
        if (-not(Test-Path $archiveLogsDirectory))
        {
            New-Item -ItemType Directory $archiveLogsDirectory -ErrorAction Stop | Out-Null
        }

        $archiveLogsScript = Join-Path $logsDirectory "ArchiveLogs.ps1"

        if (-not(Test-Path($archiveLogsScript)))
        {
            throw "Archive logs script '$($archiveLogsScript)' was not found"
        }
    }

    if (-not($SkipCommSReset))
    {
        $commsPath = Join-Path $productsDirectory "Comm.S\CommsApp.exe"
        if (-not(Test-Path $commsPath))
        {
            Write-Host "CommS path '$($commsPath)' not found"
        }
    }

    if (-not($SkipItcsClientsReset))
    {
        $itcsClientsDirectory = Join-Path $productsDirectory "ItcsClient"
        if (Test-Path $itcsClientsDirectory)
        {
            $itcsClients = @(ls $itcsClientsDirectory -Recurse -Include "*ItcsClientConsole.exe")
			if ($itcsClients.Count -gt 0)
			{
				Write-Host "Found $($itcsClients.Count) Itcs client(s)"
			}
			else
			{
				Write-Host "Directory '$($itcsClientsDirectory)' is empty"
			}
        }
        else
        {
            $itcsClients = @()
            Write-Host "Directory '$($itcsClientsDirectory)' not found"
        }
    }
}

process
{
    if (-not($SkipItcsClientsReset))
    {
        Get-Process -Name ItcsClientConsole -ErrorAction SilentlyContinue | Stop-Process
    }

    if (-not($SkipIISReset))
    {
        iisreset /stop
    }

    if (-not($SkipCommSSReset))
    {
        Get-Process -Name CommsApp -ErrorAction SilentlyContinue | Stop-Process
    }

    if ($ArchiveLogs)
    {
        Write-Host "Executing '$($archiveLogsScript)'"
        $logDirectories | %{ & $archiveLogsScript -Directory (Resolve-Path $_) -Target $archiveLogsDirectory -PeriodicTimeMinutes 0 -RemoveUncompressedFiles }
    }

    cp "$($scriptDirectory)\*" $productsDirectory -Recurse -Exclude *.ps1 -Force -Verbose
	
    if (-not($SkipCommSReset) -and (Test-Path $commsPath))
    {
		if ($Interactive)
		{
			Read-Host "Type <Enter> to start the Comm.S" | Out-Null
        }
		else
		{
			Start-Sleep -Seconds $CommSRestartDelay.TotalSeconds
		}
		
        $commsDirectory = Split-Path $commsPath -Parent
        Write-Host "Starting Comm.S '$($commsPath)' in '$($commsDirectory)'"
        Start-Process -FilePath $commsPath -WorkingDirectory $commsDirectory -WindowStyle Minimized
    }

    if (-not($SkipIISReset))
    {
        
		if ($Interactive)
		{
			Read-Host "Type <Enter> to start IIS" | Out-Null
        }
		else
		{
			Start-Sleep -Seconds $IISRestartDelay.TotalSeconds
		}
		
        iisreset /start
    }
	
    if ($Ping)
    {
        Ping-System "http://localhost/BackgroundSystem"
        Ping-System "http://localhost/Center"
    }

    if (-not($SkipItcsClientsReset) -and ($itcsClients.Length -gt 0))
    {
        
		if ($Interactive)
		{
			Read-Host "Type <Enter> to start Itcs clients" | Out-Null
        }
		else
		{
			Start-Sleep -Seconds $ItcsClientsRestartDelay.TotalSeconds
		}
		
        foreach ($itcsClient in $itcsClients)
        {
            Start-ItcsClient $itcsClient
        }
    }
}