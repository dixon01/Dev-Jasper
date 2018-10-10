function Get-DisplayAreaFilterKey($filter)
{
    <#
        .SYNOPSIS 
        Returns a string containing the properties of the specified $filter object.

        .INPUTS
        $filter. 
        The filter object containing the data that will used to build the formatted string.

        .OUTPUTS
        String. 
        Returns a formatted string representing the specifed $filter object. The output format is :
        [ProviderName]#;#[DisplayAreaName]                 
    #>

    $f = "{0}#;#{1}"
    return [string]::Format($f, $filter.ProviderName, $filter.DisplayAreaName)
}

function Test-DisplayAreasFilter($DisplayAreas, $key)
{
    <#
        .SYNOPSIS 
        Check if the specified filter is contained into the display area list.

        .DESCRIPTION
        If the list of display areas contains the specified filter <Provider name, Display area name>, 
        then returns $True, otherwise returns $False.
            
        .INPUTS
        $DisplayAreas. 
        The list of all display areas.

        $key. 
        The key to find in the list of display areas. This key is composed of Provider name and Display area Name> 

        .OUTPUTS
        boolean. 
        Returns true if the $filter is found into the display area array , otherwize false.      
    #>
    if($DisplayAreas.ContainsKey($key))
    {
        return $true
    }

    return $false
}

function Resolve-RelativePath
{
    <#
        .SYNOPSIS
        Resolves a given relative or absolute path and returns the absolute path.
        If the path does not exist, an empty string is returned.

        .PARAMETER FilePath
        The path to the file used. Can be absolute or relative to the parameter BaseDirectory

        .PARAMETER BaseDirectory
        The base path in case the parameter FilePath is relative.
    #>
    param
    (
        [Parameter(Mandatory = $true)]
        $FilePath,

        [string]
        $BaseDirectory
    )

    process
    {
        [string] $AbsolutePath = ""
        if (Test-Path $FilePath)
	    {
		    $AbsolutePath = Resolve-Path $FilePath
		    Write-Verbose "The path is absolute. Resolved to $($AbsolutePath)"
	    }
	    else
	    {
		    $AbsolutePath = Join-Path $BaseDirectory $FilePath
            Write-Verbose "New path: $($AbsolutePath)"
		    if (Test-Path $AbsolutePath)
            {
                Write-Verbose "File $($FilePath) found in the base directory $($BaseDirectory)"
            }
            else
		    {
                Write-Host "File $($FilePath) not found"
                $AbsolutePath = ""
		    }
	    }

        return $AbsolutePath
    }
}

Export-ModuleMember Get-DisplayAreaFilterKey
Export-ModuleMember Test-DisplayAreasFilter
Export-ModuleMember Resolve-RelativePath