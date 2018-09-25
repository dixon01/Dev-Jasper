<#
	.SYNOPSIS
	
	Archives (zip) files of a specific directory. By default it archives continuously after the specified periodic time.
	The zip name is the current date and time in format yyyy-mm-dd-hhmm.zip. E.g. 2013-04-12-0855.zip

	.PARAMETER Directory
    Path of the working directory. Default value is .\Logs.

    .PARAMETER Target
    Path of the directory to create the zip files. If empty or not set, the script directory will be used.

    .PARAMETER PeriodicTimeMinutes
    The time in minutes after which the files will be archived. The default value is every 60 minutes. 
    If set to 0, the script archives only once.

    .PARAMETER RemoveUncompressedFiles
    If set all the files within parameter Directory will be removed after archiving.
#>
param
(
	[string] $Directory = "\Logs",
    [string] $Target,
	[int] $PeriodicTimeMinutes = 60,
    [switch] $RemoveUncompressedFiles
)


begin
{
    function Get-ScriptDirectory()
	{
        <#
            .SYNOPSIS
            Gets the path where the script is executed
        #>
		$Invocation = (Get-Variable MyInvocation -Scope 1).Value
		Split-Path $Invocation.MyCommand.Path
	}

    function Get-OutputPath
    {
        <#
            .SYNOPSIS
            Gets the path including the filename of the zip file to write.
            The filename contains the current date and time. E.g. 2013-04-12-0855.zip
        #>
        $date = Get-Date -Format yyyy-MM-dd-hhmm
        if ($Target)
        {
            if (Test-Path $Target)
            {
                $outputPath = $Target + "\" + $date + ".zip"
            }
            else
            {
                Write-Warning "The path '$($Target)' couldn't be resolved. The archive will be created in the script directory."
                $outputPath = $scriptDirectory + "\" + $date + ".zip"
            }
            
            
        }
        else
        {
            $outputPath = $scriptDirectory + "\" + $date + ".zip"
            Write-Host "Archive path: '$($outputPath)'"
        }

        return $outputPath
    }
    
    $scriptDirectory = Get-ScriptDirectory

    Import-Module Pscx -ErrorAction SilentlyContinue | Out-Null

    if(Test-Path $Directory)
    {
        Write-Verbose "The given path exists and will be used as working directory."
        $absolutePath = Resolve-Path $Directory
        Write-Host "Directory: $($Directory)"
    }
    else
    {
        $absolutePath = Join-Path $scriptDirectory $Directory
            if(-not(Test-Path $absolutePath))
            {
			    throw "The path '$($Directory)' couldn't be resolved. The script can't be executed."
			    exit
            }
        $absolutePath = Resolve-Path $absolutePath
        Write-Verbose "The given relative path exists and will be resolved as working directory."
        Write-Host "Directory: $($absolutePath)"
   }
}

process
{
    if ($PeriodicTimeMinutes -eq 0)
    {
        $output = Get-OutputPath
        Write-Zip $absolutePath -OutputPath $output -IncludeEmptyDirectories
        if ($RemoveUncompressedFiles)
        {
            $removeFilePath = Join-Path $absolutePath "*"
            Remove-Item $removeFilePath -Recurse -Verbose -ErrorAction Continue
        }
    }
    else
    {
        while ($true)
        {
            try
            {
                $time = [System.DateTime]::Now.AddMinutes($PeriodicTimeMinutes)
            
                Write-Host "Next archiving at $time."
                Start-Sleep -Seconds ($PeriodicTimeMinutes * 60)
                $output = Get-OutputPath
                Write-Zip $absolutePath -OutputPath $output
          
                if ($RemoveUncompressedFiles)
                {
                    $removeFilePath = Join-Path $absolutePath "*"
                    Remove-Item $removeFilePath -Recurse -Verbose -ErrorAction Continue
                }
            }
            catch
            {
                Write-Error "Error while archiving logs at '$($time)'"
                Write-Host -ForegroundColor Red $_.Exception.ToString()
            }
        }
    }
}

end
{
}