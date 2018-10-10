<#
	.SYNOPSIS
	
	Imports itcs mapping texts into database from csv files.
	
	.PARAMETER WorkingDirectory
    Path of the working directory. Default value is the directory from where the script is executed.

    .PARAMETER ServicesBaseAddress
    The script uses a background services to import data into the database. the parameter is the base address to access to these services.

    .PARAMETER ResetIis
    Indicates if the script restarts IIS after adding all entries.

    .PARAMETER ResetMappings
    Indicates if the script deletes all existing entries in database before importing the new ones.

#>
param
(
	[string] $Directory = ".",
	[string] $ServicesBaseAddress = "net.tcp://localhost/BackgroundSystem",
    [switch] $ResetMappings,
    [switch] $ResetIis
)

begin
{	
	Write-Host "$ServicesBaseAddress"
	Remove-Module CenterCmdlets -ErrorAction SilentlyContinue

    # The script use the CenterCmdlets, so it needs to import and load ithem :
	Import-Module CenterCmdlets
	$centerCmdlets = Get-Module CenterCmdlets
	if(!$centerCmdlets)
	{
		throw "Can't find the CenterCmdlets module. Please ensure that it is available to the PS host."
		exit
	}

    if(Test-Path $Directory)
    {
        Write-Verbose "The given path exists and will be used as working directory."
        $Directory = Resolve-Path $Directory
        Write-Host "Directory: $($Directory)"
    }
    else
    {
        Write-Error "The given path for the working directory does not exist."
        exit
    }

    $maxInt = [int32]::MaxValue
	$quota = New-Object Gorba.Center.BackgroundSystem.PowerShell.Proxy.BindingQuotaSettings
	$quota.MaxReceivedMessageSize = $maxInt
    $mappingService = New-CertificateAuthenticatedClientProxyConfiguration "$ServicesBaseAddress/ItcsTextMappingService.svc/Certificate" -DnsName "BackgroundSystem" -CertificateName "CenterOnline" -BindingQuotaSettings $quota
    $unitService = New-CertificateAuthenticatedClientProxyConfiguration "$ServicesBaseAddress/UnitService.svc/Certificate" -DnsName "BackgroundSystem" -CertificateName "CenterOnline" -BindingQuotaSettings $quota
    $itcsDataService = New-CertificateAuthenticatedClientProxyConfiguration "$ServicesBaseAddress/ItcsConfigDataService.svc/Certificate" -DnsName "BackgroundSystem" -CertificateName "CenterOnline" -BindingQuotaSettings $quota
}

process
{
    function Get-Filenames([String]$directory)
    {
        <#
            .SYNOPSIS
            Gets all child file names of the base directory.

            .PARAMETER directory
            The parent directory.
        #>  

        $names = @()
        $items = Get-ChildItem -Path $directory
        foreach ($item in $items)
        {
            if ($item.Name.EndsWith(".csv"))
            {
                $names += $item.Name
            }
        }

        return $names
    }

    function Get-ProductTypeFromId([int] $id)
    {
        <#
            .SYNOPSIS
            Gets the ProductType with the given id

            .PARAMETER id
            The identifier of the ProductType
        #>  
        $productType = Get-ProductType $unitService -Id $id
        return $productType
    }

    function Add-Mappings([String]$filename)
    {
        
        <#
            .SYNOPSIS
            Parses all entries of the csv to an ItcsTextMapping object and adds it to an array.

            .PARAMETER providerName
            The name of the ItcsProvider associated with the text mapping entries.

            .PARAMETER filename
            The csv file to import.
        #>

        $ref = Join-Path $Directory $filename
        Write-Host "Import file" $filename
        $filter = New-ItcsTextMappingFilter
        $csv = Import-Csv $ref -Delimiter ','
        foreach ($entry in $csv)
        {
            $provider = Get-ItcsProvider $itcsDataService $entry.ProviderName
            
			$mappedTexts = @()
            $map = New-ItcsTextMapping -ItcsProviderId $provider.Id  -Type $entry.Type  -SourceText $entry.SourceText
                
            if ($entry.TtsText)
            {
                $map.TtsText = $entry.TtsText
            }
            
            if ($entry.ProductTypeId)
            {
                $map.ProductType = Get-ProductTypeFromId($entry.ProductTypeId)
            }
                
            if ($entry.MappedText)
            {
                $map.MappedText = $entry.MappedText
            }

            if ($entry.LastUsed)
            {
                $map.LastUsed = [DateTime]::Parse($entry.LastUsed)
            }
                
            Write-Verbose $map
            Write-Output $map
        }
    }
    
    if ($ResetMappings)
    {
        Write-Host "Resetting all entries..."
        Reset-ItcsTextMapping $mappingService -DeleteEntry
    }

    $mappedTexts = @()

    $filenames = Get-Filenames $Directory
        
    foreach ($file in $filenames)
    {
		$mappings = Add-Mappings $file
		if ($mappings)
		{
			$mappedTexts += $mappings
		}
    }
    
    $mapCount = $mappedTexts.Count
    Write-Host "Adding $mapCount entries to database"

    $mappedTexts | %{ Set-ItcsTextMapping $mappingService $_ } | Out-Null

    Write-Host

    if ($ResetIis)
    {
        if (-not([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(
        [Security.Principal.WindowsBuiltInRole] "Administrator"))
        {
            Write-Error "You do not have Administrator rights to reset IIS!`nPlease reset IIS manually!"
            exit
        }

        iisreset
    }
    else
    {
        Write-Warning "Please reset IIS so that the changes take effect!"
    }
}

end
{
}
