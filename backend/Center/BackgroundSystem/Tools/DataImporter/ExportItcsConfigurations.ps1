<#
    .SYNOPSIS
    This script generates single configuration files out of old configuration files for iqubes and VDV software.

    .DESCRIPTION
    A working directory with the following structure must be provided:
    - ItcsProviders
      - {ProviderName}
        - IqubeConfig.xml
        - VDV453ClientService.exe.config

    where {ProviderName} is the name of the provider as used in the excel file for the import. If one file is missing,
    a warning message will be written and the whole directory ignored.
    Resulting files will be written in the root (working directory).
    
    .PARAMETER ExcelFileName
    Path of the workbook used to add missing units. It can be absolute or relative to the script directory.
    
    .PARAMETER WorkingDirectory
    WorkingDirectory is the base directory where the ItcsProviders folder is located and where the generated xml file
    will be saved. The WorkingDirectory parameter can be absolute or relative to the script position.
    The default value is the script path.

    .PARAMETER BuildConfiguration
    Not used !

    .PARAMETER OverwriteFiles
    If a generated xml file already exists and OverwriteFiles equals false,
    then the script asks for a confirmation before it overwrites the file.
    If OverwriteFiles equals true, the existing file will be overwritten without confirmation.
    In case the user doesn't confirm, the file {ProviderName}.xml will not be overwritten. 

    .PARAMETER ShowWarnings
    If ShowWarnings equals true, the feedback is more verbose.

    .PARAMETER UpdateUnitsA_7
    If UpdateUnitsA_7 equals true, all units with a network address like A:7* will be assigned to tenant 'Gorba (Brügg)'
#>
param
(
    [Parameter(Mandatory = $true)]
    [string]
    $ExcelFileName,

    [Parameter()]
    [string]
    $WorkingDirectory,

    [Parameter()]
    [string]
    $BuildConfiguration = "Debug",

    [Parameter()]
    [switch]
    $OverwriteFiles,

    [Parameter()]
    [switch]
    $ShowWarnings,

    [Parameter()]
    [switch]
    $UpdateUnitsA_7
)

begin
{
    function Get-ScriptDirectory
    {
        <#
            .SYNOPSIS
            Gets the path where the script is executed
        #>
        $Invocation = (Get-Variable MyInvocation -Scope 1).Value
        Split-Path $Invocation.MyCommand.Path
    }
	
	$scriptDirectory = Get-ScriptDirectory
    
    # Test if needed dlls exist     
    $openXml = Join-Path $scriptDirectory "DocumentFormat.OpenXml.dll"
    $closedXml = Join-Path $scriptDirectory "ClosedXml.dll"

    if (-not(Test-Path $openXml))
    {
        throw "The DocumentFormat.OpenXml.dll for excel file handling is missing in the script directory."
    }

    if (-not(Test-Path $closedXml))
    {
        throw "The ClosedXml.dll is missing for excel file handling in the script directory."
    }

    # If dll exist, load them
    [System.Reflection.Assembly]::LoadFrom($openXml)       
    [System.Reflection.Assembly]::LoadFrom($closedXml)

    # Load CenterCmdLets
  	Remove-Module CenterCmdlets -ErrorAction SilentlyContinue

	Import-Module CenterCmdlets
	$centerCmdlets = Get-Module CenterCmdlets
	if(!$centerCmdlets)
	{
		throw "Can't find the CenterCmdlets module. Please ensure that it is available to the PS host."
		exit
	}

    Import-Module -Name "$scriptDirectory\ImportDataUtils.psm1"

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
            throw "The resulting directory doesn't exist and can't be used. ($($WorkingDirectory))"
        }
    }

    $ExcelPath = Resolve-RelativePath -FilePath $ExcelFileName -BaseDirectory $WorkingDirectory
    if (-not(Test-Path $ExcelPath))
    {
        throw "The required excel file $($ExcelPath) could not be found."
    }

    Write-Host "Using Excel file $($ExcelPath)"

    $itcsProvidersDirectory = Join-Path $WorkingDirectory "ItcsProviders"
    if(-not(Test-Path $itcsProvidersDirectory))
    {
        throw "The WorkingDirectory doesn't contain the required ItcsProviders folder"
    }
    
    $assembliesPath = Join-Path $WorkingDirectory "RequiredAssemblies"
    Write-Host "Working assembly path: $($assembliesPath)"
    $assemblyNames = "System.Configuration", "System.Xml", "System.Xml.Linq", "WindowsBase"
    $assemblyNames | % { Add-Type -AssemblyName $_ }

    [System.Xml.Linq.XNamespace] $ix = "http://schemas.gorba.com/iqube"
    $units = @{}
    $stopPoints = @{}
    $filters = @{}    
}

process
{
    function Read-ItcsId
    {
        <#
            .SYNOPSIS 
            Read the operator id (=ItcsId) from the given $configFile parameter.

            .DESCRIPTION
            Read the ItcsId in VDV453ClientService.exe.configuration file for the given itcs client.

            .PARAMETER ConfigFile
            The configuration file name that contains configuation to connect to the Vdv 453 server.

            .OUTPUTS
            System.int. 
            Returns the read Itcs identifier. If the information is not retreived, returns 0.           
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [string]
            $ConfigFile
        )

        process
        {
            Write-Host "Read Itcs id from $($ConfigFile) reading the operator key:"
        
            [System.Xml.XmlDocument] $xd = new-object System.Xml.XmlDocument
            $xd.load($ConfigFile)
            $nodelist = $xd.selectnodes("/configuration/appSettings/add[@key='Operator']")
            foreach ($Node in $nodelist) {
                $id = $Node.getAttribute("value")
                write-host "Operator (ItcsId) = $id"
                return $id
            }

            return 0
        }
    }
        
    function Get-ItcsDirectory
    {
        <#
            .SYNOPSIS 
            Initialize the $ItcsProvider structure with the name of each required file.

            .DESCRIPTION
            This function tests if IqubeConfig.xml (optional) and VDV453ClientService.exe.config(mandatory)
            exist in the WorkingDirectory;
            If not, the IsValid flag is set to false. In that case, the export for the specifed itcs client is skipped.

            .PARAMETER Directory
            The current Itcs directory path. 

            .OUTPUTS
            itcsProvider stucture. 
            Returns the ItcsProvider structure set with read data from the xml and config files.           
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            $Directory
        )

        process
        {
            $iqubeConfig = Join-Path $Directory "IqubeConfig.xml"
            $vdv = Join-Path $Directory "VDV453ClientService.exe.config"

            if (-not(Test-Path $iqubeConfig))
            {
                $iqubeConfig = "";
            }

            if (-not(Test-Path $vdv))
            {
                $vdv = "";
                $isValid = $false
            }  
            else
            {
                $isValid = $true
            }      
        
            $itcsId = 0
            if ($isValid -eq $True)
            {
                $itcsId = Read-ItcsId $vdv
                if ($itcsId -eq 0)
                {
                    $isValid = $false
                    write-warning "The Itcs id is not found in the configuration file 'VDV453ClientService.exe.config'. This itcs provider will be ignored."
                } 
            }
            else
            {
                write-warning "The Itcs directory $($Directory) is not valid. This itcs provider will be ignored."
            }
        
            $itcsProvider = @{
            "IsValid" = $isValid;
            "ItcsId" = $itcsId;
            "Name" = $Directory.Name;
            "Path" = $Directory.FullName;
            "IqubeConfig" = $iqubeConfig;
            "Vdv" = $vdv;
            "Protocol" = "VDV453";
            "PropertiesFile" = "$($Directory.Name).xml";
            }

            return $itcsProvider
        }
    }

    function Create-VdvConfiguration
    {
        <#
            .SYNOPSIS
            Create a VDV configuration object and assigns the values.
            
            .PARAMETER Name
            The name of the configuration.

            .PARAMETER Configuration
            The "old" exe configuration file to be parsed.

            .PARAMETER DefaultSubscription
            Default values for a subscription.

            .OUTPUTS
            Returns the VdvConfiguration object.
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [string]
            $Name,

            [Parameter(Mandatory = $true)]
            [System.Configuration.Configuration]
            $Configuration,

            [Parameter(Mandatory = $true)]
            [Gorba.Center.ItcsClient.ServiceModel.VdvSubscriptionConfiguration]
            $DefaultSubscription
        )

        process
        {
            $vdvConfiguration = New-VdvConfiguration
            $vdvConfiguration.Vdv.UseRealTimeDataOnly = $false
            $vdvConfiguration.Vdv.HttpListenerHost = $Configuration.AppSettings.Settings["HTTPListenerHost"].Value
            $vdvConfiguration.Vdv.HttpListenerPort = $Configuration.AppSettings.Settings["HTTPListenerPort"].Value
            $vdvConfiguration.Vdv.HttpServerHost = $Configuration.AppSettings.Settings["HTTPServerHost"].Value
            $vdvConfiguration.Vdv.HttpServerPort = $Configuration.AppSettings.Settings["HTTPServerPort"].Value
            $vdvConfiguration.Vdv.HttpWebProxyHost = ""
        
            if ($configuration.AppSettings.Settings["HttpWebProxyHostName"])
            {
                $vdvConfiguration.Vdv.HttpWebProxyHost = `                    $Configuration.AppSettings.Settings["HttpWebProxyHostName"].Value
            }
        
            if ($configuration.AppSettings.Settings["HttpWebProxyPort"])
            {
                $vdvConfiguration.Vdv.HttpWebProxyPort = $Configuration.AppSettings.Settings["HttpWebProxyPort"].Value
            }
        
            $vdvConfiguration.Vdv.HttpClientIdentification = `                        $Configuration.AppSettings.Settings["HTTPClientIdentification"].Value
            $vdvConfiguration.Vdv.HttpServerIdentification = `                        $Configuration.AppSettings.Settings["HTTPServerIdentification"].Value
            $vdvConfiguration.Vdv.HttpResponseTimeOut = `                        [TimeSpan]::FromMilliseconds($Configuration.AppSettings.Settings["HTTPResponseTimeOut"].Value)
            $vdvConfiguration.Vdv.XmlClientRequestSenderId = `                        $Configuration.AppSettings.Settings["XMLClientRequestSenderId"].Value
            $vdvConfiguration.Vdv.XmlServerRequestSenderId = `                        $Configuration.AppSettings.Settings["XMLServerRequestSenderId"].Value
        
            if ($Configuration.AppSettings.Settings["XmlNamespaceRequest"])
            {
                $vdvConfiguration.Vdv.XmlNamespaceRequest = `                    $Configuration.AppSettings.Settings["XmlNamespaceRequest"].Value
            }
        
            if ($Configuration.AppSettings.Settings["XmlNameSpaceResponse"])
            {
                $vdvConfiguration.Vdv.XmlNameSpaceResponse = `                    $Configuration.AppSettings.Settings["XmlNameSpaceResponse"].Value
            }

            $vdvConfiguration.Vdv.OmitXmlDeclaration = `                [bool]::Parse($Configuration.AppSettings.Settings["OmitXmlDeclaration"].Value)
            $vdvConfiguration.Vdv.EvaluateDataReadyInStatusResponse = `                [bool]::Parse($Configuration.AppSettings.Settings["EvalDataReadyInStatusRsp"].Value)
            $vdvConfiguration.Vdv.StatusRequestInterval = [TimeSpan]::Zero
            $vdvConfiguration.Vdv.SubscriptionRetryInterval = `                [TimeSpan]::FromSeconds($Configuration.AppSettings.Settings["SubscriptionRetryIntervalInSec"].Value)
            $vdvConfiguration.Vdv.KeepAliveOn = [bool]::Parse($configuration.AppSettings.Settings["KeepAliveOn"].Value)
            $vdvConfiguration.Vdv.KeepAliveInterval = `                [TimeSpan]::FromSeconds($Configuration.AppSettings.Settings["KeepAliveIntervalInSec"].Value)
            $vdvConfiguration.Vdv.ServerUnavailableErrorThreshold = `                [TimeSpan]::FromSeconds($Configuration.AppSettings.`                    Settings["VDVServerUnavailableErrThresholdInSec"].Value)
            $vdvConfiguration.Vdv.ServerUnavailableSpecialInfoText = `                $configuration.AppSettings.Settings["VDVServerUnavailableSpecialInfoText"].Value
            $vdvConfiguration.Vdv.ServerUnavailableModeConfiguration = `                $configuration.AppSettings.Settings["VDVServerUnavailableModeConfig"].Value
            $vdvConfiguration.Vdv.CheckDataReadyInterval = `                [TimeSpan]::FromSeconds($Configuration.AppSettings.Settings["CheckDataReadyIntervalInSec"].Value)
            $vdvConfiguration.Vdv.TechnicalServiceRetryInterval = [TimeSpan]::FromSeconds(300)
        
            $vdvConfiguration.Vdv.EnableAlternatingDirectionText = $false
            if ($Configuration.AppSettings.Settings["EnableAlternatingDirectionText"])
            {
                $vdvConfiguration.Vdv.EnableAlternatingDirectionText = `                    [bool]::Parse($Configuration.AppSettings.Settings["EnableAlternatingDirectionText"].Value)
            }

            $vdvConfiguration.Vdv.DefaultSubscriptionConfiguration = $defaultSubscriptionConfiguration

            $vdvConfiguration.Name = $Name
            $startTime = $Configuration.AppSettings.Settings["SubscriptionStartTime"]
            if($startTime -and ![string]::IsNullOrEmpty($startTime.Value))
            {
                $vdvConfiguration.OperationDayStartUtc = `                    [System.DateTime]::ParseExact($startTime.Value, "HH:mm", `                        [System.Globalization.CultureInfo]::CurrentCulture)
                $vdvConfiguration.OperationDayStartUtc = `                    [System.DateTime]::SpecifyKind($vdvConfiguration.OperationDayStartUtc, [System.DateTimeKind]::Utc)
            }
            else
            {
                $vdvConfiguration.OperationDayStartUtc = New-Object System.DateTime(2012, 1, 1, 2, 0, 0, 0, "Utc")
            }

		    $validUntilTimeString = $Configuration.AppSettings.Settings["SubscriptionValidUntilTime"]
		    if($validUntilTimeString -and ![string]::IsNullOrEmpty($validUntilTimeString.Value))
		    {
				    $validUntilTime = `                        [System.DateTime]::ParseExact($validUntilTimeString.Value, "HH:mm", `                        [System.Globalization.CultureInfo]::CurrentCulture)
				    $validUntilTime = [System.DateTime]::SpecifyKind($validUntilTime, [System.DateTimeKind]::Utc)
				    $difference = $vdvConfiguration.OperationDayStartUtc - $validUntilTime
				    if($difference -lt [System.TimeSpan]::Zero)
				    {
					    $dayDuration = $validUntilTime.Subtract($vdvConfiguration.OperationDayStartUtc)
					    $vdvConfiguration.OperationDayDuration = $dayDuration
				    }
				    else
				    {
					    $dayDuration = $validUntilTime.AddDays(1).Subtract($vdvConfiguration.OperationDayStartUtc)
					    $vdvConfiguration.OperationDayDuration = $dayDuration
				    }
		    }
		    else
		    {	
			    $vdvConfiguration.OperationDayDuration = New-Object System.TimeSpan(23, 59, 0)
		    }
		
            $vdvConfiguration.TimeZoneId = "W. Europe Standard Time"
            return $vdvConfiguration
        }
    }
 
    function Export-ItcsDirectory
    {
        <#
            .SYNOPSIS 
            Create the xml file with the entire itcs configuration.

            .DESCRIPTION
            The xml generated file will be used by the ImportData.ps1 script to fill the center online database.

            .PARAMETER DirectoryInfo
            The current Itcs directory information: "IsValid", "ItcsId", "Name", "Path", "IqubeConfig", 
            "Vdv" = $vdv, "Protocol", "PropertiesFile"

            .OUTPUTS
            Returns the given input parameter to be used as entry in the pipeline.           
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $DirectoryInfo
        )

        process
        {
            $outputPath = Join-Path $DirectoryInfo.Path "../../$($DirectoryInfo.Name).xml"
            if(Test-Path $outputPath)
            {
                $confirm = !$OverwriteFiles
                rm $outputPath -Confirm: $confirm
                if (Test-Path $outputPath)
                {
                    Write-Host "You decided to keep the old file. The ITCS provider $($DirectoryInfo.Name)`                        will not be exported."
                    return
                }
            }        

            $fileMap = New-Object System.Configuration.ExeConfigurationFileMap
            $fileMap.ExeConfigFilename = $DirectoryInfo.vdv
            $configuration = [System.Configuration.ConfigurationManager]::OpenMappedExeConfiguration($fileMap, "None")

            $realtimeHysteresis = [TimeSpan]::FromSeconds(30)
		    $maxTrips = 5
            $preview = [TimeSpan]::FromMinutes(30)
            $maxTextLength = 50
		    $disruptionHysteresis = [TimeSpan]::FromSeconds(30)
		    $handleRealtimeHysteresisLocally = $false;
		    $defaultSubscriptionConfiguration = New-DefaultSubscriptionConfiguration $realtimeHysteresis `                $disruptionHysteresis $handleRealtimeHysteresisLocally $preview $maxTrips $maxTextLength
            $vdvConfiguration = Create-VdvConfiguration -Name $DirectoryInfo.Name -Configuration $configuration `                -DefaultSubscription $defaultSubscriptionConfiguration
	      
            try
            {
                $serializer = New-Object System.Xml.Serialization.XmlSerializer(`                                    [Gorba.Center.ItcsClient.ServiceModel.VdvConfiguration])
                $stream = New-Object System.IO.StreamWriter($outputPath)
                $serializer.Serialize($stream, $vdvConfiguration)
            }
            catch
            {
                Write-Host "Error: $($_.Exception)"
            }
 
            $stream.Close()

            Write-Host "Exported the provider $($DirectoryInfo.Name)"        

            return $DirectoryInfo
        }
    }

    function Test-Qube
    {
        <#
            .SYNOPSIS 
            Reads the 'Units' worksheet from the given workbook and verifies if the given unit name exists.

            .DESCRIPTION
            Parses the second column of the 'Units' worksheet. If the specified name is found in a row,
            returns true, otherwise returns false.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER Name. 
            The name of the unit to check.

            .OUTPUTS
            boolean. 
            Returns true if the $name is found in the workbook, otherwize false.           
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,

            [Parameter(Mandatory = $true)]
            [string]
            $Name
        )

        process
        {
            if($units.ContainsKey($Name))
            {
                return $true
            }

            $ws = $Workbook.Worksheet("Units")
            $row = $ws.FirstRowUsed()
            while(!$row.Cell(2).IsEmpty())
            {
                if($row.Cell(2).Value -eq $Name)
                {
                    return $true
                }

                $row = $row.RowBelow()
            }

            return $false
        }
    }

    function Test-SP
    {
        <#
            .SYNOPSIS 
            Check if the specified stop point name ($name) is contained into the the global list of stop points
            ($stopPoints) or into the specified workbook from the worksheet named 'StopPoints'.

            .DESCRIPTION
            If the list of stop point $stopPoints contains the specified stop point name, then returns $True,
            otherwise returns $False.
            
            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER Name
            The name of the stop point to check.

            .OUTPUTS
            boolean. 
            Returns true if the $name is found into the workbook or if it was already added into the list of
            stop points, otherwize false.      
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,
            
            [Parameter(Mandatory = $true)]
            [string]
            $Name
        )

        process
        {
            if($stopPoints.ContainsKey($Name))
            {
                return $true
            }

            $ws = $Workbook.Worksheet("StopPoints")
            $row = $ws.FirstRowUsed()
            while(!$row.Cell(2).IsEmpty())
            {
                if($row.Cell(2).Value -eq $Name)
                {
                    return $true
                }

                $row = $row.RowBelow()
            }

            return $false
        }
    }

    function Test-DisplayArea
    {
        <#
            .SYNOPSIS 
            Check if the specified display area ($displayArea) is found the specified workbook from
            the worksheet named 'DisplayAreas'.

            .DESCRIPTION
            If the worksheet 'DisplayArea' of the specified workbook contains the specified display area then
            returns $True, otherwise returns $False.
            The display area is checked regarding the Provider name and the display area name.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER DisplayArea
            The hash table containing the values of a display area to check.

            .OUTPUTS
            boolean. 
            Returns true if the display area is found into the workbook, otherwize false.      
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,
            
            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $DisplayArea
        )

        process
        {
            $ws = $workbook.Worksheet("DisplayAreas")
            $row = $ws.FirstRowUsed()
            while(!$row.Cell(2).IsEmpty())
            {
                if($row.Cell(2).Value -eq $displayArea.Provider -and $row.Cell(4).Value -eq $displayArea.Name)
                {
                    return $true
                }

                $row = $row.RowBelow()
            }

            return $false
        }
    }


    function Get-FilterKey
    {
        <#
            .SYNOPSIS 
            Returns a string containing the properties of the specified $filter object.

            .PARAMETER Filter
            The filter object containing the data that will used to build the formatted string.

            .OUTPUTS
            String. 
            Returns a formatted string representing the specifed $filter object. The output format is :
            [StopPoint].[Provider].[DisplayArea].[LineReferenceName].[Line].[DirectionReferenceName].[$filter.Direction]                
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $Filter
        )

        process
        {
            $f = "{0}.{1}.{2}.{3}.{4}.{5}.{6}"
            return [string]::Format($f, $Filter.StopPoint, $Filter.Provider, $Filter.DisplayArea, `                $Filter.LineReferenceName, $Filter.Line, $Filter.DirectionReferenceName, $Filter.Direction)
        }
    }

    function Test-Filter
    {
        <#
            .SYNOPSIS 
            Check if the specified itcs filter ($filter) is contained into the the global list of
            itcs filters ($filters) or into the specified workbook from the worksheet named 'ItcsFilters'.

            .DESCRIPTION
            If the list of itcs filters or the workbook contains the specified filter, then returns $True,
            otherwise returns $False.
            
            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER Filter
            The object containing the itcs filter properties to check.

            .OUTPUTS
            boolean. 
            Returns true if the $filter is found into the workbook or if it was already added into the list itcs filters, otherwize false.      
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,
            
            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $Filter
        )
        
        process
        {
            $key = Get-FilterKey $Filter
            if($Filters.ContainsKey($key))
            {
                return $true
            }

            $ws = $Workbook.Worksheet("ItcsFilters")
            $row = $ws.FirstRowUsed()
            while(!$row.Cell(2).IsEmpty())
            {
                if(($row.Cell(2).Value -eq $Filter.StopPoint) `                    -and ($row.Cell(3).Value -eq $Filter.Provider) `                    -and ($row.Cell(4).Value -eq $Filter.DisplayArea) `                    -and ($row.Cell(5).Value -eq $Filter.LineReferenceName) `                    -and ($row.Cell(6).Value -eq $Filter.Line) `                    -and ($row.Cell(7).Value -eq $Filter.DirectionReferenceName) `                    -and ($row.Cell(8).Value -eq $Filter.Direction))
                {
                    return $true
                }

                $row = $row.RowBelow()
            }

            return $false
        }
    }

    function Test-UnitStopPointAssociation
    {
        <#
            .SYNOPSIS 
            Check if the association between the specified unit name and stop point name exists into the workbook from the worksheet 
            named 'UnitsStopPointsAssociations'.

            .DESCRIPTION
            If the worksheet 'UnitsStopPointsAssociations' of the specified workbook contains the association between the specified unit 
            and the stop point returns $True, otherwise returns $False.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER UnitName
            The unit name to check.

            .PARAMETER StopPointName
            The stop point name to check.

            .OUTPUTS
            boolean. 
            Returns true if the display area is found into the workbook, otherwize false.      
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,

            [Parameter(Mandatory = $true)]
            [string]
            $UnitName, 
            
            [Parameter(Mandatory = $true)]
            [string]
            $stopPointName
        )

        process
        {
            $ws = $Workbook.Worksheet("UnitsStopPointsAssociations")
            $row = $ws.FirstRowUsed()
            while(!$row.Cell(2).IsEmpty())
            {
                if(($row.Cell(2).Value -eq $StopPointName) -and ($row.Cell(3).Value -eq $UnitName))
                {
                    return $true
                }

                $row = $row.RowBelow()
            }

            return $false
        }
    }

    function Update-ItcsProviderReferenceId
    {
        <#
            .SYNOPSIS 
            Update the specified workbook with the data contained into the specified
            $itcsProviderSource structure if the itcs provider name is found.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER ItcsProviderSource
            Structure contining the following properties:
                "IsValid", "ItcsId", "Name", "Path", "IqubeConfig", "Vdv", "Protocol", "PropertiesFile";
            See Get-ItcsDirectory for more details about itcsProvider structure.

            .OUTPUTS
            boolean. 
            Returns $True if the itcs provider name is found into the worksheet ItcsProviders, otherwize $False.      
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,
            
            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $itcsProviderSource
        )

        process
        {
            $ws = $Workbook.Worksheet("ItcsProviders")
            $row = $ws.FirstRowUsed()
            while(!$row.Cell(3).IsEmpty())
            {
                if($row.Cell(3).Value -eq $ItcsProviderSource.Name)
                {
                    $row.Cell(6).Value = $ItcsProviderSource.ItcsId 
                    return $true
                }

                $row = $row.RowBelow()
            }

            return $false
        }
    }

    function Add-IqubeConfig
    {
        <#
            .SYNOPSIS 
            Add iqube config into the specified workbook with the data contained into
            the specified $itcsProviderSource structure.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER ItcsProviderSource
            Structure contining the following properties:
                "IsValid", "ItcsId", "Name", "Path", "IqubeConfig", "Vdv", "Protocol", "PropertiesFile";
            See Get-ItcsDirectory for more details about itcsProvider structure.
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,
            
            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $ItcsProviderSource
        )

        process
        {
            $reader = [System.Xml.XmlReader]::Create($ItcsProviderSource.IqubeConfig)
            $root = [System.Xml.Linq.XElement]::Load($reader)
            $reader.Close()
            $worksheet = $Workbook
            $namespaceManager = New-Object System.Xml.XmlNamespaceManager($reader.NameTable)
            $namespaceManager.AddNamespace("i", "http://schemas.gorba.com/iqube")
            $iqubeElements = `                [System.Xml.XPath.Extensions]::XPathSelectElements($root, "/Iqube[@i:name]", $namespaceManager)
           # [System.Xml.Linq.XName] $xn = $ix + "name"
            foreach($iqubeElement in $iqubeElements)
            {
                $n = $iqubeElement.Attribute($ix + "name")
                if($n)
                {
                    $qube = @{
                        "Name" = $n.Value;
                        "NetworkAddress" = $iqubeElement.Element("IqubeAddress").Value;
                        "Tenant" = $ItcsProviderSource.Name;
                        "Type" = $iqubeElement.Element("IqubeType").Value
                    }

                    #[System.Xml.Linq.XName] $xs = $ix + "stopPoint" 
                    $s = $iqubeElement.Attribute($ix + "stopPoint")
                    if($s)
                    {
                        $stopPointName = $s.Value
                    }
                    else
                    {
                        $stopPointName = $n.Value
                    }

                    $l = $iqubeElement.Attribute($ix + "layout")
                    if($l)
                    {
                        $qube.Layout = $l.Value
                    }
                
                    $isTenant = $iqubeElement.Attribute($ix + "isTenant")
                    if(!$isTenant -or !$isTenant.Value -or (Test-Qube $worksheet $qube.Name))
                    {
                        if($ShowWarnings)
                        {
                            Write-Warning "Duplicate iqube with name '$($qube.Name)' or this not the right tenant"
                        }
                    }
                    else
                    {
                        Add-Qube $worksheet $qube
                    }
                
                    $stopPoint = @{
                        "Name" = $stopPointName;
                        "Description" = $stopPointName
                    }

                    if(Test-SP $worksheet $stopPointName)
                    {
                        if($ShowWarnings)
                        {
                            Write-Warning "Duplicate Stop Point with name '$($stopPointName)'"
                        }
                    }
                    else
                    {
                        Add-SP $worksheet $stopPoint
                    }

                    if(Test-UnitStopPointAssociation $worksheet $qube.Name $stopPoint.Name)
                    {
                        if ($ShowWarnings)
                        {
                           Write-Warning "Duplicate unit/stoppoint association with unit `                            '$($qube.Name)' and stoppoint '$($stopPoint.Name)'"
                        }
                    }
                    else
                    {
                        Add-UnitStopPointAssociation $worksheet $qube.Name $stopPoint.Name
                    }
                
                    $filter = @{
                        "StopPoint" = $stopPoint.Name;
                        "Provider" = $ItcsProviderSource.Name;
                        }

                    $area = $iqubeElement.Element("RblStation")
                    if($area)
                    {
                        $filter.DisplayArea = $area.Value
                    }

                    $lr = $iqubeElement.Element("RblLine")
                    if($lr)
                    {
                        $filter.LineReferenceName = $lr.Value
                    }

                    $l = $iqubeElement.Element("IqubeLine")
                    if($l)
                    {
                        $filter.Line = $l.Value
                    }

                    $dr = $iqubeElement.Element("RblDirection")
                    if($dr)
                    {
                        $filter.DirectionReferenceName = $dr.Value
                    }

                    $d = $iqubeElement.Element("IqubeDirection")
                    if($d -and $d.Value -ne "0")
                    {
                        $filter.Direction = $d.Value
                    }

                    $displayArea = @{
                        "Provider" = $ItcsProviderSource.Name;
                        "Name" = $filter.DisplayArea;
                        "Description" = $filter.DisplayArea
                        }

                    if(Test-DisplayArea $worksheet $displayArea)
                    {
                        if ($ShowWarnings)
                        {
                            Write-Warning "Found duplicated display area '$($filter.DisplayArea)'"
                        }
                    }
                    else
                    {
                        Add-DisplayArea $worksheet $displayArea
                    }

                    if(Test-Filter $worksheet $filter)
                    {
                    
                        if ($ShowWarnings)
                        {
                            Write-Warning "Duplicate Filter with name '$($qube.Name)', Station `                                '$($filter.DisplayArea)', RblLine '$($filter.LineReferenceName)' `                                and RblDirection '$($filter.DirectionReferenceName)'"
                        }
                    }
                    else
                    {
                        Add-Filter $worksheet $filter
                    }                
                }
                else
                {
                    Write-Warning "Found an iqube without name"
                }
            }
        }
    }

    function Add-Qube
    {
        <#
            .SYNOPSIS 
            Add the specified unit ($qube) to the global unit list and to the workbook into the worksheet 'Units'.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER Qube
            The unit object to add.
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,
            
            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $Qube
        )

        process
        {
            $ws = $Workbook.Worksheet("Units")
            $row = $ws.FirstRowUsed()
            while(!$row.Cell(2).IsEmpty())
            {
                $row = $row.RowBelow()
            }

            $row.Cell(1).Value = 0
            $row.Cell(2).Value = $Qube.Name
            $row.Cell(5).Value = $Qube.NetworkAddress
            $row.Cell(7).Value = "W. Europe Standard Time"
            $row.Cell(8).Value = $Qube.Tenant
            $row.Cell(9).Value = $Qube.Type
            $row.Cell(10).Value = $Qube.Layout
            $units.Add($Qube.Name, $true)
        }
    }

    function Add-SP
    {
        <#
            .SYNOPSIS 
            Add the specified stop point ($stopPoint) to the global StopPoint list and to the workbook into
            the worksheet 'StopPoints'.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER StopPoint
            The stop point object to add.
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,
            
            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $StopPoint
        )

        process
        {
            $stopPoints.Add($StopPoint.Name, $true)
            $ws = $Workbook.Worksheet("StopPoints")
            $row = $ws.FirstRowUsed()
            while(!$row.Cell(2).IsEmpty())
            {
                $row = $row.RowBelow()
            }

            $row.Cell(1).Value = 0
            $row.Cell(2).Value = $StopPoint.Name
            $row.Cell(3).Value = $StopPoint.Description
        }
    }

    function Add-DisplayArea
    {
        <#
            .SYNOPSIS 
            Add the specified display area ($displayArea) to the workbook into the worksheet 'DisplayAreas'.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER DisplayArea
            The display area object to add.
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,
            
            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $DisplayArea
        )

        process
        {
            $ws = $Workbook.Worksheet("DisplayAreas")
            $row = $ws.FirstRowUsed()
            while(!$row.Cell(2).IsEmpty())
            {
                $row = $row.RowBelow()
            }

            $row.Cell(1).Value = 0
            $row.Cell(2).Value = $DisplayArea.Provider
            $row.Cell(3).Value = $DisplayArea.Description
            $row.Cell(4).Value = $DisplayArea.Name
        }
    }

    function Add-Filter
    {
        <#
            .SYNOPSIS 
            Update the specified workbook adding the specified $filter properties and add the 
            specified $filter object to the global dictionnary $filters.

            .DESCRIPTION
            Add a new row into the worksheet 'ItcsFilters' from the specified workbook. 
            Add the specified $filter object to the global dictionnary $filters. The key is composed 
            with a string formatted with the properties of the itcs filter. 
            See Get-FilterKey for more details.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER Filter
            The itcs filter object to add to the wokbook and to the filters dictionnary 
            (key = filter key, value = itcs filter)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,
            
            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $Filter
        )

        process
        {
            $key = Get-FilterKey $Filter
            $ws = $Workbook.Worksheet("ItcsFilters")
            $row = $ws.FirstRowUsed()
            while(!$row.Cell(2).IsEmpty())
            {
                $row = $row.RowBelow()
            }

            $row.Cell(1).Value = 0
            $row.Cell(2).Value = $Filter.StopPoint
            $row.Cell(3).Value = $Filter.Provider
            $row.Cell(4).Value = $Filter.DisplayArea
            $row.Cell(5).Value = $Filter.LineReferenceName
            $row.Cell(6).Value = $Filter.Line
            $row.Cell(7).Value = $Filter.DirectionReferenceName
            $row.Cell(8).Value = $Filter.Direction

            $filters.Add($key, $true)
        }
    }

    function Add-UnitStopPointAssociation
    {
        <#
            .SYNOPSIS 
            Update the workbook adding a new association between the specified unitName and stop point name.
            named 'UnitsStopPointsAssociations'.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER UnitName
            The unit name of the new association to add.

            .PARAMETER StopPointName
            The stop point name of the new association to add.
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,

            [Parameter(Mandatory = $true)]
            [string]
            $UnitName, 
            
            [Parameter(Mandatory = $true)]
            [string]
            $StopPointName 
        )

        process
        {
            $ws = $Workbook.Worksheet("UnitsStopPointsAssociations")
            $row = $ws.FirstRowUsed()
            while(!$row.Cell(2).IsEmpty())
            {
                $row = $row.RowBelow()
            }

            $row.Cell(1).Value = 0
            $row.Cell(2).Value = $StopPointName
            $row.Cell(3).Value = $UnitName
        }
    }

    function Export-ItcsProvider
    {
        <#
            .SYNOPSIS 
            Main function to export itcs provider data from customer data. 

            .DESCRIPTION
            This function reads the data from the files stored into itcs directories to create at the end the xml configuration files needed 
            to fill the icenter online database. 
            This function also update the workbook with the missing data read from the original files.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER ItcsProviderSource. 
            Structure contining the following properties:
                "IsValid", "ItcsId", "Name", "Path", "IqubeConfig", "Vdv", "Protocol", "PropertiesFile";
            See Get-ItcsDirectory for more details about itcsProvider structure.
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,
            
            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $ItcsProviderSource
        )

        process
        {
            Write-Host "Itcs provider: $($ItcsProviderSource.Name)"                
        
            if ([string]::IsNullOrEmpty($ItcsProviderSource.IqubeConfig) -eq $false)
            {
                Write-Host "Adding iqube config from file: $($ItcsProviderSource.IqubeConfig)."
                Add-IqubeConfig $Workbook $ItcsProviderSource                    
            }
        
            if (-not (Update-ItcsProviderReferenceId $Workbook $ItcsProviderSource))
            {
                if ($ShowWarnings)
                {
                    Write-Warning "ItcsProvider '$($ItcsProviderSource.Name)' is not found in ItcsProviders tab."
                }
            }
        }
    }

    #PROCESS 

    $itcs = ls "$($itcsProvidersDirectory)\*" | ? { $_.PSIsContainer }
    
    $wb = New-Object "ClosedXML.Excel.XLWorkbook"($ExcelPath)
    $itcs | % { Get-ItcsDirectory $_ } | ? { $_.IsValid } | % { Export-ItcsDirectory $_ } | % { Export-ItcsProvider $wb $_ }

    # set tenant Gorba (Brügg) for units A:7.x.x
    if ($UpdateUnitsA_7)
    {
        $ws = $wb.Worksheet("Units")
        $row = $ws.FirstRowUsed()
        while(!$row.Cell(2).IsEmpty())
        {
            if($Row.Cell(5).Value.StartsWith("A:7"))
            {
                $row.Cell(8).Value = "Gorba (Brügg)"
            }
            $row = $row.RowBelow()
        }
    }

    $wb.Save()
    # Avoid out of memory exception
    $wb.Dispose();
    $wb = $null;
}

end
{
}