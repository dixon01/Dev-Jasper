<#
    .SYNOPSIS
    Copies nuget.exe where build.cake scripts are found

    .DESCRIPTION
    Copies nuget.exe where build.cake scripts are found.
    The executable is searched recursively from the specified path until the root of the drive with name 'nuget.exe'. If
    not found, it is downloaded
#>
param
(
    # Root directory
    [Parameter(Mandatory = $false)]
    [string]
    $RootPath
)

begin
{
    function Find-Nuget
    {
        param
        (
            # Base path to search
            [Parameter(Mandatory = $true)]
            [string]
            $BasePath
        )

        process
        {
            $exe = Join-Path $BasePath "nuget.exe"
            if (Test-Path $exe)
            {
                Write-Host "Nuget found at '$($exe)'"
                return $exe
            }

            $parent = Split-Path -Parent $BasePath
            if ($parent)
            {
                return Find-Nuget $parent    
            }

            return $null
        }
    }

    function Get-Nuget
    {
        param
        (
            # Target directory
            [Parameter(Mandatory = $true)]
            [string]
            $TargetDirectory
        )

        process
        {
            $url = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
            if (-not(Test-Path $TargetDirectory))
            {
                Write-Host "Creating directory '$($TargetDirectory)'"
                New-Item -ItemType Directory $TargetDirectory | Out-Null
            }

            $nuGetPath = Join-Path $TargetDirectory "nuget.exe"

            if (Test-Path $nuGetPath)
            {
                return
            }

            try
            {
                Write-Host "Downloading for '$($url)' to '$($nuGetPath)'"
                    (New-Object System.Net.WebClient).DownloadFile($url, $nuGetPath)
                return $nuGetPath
            }
            catch
            {
                Write-Error $_.Exception.Message
                throw "Could not download NuGet.exe."
            }
        }
    }
}

process
{
    if (-not($RootPath))
    {
        $RootPath = $PSScriptRoot
        Write-Host "Root not specified. Using $($RootPath)"
    }

    Write-Host "Searching nuget"
    $exe = Find-Nuget $RootPath
    if ($exe)
    {
        Write-Host "Nuget found at '$($exe)'. Trying to update it"
        try
        {
            & $exe update -self
        }
        catch
        {
            Write-Host "Error while trying to update nuget: $($_.Message)"
        }
    }
    else
    {
        Write-Host "Nuget not found."
        $exe = Get-Nuget -TargetDirectory $RootPath
    }

    Write-Host "Searching cake scripts"
    $cakeFiles = Get-ChildItem $RootPath -Recurse -Include "build.cake"
    foreach ($cakeFile in $cakeFiles)
    {
        $containingPath = Split-Path -Parent $cakeFile
        $toolsPath = Join-Path $containingPath "tools"
        Write-Host "Copying to $($toolsPath)"
        if (-not(Test-Path $toolsPath))
        {
            New-Item -ItemType Directory $toolsPath | Out-Null
        }

        Copy-Item $exe $toolsPath -Force
    }
}

end
{

}