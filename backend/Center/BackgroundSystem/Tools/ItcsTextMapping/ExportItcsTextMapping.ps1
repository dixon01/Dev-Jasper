<#
    .SYNOPSIS
    This script exports the text mapping entries in the database to CSV files that can be later reimported

    .PARAMETER Directory
    The directory where the csv files should be created.

    .PARAMETER ServiceBaseAddress
    The base address to access the background system services.

    .PARAMETER Force
    If the Force flag is set, existing files will be overwritten. Otherwise an exception is thrown if there is already
    an existing file with the same name.
#>
param
(
    [Parameter()] [string] $Directory = ".",
    [Parameter()] [string] $ServiceBaseAddress = "net.tcp://localhost/BackgroundSystem/",
    [Parameter()] [switch] $Force
)

begin
{
    <#
        .SYNOPSIS
        This cmdlet builds and returns the full path for the file optionally testing if the file already exists.

        .DESCRIPTION
        This cmdlet builds and returns the full path for the file using the provided
        Directory and Name properties and optionally tests if the file already exists
        (if the Force flag is not set).
        If the file already exists, an exception is thrown.
        The extension .csv is added.

        .PARAMETER Directory
        The directory to save the file

        .PARAMETER Name
        The name of the file without extension

        .PARAMETER Force
         If the Force flag is set, existing files will be overwritten. Otherwise an exception
         is thrown if there is already an existing file with the same name.

    #>
    function Test-ItcsPath
    {
        param
        (
            [Parameter(Mandatory = $true)] [string] $Directory,
            [Parameter(Mandatory = $true)] [string] $Name,
            [Parameter()] [switch] $Force
        )

        process
        {
            $path = Join-Path $Directory "$($Name).csv"
            if (-not($Force) -and (Test-Path $path))
            {
                throw "The path '$($path)' already exists. Please specify the -Force flag if you intend to override the existing file, or manually delete it"
            }

            Write-Output $path
        }
    }

      <#
        .SYNOPSIS
        This function exports all itcs text mappings considering the defined filter to the given path.

        .PARAMETER Path
        The path including the filename to export mappings.

        .PARAMETER ProxyConfiguration
        The proxy for the used service.

        .PARAMETER Filter
         The filter for the mappings to export from database.

    #>
    function Export-ItcsTextMapping
    {
        param
        (
            [Parameter(Mandatory = $true)] [string] $Path,
            [Parameter(Mandatory = $true)] [Gorba.Center.BackgroundSystem.PowerShell.Proxy.ProxyConfiguration] $ProxyConfiguration,
            [Parameter(Mandatory = $true)] [Gorba.Center.Common.ServiceModel.Itcs.ItcsTextMappingFilter] $Filter,
            [Parameter(Mandatory = $true)] [switch] $OnlyDefaults
        )

        begin
        {
            <#
                .SYNOPSIS
                Given an ItcsTextMapping, this function selects an object that can be written to a CSV entry

                .DESCRIPTION
                Given an ItcsTextMapping, this function selects an object that can be written to a CSV entry.
                This function expands/selects the ProductId (if present) and the name of the provider.
            #>
            function Select-ItcsTextMappingRow
            {
                param
                (
                    [Parameter(Mandatory = $true, ValueFromPipeline = $true)] [Gorba.Center.Common.ServiceModel.DTO.Itcs.ItcsTextMapping] $InputObject
                )

                process
                {
                    $output = New-Object PSObject
                    $output | Add-Member -MemberType NoteProperty -Name "Id" -Value $InputObject.Id
                    $output | Add-Member -MemberType NoteProperty -Name "ProviderName" -Value $InputObject.ItcsProvider.Name
                    
                    if ($InputObject.ProductType)
                    {
                        $productTypeId = $InputObject.ProductType.Id
                    }
                    else
                    {
                        $productTypeId = $null
                    }

                    # TypeName is required to always have the same type on the property (we could have $null...)
                    $output | Add-Member -MemberType NoteProperty -Name "ProductTypeId" -Value $productTypeId -TypeName "System.String"
                    
                    $output | Add-Member -MemberType NoteProperty -Name "Type" -Value $InputObject.Type
                    $output | Add-Member -MemberType NoteProperty -Name "SourceText" -Value $InputObject.SourceText
                    $output | Add-Member -MemberType NoteProperty -Name "MappedText" -Value $InputObject.MappedText
                    $output | Add-Member -MemberType NoteProperty -Name "TtsText" -Value $InputObject.TtsText

                    if ($InputObject.LastUsed)
                    {
                        $lastUsed = $InputObject.LastUsed.ToString()
                    }
                    else
                    {
                        $lastUsed = $null
                    }

                    # TypeName required...see above (productTypeId)
                    $output | Add-Member -MemberType NoteProperty -Name "LastUsed" -Value $lastUsed -TypeName "System.String"

                    Write-Output $output
                }
            }
        }

        process
        {
            if (Test-Path $Path)
            {
                rm $Path
            }

            Write-Host "Getting the text mappings for the path '$($Path)'. This could require some time..."
            $mappings = Get-ItcsTextMapping -ProxyConfiguration $ProxyConfiguration -Filter $Filter -OnlyDefaults:$OnlyDefaults
            $mappings | Select-ItcsTextMappingRow | Export-Csv -Path $Path -Force -Append -NoTypeInformation -encoding "unicode"
        }
    }

    Import-Module CenterCmdlets -ErrorAction Stop
    $defaults = Test-ItcsPath $Directory "Defaults" -Force: $Force
   
    $maxInt = [int32]::MaxValue
	$quota = New-Object Gorba.Center.BackgroundSystem.PowerShell.Proxy.BindingQuotaSettings
	$quota.MaxBufferPoolSize = $maxInt
	$quota.MaxBufferSize = $maxInt
	$quota.MaxReceivedMessageSize = $maxInt
    $max = [System.Int32]::MaxValue
    $ItcsTextMappingServiceUri = $ServiceBaseAddress + "ItcsTextMappingService.svc/Certificate"
    $itcsTextMappingService = New-CertificateAuthenticatedClientProxyConfiguration -RemoteAddress $ItcsTextMappingServiceUri -DnsName "BackgroundSystem" -CertificateName "CenterOnline" -BindingQuotaSettings $quota
    
    $UnitServiceUri = $ServiceBaseAddress + "UnitService.svc/Certificate"
    $unitService = New-CertificateAuthenticatedClientProxyConfiguration -RemoteAddress $UnitServiceUri -DnsName "BackgroundSystem" -CertificateName "CenterOnline" -BindingQuotaSettings $quota
}

process
{
    $filter = New-ItcsTextMappingFilter
    Export-ItcsTextMapping -Path $defaults -Filter $filter -ProxyConfiguration $itcsTextMappingService -OnlyDefaults:$true
    $productTypes = Get-ProductType -ProxyConfiguration $unitService
    foreach($productType in $productTypes)
    {
        $filter = New-ItcsTextMappingFilter
        $filter.ExcludeDefaults = $true
        $filter.ProductTypeId = $productType.Id
        $filename = Test-ItcsPath $Directory $productType.Name -Force: $Force
        Export-ItcsTextMapping -Path $filename -Filter $filter -ProxyConfiguration $itcsTextMappingService -OnlyDefaults:$false
    }
}

end
{
}