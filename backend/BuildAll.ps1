<#
    .SYNOPSIS
    Builds all solutions in this repository.
#>
param
(
    # Configuration to build (Debug or Release)
    [Parameter()]
    [Alias("Configuration")]
    [ValidateSet("Debug", "Release")]
    [string]
    $BuildConfiguration = "Debug",

    # Flag to skip building Center products
    [Parameter()]
    [switch]
    $SkipCenter,

    # Flag to skip building Common components
    [Parameter()]
    [switch]
    $SkipCommon,

    # Flag to skip building Motion products
    [Parameter()]
    [switch]
    $SkipMotion
)

begin
{
    function Invoke-Build
    {
        param
        (
            [Parameter(Mandatory = $true)]
            [string]
            [ValidateScript({ Test-Path $_ })]
            $Path,

            [Parameter(Mandatory = $true)]
            [string]
            [ValidateSet("Debug", "Release")]
            $BuildConfiguration,

            [Parameter()]
            $Args
        )

        process
        {
            $cake = Join-Path $Path "build.cake"
            $build = Join-Path $Path "build.ps1"
            $packagesToken = "5txh4kae3oiug7salixoajpk6asum2izkjrisreharrjihepdora"
            $scriptArgs = "--packagesToken=$($packagesToken)"
            if ($Args -ne $null)
            {
                foreach ($arg in $Args.Keys)
                {
                    $scriptArgs += " --$($arg)='$($Args[$arg])'"
                }
            }

            $output = & $build -Script $cake -Configuration $buildConfiguration -Target "RunTests" -ScriptArgs $scriptArgs `
                -Verbosity Quiet
            if ($LASTEXITCODE -ne 0)
            {
                throw "Build '$($Path)' didn't succeed"
            }

            Write-Host "Build '$($Path)' successfully completed"
        }
    }

    function Write-BuildInfo
    {
        param
        (
            # Name of the build
            [Parameter(Mandatory = $true)]
            [string]
            $Name
        )

        process
        {
            Write-Host ""
            Write-Host "--------------Build--------------"
            Write-Host "> $($Name)"
            Write-Host "---------------------------------"
            Write-Host ""
        }
    }

    $areas = @{ }
    if (-not($SkipCenter))
    {
        $areas["Center"] =  @(
            @{ "Name" = "Admin" },
            @{ "Name" = "BackgroundSystem" },
            @{ "Name" = "Common" },
            @{ "Name" = "Diag" },
            @{ "Name" = "Media" },
            @{ "Name" = "Portal" })
    }

    if (-not($SkipCommon))
    {
        $areas["Common"] =  @(
            @{ "Name" = "ComponentModel" },
            @{ "Name" = "Configuration" },
            @{ "Name" = "Formats" },
            @{ "Name" = "Gioom" },
            @{ "Name" = "Logging" },
            @{ "Name" = "Medi" },
            @{ "Name" = "Protocols" },
            @{ "Name" = "SystemManagement" },
            @{ "Name" = "Tfs" },
            @{ "Name" = "Update" },
            @{ "Name" = "Utility" },
            @{ "Name" = "VisualStudio" })
    }

    if (-not($SkipMotion))
    {
        $areas["Motion"] =  @(
            @{ "Name" = "Common" },
            @{ "Name" = "ControlUnit" },
            @{ "Name" = "HardwareManager" },
            @{ "Name" = "Infomedia"
                "Args" = @{ "manifestId" = "Gorba.Motion.Infomedia.Composer" } },
            @{ "Name" = "Protran" },
            @{ "Name" = "SystemManager" },
            @{ "Name" = "Update" })
    }
}

process
{
    $start = [System.DateTime]::Now
    Write-Host "Start: $($start)"
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    if (-not($SkipMotion))
    {
        Write-BuildInfo "Motion"
        $path = Join-Path $PSScriptRoot "Motion"
        Invoke-Build -Path $path -BuildConfiguration $BuildConfiguration
    }

    foreach ($area in $areas.Keys)
    {
        $areaStopwatch = [System.Diagnostics.Stopwatch]::StartNew()
        foreach ($project in $areas[$area])
        {
            $path = Join-Path $PSScriptRoot (Join-Path $area $project["Name"])
            Write-BuildInfo "$($area).$($project["Name"])"
            Invoke-Build -Path $path -BuildConfiguration $BuildConfiguration -Args $project["Args"]
        }

        $areaStopwatch.Stop()
        Write-Host "All builds in area '$($area)' succeeded in $($areaStopwatch.Elapsed)"
    }

    $stopwatch.Stop()
    Write-Host "All builds in $($areas.Keys.Count) area(s) started at $($start) succeeded in $($stopwatch.Elapsed)"
}

end
{
}