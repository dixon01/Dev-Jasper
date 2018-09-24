<#
	.SYNOPSIS
	Imports itcs mapping texts into database from csv files.
	
	.PARAMETER WorkingDirectory
    Path of the working directory. Default value is ../../Deploy/Source

    .PARAMETER ServicesBaseAddress
    The script uses a background services to import data into the database. the parameter is the base
    address to access to these services.

    .PARAMETER ResetIis
    Indicates if the script restarts IIS after adding all entries.

    .PARAMETER ResetMappings
    Indicates if the script deletes all existing entries in database before importing the new ones.
#>
param
(
    [Parameter()]
	[string]
    $WorkingDirectory = "../../Deploy/Source",

    [Parameter()]
	[string] 
    $ServicesBaseAddress = "net.tcp://localhost/BackgroundSystem",

    [Parameter()]
    [switch] 
    $ResetMappings,
    
    [Parameter()]
    [switch] 
    $ResetIis
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

    $scriptDirectory = Get-ScriptDirectory

    if(Test-Path $WorkingDirectory)
    {
        Write-Verbose "The given path exists and will be used as working directory."
        $WorkingDirectory = Resolve-Path $WorkingDirectory
        Write-Host "Working directory: $($WorkingDirectory)"
    }
    else
    {
        $WorkingDirectory = Join-Path $scriptDirectory $WorkingDirectory
        if(-not(Test-Path $WorkingDirectory))
        {
            throw "The resulting directory doesn't exist and can't be used."
        }
    }

    $referenceTextDirectory = Join-Path $WorkingDirectory "ReferenceTexts"
    if (-not(Test-Path $referenceTextDirectory))
    {
        throw "The WorkingDirectory doesn't contain the required ReferenceTexts folder"
    }

    $mappingService = New-CertificateAuthenticatedClientProxyConfiguration `        "$ServicesBaseAddress/ItcsTextMappingService.svc/Certificate" `        -MaxReceivedMessageSize 2500000 -DnsName "BackgroundSystem" -CertificateName "CenterOnline"
    $unitService = New-CertificateAuthenticatedClientProxyConfiguration `        "$ServicesBaseAddress/UnitService.svc/Certificate" -DnsName "BackgroundSystem" -CertificateName "CenterOnline"
    $itcsDataService = New-CertificateAuthenticatedClientProxyConfiguration `        "$ServicesBaseAddress/ItcsConfigDataService.svc/Certificate" -DnsName "BackgroundSystem" `        -CertificateName "CenterOnline"
    $script:textType = [Gorba.Center.Common.ServiceModel.DTO.Itcs.ItcsTextType]::Unknown
    $script:productType = $null
    $script:directories = @()
    $script:filenames = @() 
}

process
{
    function Get-Filenames
    {
        <#
            .SYNOPSIS
            Gets all child file names of the base directory.

            .PARAMETER directory
            The parent directory.
        #>  
        param
        (
            [Parameter()]
            [string]
            $directory
        )

        process
        {
            $script:filenames = @()
            $items = Get-ChildItem -Path $directory
            foreach ($item in $items)
            {
                $script:filenames += $item.Name
            }
        }
    }

    function Get-DirectoryNames
    {
        <#
            .SYNOPSIS
            Gets all child directory names of the base reference text directory.
        #>
        param
        (
        )

        process
        {
            $directories = Get-ChildItem -Path $referenceTextDirectory
            foreach ($item in $directories)
            {
                $script:directories += $item.Name
            }
        }
    }

    function Add-Mappings
    {
        <#
            .SYNOPSIS
            Parses all entries of the csv to an ItcsTextMapping object and adds it to an array.

            .PARAMETER providerName
            The name of the ItcsProvider associated with the text mapping entries.

            .PARAMETER filename
            The csv file to import.
        #>
        param
        (
            [Parameter()]
            [String]$providerName, 
            
            [Parameter()]
            [String]$filename
        )

        process
        {
            $provider = Get-ItcsProvider $itcsDataService $providerName
            $ref = Join-Path $script:clientTextsDirectory $filename
            Write-Host "Import file" $filename
            $csv = Import-Csv $ref -Delimiter ';'
            Get-ProductTypeFromFilename $filename
            Get-TextType $filename
            foreach ($entry in $csv)
            {
                    $ttsText = $null

                    if ($entry.TTsText)
                    {
                        $ttsText = $entry.TTsText
                    }
        
                    $map = New-ItcsTextMapping -ItcsProviderId $provider.Id -Type $script:textType `                                -SourceText $entry.OriginalText -TtsText $ttsText
                    if ($script:productType)
                    {
                        $map.ProductType = $script:productType
                        $key = ($entry.ReferenceNumber + $script:productType.Name)
                    }
                    else
                    {
                        $key = $entry.ReferenceNumber
                    }
                
                    if ($entry.MappingText)
                    {
                        $map.MappedText = $entry.MappingText
                    }
                
                    $mappedTexts.Add($key, $map)
                Write-Verbose $map 
            }
        }
    }

    function Get-ProductTypeFromFilename
    {
        <#
            .SYNOPSIS
            Get the product type according to the type part of the filename

            .PARAMETER filename
            The filename which contains the product type. E.g. ML for dest.ML.csv or null for dest.csv
        #>
        param
        (
            [Parameter()]
            [string]
            $filename
        )

        process
        {
            $script:productType = $null

            $splitted = $filename.Split(".")
        
            if ($splitted.Count -eq 3)
            {
                $script:productType = Get-ProductType $unitService -Name $splitted[1]
            }
        }
    }

    function Get-TextType
    {
        <#
            .SYNOPSIS
            Assign the ItcsTextType according to the first part of the filename

            .PARAMETER filename
            The filename which contains the text type. E.g. destination for dest.csv or dest.ML.csv
        #>
        param
        (
            [Parameter()]
            [string]
            $filename
        )

        process
        {
            $splitted = $filename.Split(".")
            $type = $splitted[0]

            if ($type -eq "line")
            {
                $script:textType = [Gorba.Center.Common.ServiceModel.DTO.Itcs.ItcsTextType]::Line
            } 
            elseif ($type -eq "dest")
            {
                $script:textType = [Gorba.Center.Common.ServiceModel.DTO.Itcs.ItcsTextType]::Destination
            }
            elseif ($type -eq "lane")
            {
                $script:textType = [Gorba.Center.Common.ServiceModel.DTO.Itcs.ItcsTextType]::Lane
            }
            else
            {
                $script:textType = [Gorba.Center.Common.ServiceModel.DTO.Itcs.ItcsTextType]::Unknown
            }
        }
    }

    
    if ($ResetMappings)
    {
        Write-Host "Resetting all entries..."
        Reset-ItcsTextMapping $mappingService -DeleteEntry
    }

    $mappedTexts = @{}

    Get-DirectoryNames

    $providerCount = $script:directories.Count
    $progress = (1 / $providerCount) * 100
    $i = 1
    foreach ($directory in $script:directories)
    {
        $mappedTexts.Clear()
        $progress = ([int]$i / [int]$providerCount) * 100
        $script:clientTextsDirectory = Join-Path $referenceTextDirectory $directory
        Get-Filenames $script:clientTextsDirectory
        Write-Progress -Activity "Adding $directory" -Status "Percent completed:" -PercentComplete $progress
        Write-Host "Importing mappings for provider $directory"
        
        foreach ($file in $script:filenames)
        {
            if ($file -ne "reftext.csv")
            {
                Add-Mappings $directory $file        
            }
        }

        $mapCount = $mappedTexts.Count
        Write-Host "Adding $mapCount entries to database"

        foreach ($entry in $mappedTexts.GetEnumerator())
        {
            Add-ItcsTextMapping $mappingService $entry.Value | Out-Null
        }

        $i ++
        Write-Host
    }

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
        Write-Warning "Please reset IIS so that the changes take effect"
    }
}

end
{
}
