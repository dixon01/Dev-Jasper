<#
	.SYNOPSIS
	Imports data into a remote BackgroundSystem instance using an excel file as input.
	
	Supported sheet version: 0.2

    .PARAMETER ExcelFileName
    Path of the workbook used to import data into database. It can be absolute or relative to the script directory.
    Default value = "../../Deploy/Source/Test data.xlsx".

    .PARAMETER ServicesBaseAddress
    The script uses a background services to import data into the database. the parameter is 
    the base address to access to these services.
    Default value = "net.tcp://localhost/BackgroundSystem"

    .PARAMETER ExportItcs
    Indicates if before importing data, this script calls first the Export itcs configuration script 
    (ExportItcsConfigurations.ps1) to create the needed configuration files if they not already exist.
    The existing provider configuration files must be available in the following structure within the directory
    where the excel file is located:
    - ItcsProviders
      - {ProviderName}
        - IqubeConfig.xml
        - VDV453ClientService.exe.config
    
    .PARAMETER XsdSchemaPath
    Full path of the xsd file to verify the xml configuration files. If the path doesn't exist, the 
    verification will be skipped. Will be used while adding itcs providers.

    .PARAMETER SkipReset
    Indicates if the script resets the background system or if it skips this step. This step mainly 
    clear the database and restart IIS.

    .PARAMETER Interactive
    If set to $True the script stops at certain points allowing the user to check the logs.

    .PARAMETER UseNetNamedPipeBinding
    If set to $True the NetNamedPipeBinding will be used. It only works locally and if the server is properly configured to accept it.
#>
param
(
    [Parameter()]
	[string] 
    $ExcelFileName = "../../Deploy/Source/Test data.xlsx",
	
    [Parameter()]
    [string] 
    $ServicesBaseAddress = "net.tcp://localhost/BackgroundSystem",
    
    [Parameter()]
    [switch] 
    $ExportItcs,

    [Parameter()]
    [string] 
    $XsdSchemaPath,

    [Parameter()]
	[switch] 
    $SkipReset,
	
	[Parameter()]
	[switch]
	$Interactive,

    [Parameter()]
    [switch]
    $UseNetNamedPipeBinding
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

    function ValidateXmlAgainstXsd()
    {       
        <#
            .SYNOPSIS
            Enables to validate the xml content which could be loaded from the given xmlPathName. 

            .DESCRIPTION
            The validation is done according to the xsd schema loaded from the given xsdPathName.
            Returns $True if the xml matches with xsd schema, otherwize returns $False and displays encountered errors
            during the validation.
        
            .PARAMETER XmlPathName
            The xml file path.

            .PARAMETER XsdPathName
            The xsd file path.

            .NOTE
            Uses ther Validate-Xml pseudo command implemented in the ValidateXml.ps1 script.

            .OUTPUTS
            boolean. 
            Returns $True if no error was encountered by the verification script, otherwize returns $False.    
        #>       
        param
        (
            [Parameter()]
            [String]$xmlPathName, 

            [Parameter()]
            [String]$xsdPathName
        )

        process
        {
            try
		    {   
			    $validatePath = Join-Path $scriptDirectory "ValidateXml.ps1"
			    . $validatepath
                [xml]$xml = Get-Content $xmlPathName
                $errors = Validate-Xml $xml $xsdPathName | Where-Object { $_.Message }
                if (!$errors)
                {
                    return $true
                }
                else
                {   
                    foreach ($element in $errors)
                    { 
                        Write-Host $element
                    }
                }
            }
		    catch [System.Exception]
		    {
			        Write-Error "Error while validating the xlm file $($xmlPathName) on line ~$($i)"
			        Write-Error $_.Exception.ToString()
		    }

            return $false;
        }
    }
    
    [System.Reflection.Assembly]::LoadWithPartialName("System.ServiceModel")

    $ItcsXsdFileName = "VdvConfiguration.xsd"
    $VdvSubscriptionXsdFileName = "VdvSubscriptionConfiguration.xsd"
    
	# -----------------------------------------------------

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

    Add-Type -AssemblyName "System.Xml.Linq"
	
	$scriptDirectory = Get-ScriptDirectory

    # Import the module to validate xml content against the xsd schema
    Import-Module -Name "$scriptDirectory\ValidateXml.ps1"

    Import-Module -Name "$scriptDirectory\ImportDataUtils.psm1"

    $openXml = Join-Path $scriptDirectory "DocumentFormat.OpenXml.dll"
    [System.Reflection.Assembly]::LoadFrom($openXml)

    $closedXml = Join-Path $scriptDirectory "ClosedXml.dll"
    [System.Reflection.Assembly]::LoadFrom($closedXml)

    $ExcelPath = $ExcelFileName

    $AbsoluteExcelPath = Resolve-RelativePath -FilePath $ExcelFileName -BaseDirectory $scriptDirectory
    
    if ([string]::IsNullOrEmpty($AbsoluteExcelPath))
    {
        throw "The path '$($ExcelPath)' couldn't be resolved. The script can't be executed."
		Exit
    }

    Write-Host "Importing data from $($AbsoluteExcelPath)."

    $ExcelDirectory = Split-Path $AbsoluteExcelPath -Parent
	
    # Set the folder path of xsd schemas to validate xml stream for itcs providers and display areas
    if ([string]::IsNullOrEmpty($XsdSchemaPath))
    {
        $XsdSchemaPath = Join-Path $scriptDirectory "../../../ItcsClient/Source/Core/Resources/"		
    }
    
    Write-Host "Xsd schemas location: $($XsdSchemaPath)."
    
    if (-not(Test-Path -path $XsdSchemaPath))
	{
	    $XsdSchemaPath = $null
        Write-Warning "The path of xsd schemas doesn't exist."    
    }
        
	Write-Host "Importing data on the service base address '$($ServicesBaseAddress)'"      
    $bindingQuota = New-Object Gorba.Center.BackgroundSystem.PowerShell.Proxy.BindingQuotaSettings
    $bindingQuota.MaxBufferPoolSize = 2147483647
    $bindingQuota.MaxBufferSize = 2147483647
    $bindingQuota.MaxReceivedMessageSize = 2147483647
    $bindingQuota.ReceiveTimeout = [TimeSpan]::FromMinutes(1)
    $bindingQuota.SendTimeout = [TimeSpan]::FromMinutes(1)
    $bindingQuota.CloseTimeout = [TimeSpan]::FromSeconds(30)
    $bindingQuota.OpenTimeout = [TimeSpan]::FromSeconds(30)
    $bindingQuota.MaxDepth = 512
    $bindingQuota.MaxBytesPerRead = 65536
    $bindingQuota.MaxArrayLength = 2147483647
    $bindingQuota.MaxStringContentLength = 2147483647
    $bindingQuota.MaxNameTableCharCount = 2147483647
    if ($UseNetNamedPipeBinding)
    {
	    $ms = New-NetNamedPipeBindingProxyConfiguration -Path MembershipService.svc
	    $us = New-NetNamedPipeBindingProxyConfiguration -Path UnitService.svc
	    $ds = New-NetNamedPipeBindingProxyConfiguration -Path DomainService.svc
	    $tds = New-NetNamedPipeBindingProxyConfiguration -Path ItcsConfigDataService.svc
     }
     else
     {
	    $ms = New-CertificateAuthenticatedClientProxyConfiguration `
	        -RemoteAddress "$($ServicesBaseAddress)/MembershipService.svc/Certificate" `
	        -DnsName "BackgroundSystem" -CertificateName "CenterOnline" -BindingQuotaSettings $bindingQuota
		$us = New-CertificateAuthenticatedClientProxyConfiguration `
	        -RemoteAddress "$($ServicesBaseAddress)/UnitService.svc/Certificate" `
	        -DnsName "BackgroundSystem" -CertificateName "CenterOnline" -BindingQuotaSettings $bindingQuota
		$ds = New-CertificateAuthenticatedClientProxyConfiguration `
	        -RemoteAddress "$($ServicesBaseAddress)/DomainService.svc/Certificate" `
	        -DnsName "BackgroundSystem" -CertificateName "CenterOnline" -BindingQuotaSettings $bindingQuota
		$tds = New-CertificateAuthenticatedClientProxyConfiguration `
	        -RemoteAddress "$($ServicesBaseAddress)/ItcsConfigDataService.svc/Certificate" `
	        -DnsName "BackgroundSystem" -CertificateName "CenterOnline" -BindingQuotaSettings $bindingQuota
     }   
    $ownerTenant 
}

process
{
    function Add-Tenants
    {
        <#
            .SYNOPSIS
            Imports all tenants of the worksheet Tenants into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .OUTPUTS
            Returns a HashTable of all Tenants (key = name, value = tenant object)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook
        )

        process
        {
            $ws = $Workbook.Worksheet("Tenants")
            $headerRow = $ws.FirstRowUsed()
            $t = $headerRow.RowBelow()
	        $tenants = @{}
		    $tenantUId = 1
	        while(!$t.Cell(2).IsEmpty())
	        {
		        $isDefault = [bool]::Parse($t.Cell(4).Value)
                $name = $t.Cell(2).Value
                $description = $t.Cell(3).Value
		        $tenant = New-Tenant $name $description -IsDefault: $isDefault
			    $tenant.UId = $tenantUId++
		        $tenant = Add-Tenant $ms $tenant
		        $tenants.Add($name, $tenant)
                Write-Verbose "Added tenant '$($name)"
                $t = $t.RowBelow()
			
			    if (-not($global:ownerTenant))
			    {
				    $global:ownerTenant = $tenant
			    }
	        }
            
            return $tenants
        }
    }

    function Add-UserRoles
    {
        <#
            .SYNOPSIS
            Imports all user roles of the worksheet UserRoles into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .OUTPUTS
            Returns a HashTable of all UserRoles (key = UserRole name, value = UserRole object)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook
        )

        process
        {
            $ws = $Workbook.Worksheet("UserRoles")
            $headerRow = $ws.FirstRowUsed()
            $r = $headerRow.RowBelow()
	        $userRoles = @{}
	        while(!$r.Cell(2).IsEmpty())
	        {
                $name = $r.Cell(2).Value
		        $userRole = New-UserRole
		        $userRole.Name = $name
		        $userRole.Description = $r.Cell(3).Value
		        $userRole = Add-UserRole $ms $userRole
			    $userRoles.Add($name, $userRole)
                $r = $r.RowBelow()
	        }

            return $userRoles
        }
    }

    function Add-Permissions
    {
        <#
            .SYNOPSIS
            Imports all permissions of the worksheet Permissions into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .OUTPUTS
            Returns a HashTable of all Permissions (key = Permission name, value = Permission object)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook
        )

        process
        {
            $ws = $Workbook.Worksheet("Permissions")
            $headerRow = $ws.FirstRowUsed()
            $p = $headerRow.RowBelow()
	        $permissions = @{}
	        while(!$p.Cell(2).IsEmpty())
	        {
                $name = $p.Cell(2).Value
		        $permission = New-Permission $name
		        $permission = Add-permission $ds $permission
		        $permissions.Add($name, $permission)
                Write-Verbose "Added Permission $($name)"
                $p = $p.RowBelow()
	        }

            return $permissions
        }
    }

    function Add-DataScopes
    {
        <#
            .SYNOPSIS
            Imports all data scopes of the worksheet "Data scopes" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .OUTPUTS
            Returns a HashTable of all data scopes (key = name, value = DataScope object)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook
        )

        process
        {
            $ws = $Workbook.Worksheet("Data scopes")
            $headerRow = $ws.FirstRowUsed()
            $d = $headerRow.RowBelow()
	        $dataScopes = @{}
	        while(!$d.Cell(2).IsEmpty())
	        {
                $name = $d.Cell(2).Value
		        $dataScope = New-DataScope $name
		        $dataScope = Add-DataScope $ds $dataScope
                Write-Verbose "Added data scope $($name)"
		        $dataScopes.Add($name, $dataScope)
                $d = $d.RowBelow()
	        }

            return $dataScopes
        }
    }

    function Add-Users
    {
        <#
            .SYNOPSIS
            Imports all data scopes of the worksheet "Users" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .OUTPUTS
            Returns a HashTable of all users (key = name, value = User object)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook
        )

        process
        {
            $ws = $wb.Worksheet("Users")
            $headerRow = $ws.FirstRowUsed()
            $u = $headerRow.RowBelow()
	        $users = @{}
	        while(!$u.Cell(2).IsEmpty())
	        {
                $username = $u.Cell(2).Value
		        $user = New-User
			    $user.OwnerTenant = $global:ownerTenant
		        $user.Username = $username
		        $user.Password = Invoke-PasswordEncryption $u.Cell(3).Value
		        $user.Culture = $u.Cell(4).Value
		        $user = Add-User $ms $user
                Write-Verbose "Added user $($username)"
		        $users.Add($username, $user)
                $u = $u.RowBelow()
	        }

            return $users
        }
    }

    function Add-UserTenantAssociations
    {
        <#
            .SYNOPSIS
            Imports all data scopes of the worksheet "Associations tenant user" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER Tenants
            The hash table with all tenants (key = name, value = tenant)

            .PARAMETER Users
            The hash table with all users (key = name, value = user)

            .PARAMETER UserRoles
            The hash table with all user roles (key = name, value = user role)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $Tenants,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $Users,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $UserRoles
        )

        process
        {
            $ws = $Workbook.Worksheet("Associations tenant user")
            $headerRow = $ws.FirstRowUsed()
            $a = $headerRow.RowBelow()
	        #$associationTenantUsers = @{}
	        $i = 0
	        while(!$a.Cell(2).IsEmpty())
	        {
		        try
		        {
                    $username = $a.Cell(2).Value
			        $user = $Users[$username]
                    $tenantName = $a.Cell(3).Value
			        $tenant = $Tenants[$tenantName]
                    $roleName = $a.Cell(4).Value
			        $userRole = $UserRoles[$roleName]
			        if (-not($user))
			        {
				        throw "The user is null for $($username)"
			        }
			
			        Grant-UserRole $ds $tenant.Id $user.Id $userRole.Id
                    Write-Verbose "Granted role '$($roleName)' to user '$($username)' for tenant '$($tenantName)'"
		
			        $i ++
		        }
		        catch [System.Exception]
		        {
			        Write-Error "Error while associating User $($a.User), tenant $($a.Tenant) and role $($a.Role) on line ~$($i)"
			        Write-Error $_.Exception.ToString()
			        exit
		        }

                $a = $a.RowBelow()
	        }
        }
    }

    function Grant-Permissions
    {
         <#
            .SYNOPSIS
            Imports all data scopes of the worksheet "Associations tenant user" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER Permissions
            The hash table with all permissions (key = name, value = permission)

            .PARAMETER DataScopes
            The hash table with all data scopes (key = name, value = data scope)

            .PARAMETER UserRoles
            The hash table with all user roles (key = name, value = user role)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $Permissions,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $DataScopes,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $UserRoles
        )

        process
        {
            $ws = $Workbook.Worksheet("Associations p-ds-ur")
            $headerRow = $ws.FirstRowUsed()
            $a = $headerRow.RowBelow()
	        #$associationPermissionDataScopeUserRoles = @{}
	        while(!$a.Cell(2).IsEmpty())
	        {
                $permissionName = $a.Cell(2).Value
		        $permission = $Permissions[$permissionName]
                $dataScopeName = $a.Cell(3).Value
		        $dataScope = $DataScopes[$dataScopeName]
                $userRoleName = $a.Cell(4).Value
		        $userRole = $UserRoles[$userRoleName]
		        Grant-Permission $ds $userRole.Id $dataScope.Id $permission.Id
                Write-Verbose "Granted permission '$($permissionName)' to role '$($userRoleName)' for `                    data scope '$($dataScopeName)'"
                $a = $a.RowBelow()
	        }
        }
    }

    function Add-UnitTypes
    {
        <#
            .SYNOPSIS
            Imports all unit types of the worksheet "Unit types" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .OUTPUTS
            Returns a HashTable of all unit types (key = name, value = unit type)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook
        )

        process
        {
            $ws = $Workbook.Worksheet("Unit types")
            $headerRow = $ws.FirstRowUsed()
            $t = $headerRow.RowBelow()
            $unitTypes = @{}
	        while(!$t.Cell(2).IsEmpty())
	        {
		        try
		        {
                    $name = $t.Cell(2).Value
                    $description = $t.Cell(3).Value
                    [bool] $isDefault = $t.Cell(4).Value
			        $unitType = New-UnitType $name
                    #$unitType.Description = $description
			        $unitType.IsDefault = [bool]::Parse($isDefault)				
			        $unitType = Add-UnitType $us $unitType
                    $unitTypes.Add($name, $unitType)
                    Write-Verbose "Added unit type '$($name)'"
		        }
		        catch [System.Exception]
		        {
			        Write-Host $_.Exception.ToString()
			        exit
		        }

                $t = $t.RowBelow()
	        }

            return $unitTypes
        }
    }

    function Get-ProductTypeProperties
    {
        <#
            .SYNOPSIS
            Gets all product type properties of the worksheet "iqube Product type properties".

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .OUTPUTS
            Returns a HashTable of all product type properties (key = product type name, value = product type properties)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XlWorkbook]
            $Workbook
        )

        process
        {
            $ws = $Workbook.Worksheet("iqube Product type properties")
            $headerRow = $ws.FirstRowUsed()            $row = $headerRow.RowBelow()
            $productTypeProperties = @{}
            while(!$row.Cell(2).IsEmpty())
            {
                $name = $row.Cell(2).Value
                $line = $row.Cell(3).Value
                $destination = $row.Cell(4).Value
                $lane = $row.Cell(5).Value
                $tts = $row.Cell(6).Value
                $properties = New-ProductTypeProperties
                $properties.Line = $line
                $properties.Destination = $destination
                $properties.Lane = $lane
                $properties.Tts = $tts
                $productTypeProperties.Add($name, $properties)
                Write-Verbose "Added product type properties for product type '$($name)'"
                $row = $row.RowBelow()
            }

            return $productTypeProperties
        }
    }

    function Add-ProductTypes
    {
        <#
            .SYNOPSIS
            Imports all product types of the worksheet "Product types" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER UnitTypes
            The hash table with all imported unit types (key = name, value = unit type)

            .OUTPUTS
            Returns a HashTable of all product types (key = name, value = product type)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $UnitTypes
        )

        process
        {
            $productTypeProperties = Get-ProductTypeProperties $Workbook
            $ws = $Workbook.Worksheet("Product types")
            $headerRow = $ws.FirstRowUsed()
            $t = $headerRow.RowBelow()
	        $productTypes = @{}
	        while(!$t.Cell(2).IsEmpty())
	        {
                $name = $t.Cell(2).Value
                $unitTypeName = $t.Cell(3).Value
                $unitType = $UnitTypes[$unitTypeName]
		        $productType = New-ProductType $name
		        #$productType.Description = $t.Description
		        $productType.UnitType = $unitType
                $properties = $productTypeProperties[$name]
                $productType.Properties = $properties
		        $productType = Add-ProductType $us $productType
                Write-Verbose "Added product type '$($name)' for unit type '$($unitTypeName)'"
		        $productTypes.Add($name, $productType)

                $t = $t.RowBelow()
	        }

            return $productTypes
        }
    }

    function Add-UnitGroupTypes
    {
        <#
            .SYNOPSIS
            Imports all product types of the worksheet "UnitGroup Types" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .OUTPUTS
            Returns a HashTable of all unit group types (key = name, value = unit group type)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook
        )

        process
        {
            $ws = $Workbook.Worksheet("UnitGroup Types")
            $headerRow = $ws.FirstRowUsed()
            $t = $headerRow.RowBelow()
            $unitGroupTypes = @{}
	        while(!$t.Cell(2).IsEmpty())
	        {
		        try
		        {
                    $name = $t.Cell(2).Value
                    $description = $t.Cell(3).Value  
                    $unitGroupType = New-UnitGroupType $name     
                    $unitGroupType.Description = $description			   		
			        $unitGroupType = Add-UnitGroupType $us $unitGroupType 
                    $unitGroupTypes.Add($name, $unitGroupType)                
                    Write-Verbose "Added unit group type '$($name)'"
		        }
		        catch [System.Exception]
		        {
			        Write-Host $_.Exception.ToString()
			        exit
		        }

                $t = $t.RowBelow()
	        }

            return $unitGroupTypes
        }
    }

    function Add-UnitGroups
    {
         <#
            .SYNOPSIS
            Imports all product types of the worksheet "UnitGroups" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER Tenants
            The hash table with all imported tenants (key = name, value = tenant)

            .PARAMETER UnitGroupTypes
            The hash table with all imported unit group types (key = name, value = unit group type)

            .OUTPUTS
            Returns a HashTable of all unit groups (key = name, value = unit group)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $Tenants,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $UnitGroupTypes
        )

        process
        {
            $ws = $Workbook.Worksheet("UnitGroups")
            $headerRow = $ws.FirstRowUsed()
            $t = $headerRow.RowBelow()
            $unitGroups = @{}
	        while(!$t.Cell(2).IsEmpty())
	        {
		        try
		        {
                    $tenantName = $t.Cell(2).Value
			        $tenant = $Tenants[$tenantName]
                    if (-not($tenant))
			        {
				        throw "The tenant is null for $($tenantName)"
			        }
                
                    $unitGroupTypeName = $t.Cell(3).Value
                    $unitGroupType = $UnitGroupTypes[$unitGroupTypeName]
                    if (-not($unitGroupType))
			        {
				        throw "The unit group is null for $($unitGroupTypeName)"
			        }
                
                    $name = $t.Cell(4).Value
                    $systemName = $t.Cell(5).Value
                    $description = $t.Cell(6).Value  
                    $unitGroup = New-UnitGroup $name $systemName $tenant.Id $unitGroupType.Id     
                    $unitGroup.SystemName = $systemName			   		
                    $unitGroup.Description = $description			   		
			        $unitGroup = Add-UnitGroup $us $unitGroup
                    $unitGroups.Add($name, $unitGroup)                
                    Write-Verbose "Added unit group '$($name)/$($systemName)' for tenant '$($tenantName)'"
		        }
		        catch [System.Exception]
		        {
			        Write-Host $_.Exception.ToString()
			        exit
		        }

                $t = $t.RowBelow()
	        }
            
            return $unitGroups
        }
    }

    function Add-Layouts
    {
        <#
            .SYNOPSIS
            Imports all layouts of the worksheet "Layouts" into database.
            It is assumed that only the path relative to the script parameter ExcelFileName is given.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .OUTPUTS
            Returns a HashTable of all layouts (key = name, value = layout)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook
        )

        process
        {
            $ws = $Workbook.Worksheet("Layouts")
            $headerRow = $ws.FirstRowUsed()
            $l = $headerRow.RowBelow()
	        $layouts = @{}

	        while(!$l.Cell(2).IsEmpty())
	        {
		        try
		        {
                    $name = $l.Cell(2).Value
			        $description = $l.Cell(3).Value
                    $definition = $null
                    if(!$l.Cell(4).IsEmpty())
                    {
                        $definitionPath = $l.Cell(4).Value

                        $definitionFullPath = Resolve-RelativePath -FilePath $definitionPath -BaseDirectory $ExcelDirectory
                    
                        if(-not([string]::IsNullOrEmpty($definitionFullPath)))
                        {
                            $definition = [System.Xml.Linq.XElement]::Load($definitionFullPath)
                        }
                        else
                        {
                            Write-Warning "The path for the definition of the layout '$($name)' was defined, but the file was not found"
                        }
                    }

                    $layout = New-Layout $name $definition
                    $createdLayout = Add-Layout $us $layout
                    Write-Verbose "Added layout '$($name)'"
                    $layouts.Add($name, $createdLayout)
                    $l = $l.RowBelow()
		        }
		        catch [System.Exception]
		        {
			        Write-Host "Layout $($name)"
			        Write-Host $_.Exception.ToString()
			        exit
		        }
	        }

            return $layouts
        }
    }

    function Add-Units
    {
         <#
            .SYNOPSIS
            Imports all units of the worksheet "Units" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER Tenants
            The hash table with all imported tenants (key = name, value = tenant)

            .PARAMETER ProductTypes
            The hash table with all imported product types (key = name, value = product type)

            .PARAMETER Layouts
            The hash table with all imported layouts (key = name, value = layout)

            .OUTPUTS
            Returns a HashTable of all units (key = name, value = unit)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $Tenants,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $ProductTypes,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $Layouts
        )

        process
        {
            $ws = $Workbook.Worksheet("Units")
            $headerRow = $ws.FirstRowUsed()
            $u = $headerRow.RowBelow()
	        $units = @{}
            $unitsCount = 0
	        while(!$u.Cell(2).IsEmpty())
	        {
                $unitsCount++
                $u = $u.RowBelow()
            }
        
            $u = $headerRow.RowBelow()
		    $unitUId = 1
	        while(!$u.Cell(2).IsEmpty())
	        {
		        try
		        {
                    $tenantName = $u.Cell(8).Value
			        $tenant = $Tenants[$tenantName]
                    $productTypeName = $u.Cell(9).Value
			        $productType = $ProductTypes[$productTypeName]
                    $name = $u.Cell(2).Value
                
                    $layoutName = $u.Cell(10).Value
                    $layoutId = $null
                    if($layoutName)
                    {
                        $layout = $Layouts[$layoutName]
                        if (-not($layout))
			            {
				            throw "The layout $($layoutName) of the unit $($name) is not found in the list of layouts."
			            }
                    
                        $layoutId = $layout.Id
                    }
                
                    $unit = New-Unit $name $tenant.Id $productType.Id $layoutId
                
                    $description = $u.Cell(6).Value
			        $unit.Description = $description
                    $networkAddress = $u.Cell(5).Value
                    $gatewayAddress = $u.Cell(4).Value
			        $unit.NetworkAddress = $networkAddress
                    $unit.GatewayAddress = $gatewayAddress

				    $unit.UId = $unitUId++
                    Write-Verbose "Adding unit $($unit)"
			        $unit = Add-Unit $us $unit
			        $units.Add($name, $unit)
                    Write-Verbose "Added unit '$($name)' with address '$($networkAddress)'"
		        }
		        catch [System.Exception]
		        {
			        Write-Host "Error while adding unit $($name), tenant: $($tenantName)"
			        Write-Host $_.Exception.ToString()
			        exit
		        }

                $u = $u.RowBelow()
		
		        $pdone = ($units.Count / $unitsCount) * 100
		
		        Write-Progress -Activity "Adding units" -Status "Percent completed:" -PercentComplete $pdone
	        }

            return $units
        }
    }

    function Add-ProtocolTypes
    {
        <#
            .SYNOPSIS
            Imports all protocol types of the worksheet "Protocol types" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .OUTPUTS
            Returns a HashTable of all protocol types (key = name, value = protocol type)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook
        )

        process
        {
            $ws = $Workbook.Worksheet("Protocol types")
            $headerRow = $ws.FirstRowUsed()
            $pt = $headerRow.RowBelow()
	        $protocolTypes = @{}
	        while(!$pt.Cell(2).IsEmpty())
	        {
		        try
		        {
                    $name = $pt.Cell(2).Value
			        $protocolType = New-ProtocolType $name
			        $protocolType = Add-ProtocolType $tds $protocolType
			        $protocolTypes.Add($name, $protocolType)
                    Write-Verbose "Added protocol type '$($name)'"
                    $pt = $pt.RowBelow()
		        }
		        catch [System.Exception]
		        {
			        Write-Host "Protocol $($name)"
			        Write-Host $_.Exception.ToString()
			        exit
		        }
	        }
                
            return $protocolTypes
        }
    }

    function Add-UnitUnitGroupAssociations
    {
          <#
            .SYNOPSIS
            Imports all units of the worksheet "Associations Unit UnitGroup" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER Units
            The hash table with all imported units (key = name, value = unit)

            .PARAMETER UnitGroups
            The hash table with all imported unit groups (key = name, value = unit group)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $Units,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $UnitGroups
        )

        process
        {
            $ws = $Workbook.Worksheet("Associations Unit UnitGroup")
            $headerRow = $ws.FirstRowUsed()
            $a = $headerRow.RowBelow()
	        $i = 0
	        while(!$a.Cell(2).IsEmpty())
	        {
		        try
		        {
                    $unitName = $a.Cell(2).Value
			        $unit = $units[$unitName]
                    $unitGroupName = $a.Cell(3).Value
			        $unitGroup = $unitGroups[$unitGroupName]
                
			        if (-not($unit))
			        {
				        throw "The unit is null for $($unitName)"
			        }
			
                    if (-not($unitGroup))
			        {
				        throw "The unit group is null for $($unitGroupName)"
			        }
            
			        Add-UnitToUnitGroup $us $unitGroup.Id $unit.Id | Out-Null
                    Write-Verbose "Add unit '$($unitName)' to group '$($unitGroupName)'"
		
			        $i ++
		        }
		        catch [System.Exception]
		        {
			        Write-Error "Error while adding unit $($unitName) to unitGroup $($unitGroupName) on line ~$($i)"
			        Write-Error $_.Exception.ToString()
			        exit
		        }

                $a = $a.RowBelow()
	        }       
        }
    }

    function Add-ItcsProviders
    {
          <#
            .SYNOPSIS
            Imports all ITCS providers of the worksheet "ItcsProviders" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER ProtocolTypes
            The hash table with all imported protocol types (key = name, value = protocol type)

            .OUTPUTS
            Returns a HashTable of all ITCS providers (key = name, value = ITCS provider)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $ProtocolTypes
        )

        process
        {
            $ws = $Workbook.Worksheet("ItcsProviders")
            $headerRow = $ws.FirstRowUsed()
            $p = $headerRow.RowBelow()
	        $itcsProviders = @{}
	        while(!$p.Cell(2).IsEmpty())
	        {
                $protocolName = $p.Cell(2).Value
		        $protocolType = $ProtocolTypes[$protocolName]
		        try
		        {		
                    $itcsId = $p.Cell(6).Value
                    $name = $p.Cell(3).Value
                    $properties = $null
                    if($p.Cell(5).IsEmpty())
                    {
                        Write-Warning "The file path is not set in PropertiesFile column for itcs provider $($name)."
                        $propertiesFileNamePath = Join-Path $ExcelDirectory "/$($name).xml"
                        Write-host "Default file name path $($propertiesFileNamePath) will be used instead."                        
                    }
                    else
                    {
                        $propertiesFileNamePath = $p.Cell(5).Value                    
                    }
                    
                    $propertiesPath = Resolve-RelativePath -FilePath $propertiesFileNamePath -BaseDirectory $ExcelDirectory
                    if(-not([string]::IsNullOrEmpty($propertiesPath)))
                    {
                        $properties = [System.Xml.Linq.XElement]::Load($propertiesPath)
                        
                        # Validation of the xml according to the xsd schema
					    if (-not([string]::IsNullOrEmpty($XsdSchemaPath)))
					    {
						    $xsd = $XsdSchemaPath + $ItcsXsdFileName
						    if (!(ValidateXmlAgainstXsd $propertiesPath $xsd))
						    {
							    $properties = $null
							    Write-Warning "The properties are not valid for itcs provider $($name)."
						    }
					    }
					    else
					    {
						    Write-Warning "The xml stream is not validated for provider $($name) because no xsd file found."
					    }
                    }
                    else
                    {
                        Write-Warning "The path for the properties of itcs provider '$($name)' is defined,`                            but the file is not found."
                    }
                
			        $itcs = New-ItcsProvider $protocolType.Id $name $itcsId -Properties $properties
			        $itcs = Add-ItcsProvider $tds $itcs
			        $itcsProviders.Add($name, $itcs)

                    $p = $p.RowBelow()                                                               
		        }
		        catch [System.Exception]
		        {
			        Write-Host "Itcs provider $($p.Name), properties file: $($p.PropertiesFile)"
			        Write-Host $_.Exception.ToString()
			        exit
		        }
	        }

            return $itcsProviders
        }
    }

    function Add-DisplayAreas
    {
        <#
            .SYNOPSIS
            Imports all display areas of the worksheet "DisplayAreas" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER ItcsProviders
            The hash table with all imported ITCS providers (key = name, value = ITCS provider)

            .OUTPUTS
            Returns a HashTable of all display areas (key = name, value = display area)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $ItcsProviders
        )

        process
        {
            $ws = $Workbook.Worksheet("DisplayAreas")
            $headerRow = $ws.FirstRowUsed()
            $a = $headerRow.RowBelow()
	        $itcsDisplayAreas = @{}
	        $itcs = $null
	        while(!$a.Cell(2).IsEmpty())
	        {
                $providerName = $a.Cell(2).Value
		        $itcs = $ItcsProviders[$providerName]
		        if (-not($itcs))
		        {
			        throw "Can't find the ITCS provider for $($providerName)"
			        exit
		        }
		
                $name = $a.Cell(4).Value
			    $properties = $null
                if(!$a.Cell(5).IsEmpty())
                {
                    $definitionPath = $a.Cell(5).Value

                    $definitionFullPath = Resolve-RelativePath -FilePath $definitionPath -BaseDirectory $ExcelDirectory
                    
                    if(-not([string]::IsNullOrEmpty($definitionFullPath)))
                    {                   
                        $properties = [System.Xml.Linq.XElement]::Load($definitionFullPath)
                        
                        # Validation of the xml according to the xsd schema
                        $xsd = $XsdSchemaPath + $VdvSubscriptionXsdFileName
                        if (!(ValidateXmlAgainstXsd $definitionFullPath $xsd))
                        {
                            $properties = $null
                            Write-Warning "The properties are not valid for itcs area display $($name)."
                        } 
                    }
                    else
                    {
                        Write-Warning "The path for the definition of the special properties for display area `                            '$($name)' was defined, but the file was not found"
                    }
                }

                $filter = @{
                        "ProviderName" = $providerName;
                        "DisplayAreaName" = $name;
                        }
                $key = Get-DisplayAreaFilterKey $filter
                if (-not(Test-DisplayAreasFilter $itcsDisplayAreas $key)) 
                {            
		            $itcsDisplayArea = New-ItcsDisplayArea $name -ProviderId $itcs.Id -Properties $properties
		            $itcsDisplayArea = Add-ItcsDisplayArea $tds $itcsDisplayArea
                    try
                    {
		                $itcsDisplayAreas.Add($key, $itcsDisplayArea)
                    }
                    catch [ArgumentException]
                    {
                        Write-Error "Duplicated <Provider name, display area name>, row not added: $($providerName), `                            $($name)"
                        Write-Host $_.Exception.ToString()
                    }
                }
                else
                {
                    Write-Warning "Duplicated key <Provider Name, display area name>, row not added: $($providerName), `                        $($name)"
                }

                $a = $a.RowBelow()
	        }
            
            return $itcsDisplayAreas
        }
    }

    function Add-StopPoints
    {
         <#
            .SYNOPSIS
            Imports all stop points of the worksheet "StopPoints" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .OUTPUTS
            Returns a HashTable of all stop points (key = name, value = stop point)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook
        )

        process
        {
            $ws = $Workbook.Worksheet("StopPoints")
            $headerRow = $ws.FirstRowUsed()
            $sp = $headerRow.RowBelow()
	        $stopPoints = @{}
	        while(!$sp.Cell(2).IsEmpty())
	        {
                $name = $sp.Cell(2).Value
		        $stopPoint = New-StopPoint $name
		        $createdStopPoint = Add-StopPoint $us $stopPoint
		        $stopPoints.Add($name, $createdStopPoint)
                $sp = $sp.RowBelow()
	        }

            return $stopPoints
        }
    }

    function Add-UnitStopPointAssociations
    {
         <#
            .SYNOPSIS
            Imports all display areas of the worksheet "DisplayAreas" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER Units
            The hash table with all imported units (key = name, value = unit)

            .PARAMETER StopPoints
            The hash table with all imported stop points (key = name, value = stop point)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $Units,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $StopPoints
        )

        process
        {
            $ws = $wb.Worksheet("UnitsStopPointsAssociations")
            $headerRow = $ws.FirstRowUsed()
            $a = $headerRow.RowBelow()
	        $stopPointsAssociations = @{}
	        $stopPoint = $null
	        $unit = $null
	        $stopPoint = $null
	        while(!$a.Cell(2).IsEmpty())
	        {
                $stopPointName = $a.Cell(2).Value
		        $stopPoint = $StopPoints[$stopPointName]
                $unitName = $a.Cell(3).Value
		        $unit = $Units[$unitName]
		        if (-not($stopPoint))
		        {
			        throw "Can't find stop point $($stopPointName)"
			        exit
		        }
		
		        if (-not($unit))
		        {
			        throw "Can't find unit '$($unitName)'"
			        exit
		        }
		
		        Connect-StopPoint $us $stopPoint $unit | Out-Null
                $a = $a.RowBelow()
	        }
        }
    }

    function Add-ItcsFilters
    {
         <#
            .SYNOPSIS
            Imports all ITCS filters of the worksheet "ItcsFilters" into database.

            .PARAMETER Workbook
            The workbook corresponding to the excel file specified in the script parameter.

            .PARAMETER ItcsProviders
            The hash table with all imported ITCS providers (key = name, value = ITCS provider)

            .PARAMETER StopPoints
            The hash table with all imported stop points (key = name, value = stop point)

            .PARAMETER DisplayAreas
            The hash table with all imported display areas (key = name, value = display area)
        #>
        param
        (
            [Parameter(Mandatory = $true)]
            [ClosedXML.Excel.XLWorkbook]
            $Workbook,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $ItcsProviders,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $StopPoints,

            [Parameter(Mandatory = $true)]
            [System.Collections.Hashtable]
            $DisplayAreas
        )

        process
        {
            $ws = $Workbook.Worksheet("ItcsFilters")
            $headerRow = $ws.FirstRowUsed()
            $f = $headerRow.RowBelow()
	        # $itcsFilters = @{} 
            $filter = @{
                        "ProviderName" = "provider";
                        "DisplayAreaName" = "displayArea";
                        }
            $i = 0
	        while(!$f.Cell(2).IsEmpty())
	        {
                $stopPointName = $f.Cell(2).Value
		        $stopPoint = $StopPoints[$stopPointName]
		        if(-not($stopPoint))
		        {
			        throw "Can't find stop point $($stopPointName). Aborting the operation"
			        exit
		        }
		    
                $providerName = $f.Cell(3).Value
		        $itcs = $ItcsProviders[$providerName]
                $displayAreaName = $f.Cell(4).Value
                $lineReference = $f.Cell(5).Value
		        $lineText = $f.Cell(6).Value
		
                $propertiesFilePath = $f.Cell(9).Value
                $propertiesFile = $null
                if (!$f.Cell(9).IsEmpty())
                {
                    $propertiesFile = Resolve-RelativePath -FilePath $propertiesFilePath -BaseDirectory $ExcelDirectory
		            if ([string]::IsNullOrEmpty($propertiesFile))
		            {
			            $propertiesFile = $null
		            }
		        }
		
                $directionText = $f.Cell(8).Value
                $directionReference = $f.Cell(7).Value

                $filter.ProviderName = $providerName
                $filter.DisplayAreaName = $displayAreaName
                $key = Get-DisplayAreaFilterKey $filter
		        $displayArea = $DisplayAreas[$key]
                if (-not($displayArea))
                {
                    throw "Can't find <provider name/display area> : <$($providerName), $($displayAreaName)>"
			        exit
                }
                else
		        {
                    Add-ItcsFilter $tds -StopPointId $stopPoint.Id -DisplayAreaId $displayArea.Id `                        -LineText $lineText -LineReference $lineReference -DirectionText $directionText `                        -DirectionReference $directionReference -Properties $propertiesFile | Out-Null
                }

                $i++
                $f = $f.RowBelow()
	        }
            
            return $i
        }
    }

    try
    {	
    	if ($UseNetNamedPipeBinding)
	{
        	$xs = New-NetNamedPipeBindingProxyConfiguration -Path MaintenanceService.svc
	}
	else
	{
		$xs = New-CertificateAuthenticatedClientProxyConfiguration `
                -RemoteAddress "$($ServicesBaseAddress)/MaintenanceService.svc/Certificate" `
                -DnsName "BackgroundSystem" -CertificateName "CenterOnline"
	}

	    if(!$SkipReset)
		{
            Write-Host "Reseting database..."
			Reset-BackgroundSystem $xs
		}
		
		if ($Interactive)
		{
			Read-Host "Type <Enter> to continue"
		}

	    

        # Creation of the xml files containing the itcs properties from a previous system 
        # calling another script ExportItcsConfigurations.ps1.
        if($ExportItcs)
        {
            $exportItcsScript = Join-Path $scriptDirectory "ExportItcsConfigurations.ps1"
            $d = Split-Path $AbsoluteExcelPath -Parent
            & $exportItcsScript -WorkingDirectory $d -OverwriteFiles -ExcelFileName $AbsoluteExcelPath -ShowWarnings
        }

        $wb = New-Object "ClosedXML.Excel.XLWorkbook"($AbsoluteExcelPath)

        # Add tenants
        Write-Progress -Activity "Adding tenants" -Status "Percent completed:" -PercentComplete 5
        $tenants = Add-Tenants -Workbook $wb
        Write-Host "Added $($tenants.Count) tenant(s)"

        # Add user roles
        Write-Progress -Activity "Adding user roles" -Status "Percent completed:" -PercentComplete 10
        $userRoles = Add-UserRoles -Workbook $wb
        Write-Host "Added $($userRoles.Count) user role(s)."

        # Add permissions
	    Write-Progress -Activity "Adding permissions" -Status "Percent completed:" -PercentComplete 15
        $permissions = Add-Permissions -Workbook $wb
        Write-Host "Added $($permissions.Count) permission(s)."

        # Add data scope
	    Write-Progress -Activity "Adding data scopes" -Status "Percent completed:" -PercentComplete 20
        $dataScopes = Add-DataScopes -Workbook $wb
        Write-Host "Added $($dataScopes.Count) dataScope(s)."

        # Add users
	    Write-Progress -Activity "Adding users" -Status "Percent completed:" -PercentComplete 25
        $users = Add-Users -Workbook $wb
        Write-Host "Added $($users.Count) user(s)."

        # Associate tenants and users
	    Write-Progress -Activity "Associating tenants and users" -Status "Percent completed:" -PercentComplete 30
        Add-UserTenantAssociations -Workbook $wb -Tenants $tenants -Users $users -UserRoles $userRoles
       
        # Grant permissions
	    Write-Progress -Activity "Granting permissions" -Status "Percent completed:" -PercentComplete 35
        Grant-Permissions -Workbook $wb -Permissions $permissions -DataScopes $dataScopes -UserRoles $userRoles

        # Add unit types
	    Write-Progress -Activity "Adding unit types" -Status "Percent completed:" -PercentComplete 40
        $unitTypes = Add-UnitTypes -Workbook $wb
        Write-Host "Added $($unitTypes.Count) unit type(s)."
        
        # Add products types
	    Write-Progress -Activity "Adding product types" -Status "Percent completed:" -PercentComplete 45
        $productTypes = Add-ProductTypes -Workbook $wb -UnitTypes $unitTypes
        Write-Host "Added $($productTypes.Count) product type(s)."

        # Add unit group types
        Write-Progress -Activity "Adding unitgroup types" -Status "Percent completed:" -PercentComplete 50
        $unitGroupTypes = Add-UnitGroupTypes -Workbook $wb
        Write-Host "Added $($unitGroupTypes.Count) unit group type(s)."
        
        # Add unit groups
        Write-Progress -Activity "Adding unit groups" -Status "Percent completed:" -PercentComplete 55
        $unitGroups = Add-UnitGroups -Workbook $wb -Tenants $tenants -UnitGroupTypes $unitGroupTypes
        Write-Host "Added $($unitGroups.Count) unit group(s)."               
        
        # Add layouts
        Write-Progress -Activity "Adding layouts" -Status "Percent completed:" -PercentComplete 60
        $layouts = Add-Layouts -Workbook $wb
        Write-Host "Added $($layouts.Count) layout(s)"

        # Add units
	    Write-Progress -Activity "Adding units" -Status "Percent completed:" -PercentComplete 0
        $units = Add-Units -Workbook $wb -Tenants $tenants -ProductTypes $productTypes -Layouts $layouts
        Write-Host "Added $($units.Count) unit(s)"

        # Add protocol types
	    Write-Progress -Activity "Adding protocol types" -Status "Percent completed:" -PercentComplete 55
        $protocolTypes = Add-ProtocolTypes -Workbook $wb
        Write-Host "Added $($protocolTypes.Count) protocol type(s)"

        # Associates units to unit groups
        Write-Progress -Activity "Adding unit to unit group" -Status "Percent completed:" -PercentComplete 60
        Add-UnitUnitGroupAssociations -Workbook $wb -Units $units -UnitGroups $unitGroups
        
        # Add itcs providers
	    Write-Progress -Activity "Adding itcs providers" -Status "Percent completed:" -PercentComplete 65
        $itcsProviders = Add-ItcsProviders -Workbook $wb -ProtocolTypes $protocolTypes
        Write-Host "Added $($itcsProviders.Count) Itcs provider(s)"
                
        # Add Display areas
	    Write-Progress -Activity "Adding display areas" -Status "Percent completed:" -PercentComplete 70
        $itcsDisplayAreas = Add-DisplayAreas -Workbook $wb -ItcsProviders $itcsProviders
        Write-Host "Added $($itcsDisplayAreas.Count) display area(s)."

        # Add Stop points
	    Write-Progress -Activity "Adding stop points" -Status "Percent completed:" -PercentComplete 75
        $stopPoints = Add-StopPoints -Workbook $wb
        Write-Host "Added $($stopPoints.Count) stop point(s)."
                
        # Link stop points with units
	    Write-Progress -Activity "Connecting stop points with units" -Status "Percent completed:" -PercentComplete 80
        Add-UnitStopPointAssociations -Workbook $wb -Units $units -StopPoints $stopPoints
        
        # Add itcs filters
	    Write-Progress -Activity "Adding itcs filters" -Status "Percent completed:" -PercentComplete 90
        $itcsFilterCount = Add-ItcsFilters -Workbook $wb -ItcsProviders $itcsProviders -StopPoints $stopPoints -DisplayAreas $itcsDisplayAreas
        Write-Host "Added $($itcsFilterCount) itcs filter(s)."
    
        # Happy end !
	    Write-Host "Data successfully imported."

        # Free the memory
        $wb.Dispose()
    }
    catch
    {
        throw
    }
}

end
{
    # Collect garbage
    [System.GC]::Collect()
}