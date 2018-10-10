<#
    .SYNOPSIS
    Adds default resources to a BackgroundSystem 2.0 installation

    .DESCRIPTION
    This script performs the setup of a BackgroundSystem installation adding the product types (taken
    as described in the Gorba.Common.Configuration.HardwareDescription.HardwareDescriptors object) and
    the software packages contained in the Software directory.
    Additionaly default user roles are added ('Tenant Administrator', 'Unit Administrator').
    If a user role already exists nothing is changed to that user role.

    .PARAMETER CenterPortalAddress
    Address of the Center Portal used to get the configuration. By default, it uses the local
    configuration (Portal not required).

    .PARAMETER SkipProductTypes
    If set, product types won't be imported.

    .PARAMETER SkipResourceUpload
    If set, uploads won't be uploaded, but only added from an entity point of view.

    .PARAMETER SkipAddingUserRoles
    If set, no default users will be added

    .PARAMETER UpdateProductTypes
    If set, the script only updates the product types. All other flags are ignored.

    .PARAMETER Check
    If set, the script only verifies that all resources are available on the system.

    .PARAMETER ForceResourceUpload
    If set, the script will try to upload the resources to the storage even if the table entry is already there.
#>
[CmdLetBinding(DefaultParameterSetName = "Install", SupportsShouldProcess = $true)]
param
(
    [Parameter()]
    [string]
    $CenterPortalAddress = "local",

    [Parameter(ParameterSetName = "Install")]
    [switch]
    $SkipProductTypes,

    [Parameter(ParameterSetName = "Install")]
    [switch]
    $SkipResourceUpload,

    [Parameter(ParameterSetName = "Install")]
    [switch]
    $SkipAddingUserRoles,

    [Parameter(ParameterSetName = "Update")]
    [switch]
    $UpdateProductTypes,

    [Parameter(ParameterSetName = "Check")]
    [switch]
    $Check,
        
    [Parameter(ParameterSetName = "Install")]
    [switch]
    $ForceResourceUpload
)

begin
{
    function Get-ProductTypes
    {
        param
        (
            $Assembly
        )

        process
        {
            [System.Type] $type = $Assembly.GetType("Gorba.Common.Configuration.HardwareDescription.HardwareDescriptors")
            $type.GetNestedTypes().GetProperties()
        }
    }

    function Get-UserCredentials
    {
        <#
            .SYNOPSIS
            Gets information required from the user.
        #>
        process
        {
            $Password = Read-Host "Please enter password for 'admin'"
            return (New-UserCredentials -Username "admin" -Password $Password)
        }
    }

    function Get-SofwareDescriptor
    {
        param
        (
            [Parameter(Mandatory = $true)]
            [string]
            $Path
        )

        process
        {
            $content = Get-Content -Path $Path | Out-String
            $stringReader = New-Object System.IO.StringReader($content)
            $serializer = New-Object System.Xml.Serialization.XmlSerializer([Gorba.Center.Admin.SoftwareDescription.SoftwarePackageDescriptor])
            $textReader = New-Object System.Xml.XmlTextReader($stringReader)
            $softwarePackage = $serializer.Deserialize($textReader)
            $textReader.Close()
            $stringReader.Close()
            return $softwarePackage
        }
    }

    function Import-FolderUpdate
    {
        [CmdletBinding(SupportsShouldProcess = $true)]
        param
        (
            $Path
        )

        process
        {
            $folder = New-Object Gorba.Common.Update.ServiceModel.Messages.FolderUpdate
            $folder.Name = $Path.Name
            $items = ls "$($Path.FullName)\*"
            foreach ($item in $items)
            {
                if ($item.PSIsContainer)
                {
                    $child = Import-FolderUpdate $item
                    $folder.Items.Add($child)
                    continue
                }
                
                Write-Host "item fullname: $($item.FullName)"
                $hash = [Gorba.Common.Update.ServiceModel.Resources.ResourceHash]::Create($item.FullName)
                $query = [Gorba.Center.Common.ServiceModel.Filters.Resources.ResourceQuery]::Create()
                $query = [Gorba.Center.Common.ServiceModel.Filters.Resources.ResourceFilterExtensions]::WithHash($query, $hash, [Gorba.Center.Common.ServiceModel.Filters.StringComparison]::ExactMatch)
                $existingResource = Get-Resource -UserCredentials $UserCredentials -Configuration $Configuration -Filter $query
                if (($existingResource -ne $null) -and (-not $ForceResourceUpload))
                {
                    Write-Warning "Resource already exists for file '$($item.FullName)', not uploading it"
                }
                elseif ($SkipResourceUpload)
                {
                    $resource = New-Resource
                    $resource.Description = "Resource imported during the setup"
                    $resource.Hash = $hash
                    $resource.Length = $item.Length
                    $resource.MimeType = "application/octet-stream"
                    $resource.OriginalFileName = $item.Name
                    if ($WhatIfPreference)
                    {
                        Write-Host -ForegroundColor DarkGreen "Add resource $($item.FullName)"
                    }
                    else
                    {
                        Add-Resource -UserCredentials $UserCredentials -Configuration $Configuration -InputObject $resource | Out-Null
                    }
                }
                else
                {
                    if ($WhatIfPreference)
                    {
                        Write-Host -ForegroundColor DarkGreen "Import resource $($item.FullName)"
                    }
                    else
                    {
                        try
                        {
                            $resource = Import-Resource -UserCredentials $UserCredentials -Configuration $Configuration -Path $item.FullName -MimeType "application/octet-stream"
                        }
                        catch
                        {
                            Write-Warning "Resource and File '$($item.FullName)' already exist, not uploaded"
                        }
                    }
                }

                $fileUpdate = New-Object Gorba.Common.Update.ServiceModel.Messages.FileUpdate
                $fileUpdate.Hash = $hash
                $fileUpdate.Name = $item.Name
                $folder.Items.Add($fileUpdate)
            }

            return $folder
        }
    }

    function Import-SoftwareFolder
    {
        [CmdletBinding(SupportsShouldProcess = $true)]
        param
        (
            [Parameter(Mandatory = $true)]
            [Gorba.Center.Common.ServiceModel.Security.UserCredentials]
            $UserCredentials,

            [Parameter(Mandatory = $true)]
            [Gorba.Center.Common.ServiceModel.BackgroundSystemConfiguration]
            $Configuration,

            [Parameter(Mandatory = $true)]
            [ValidateScript({Test-Path $_})]
            [string]
            $Path
        )

        process
        {
            $softwareDescriptor = Get-SofwareDescriptor -Path $Path

            $folderName = [System.IO.Path]::GetFileNameWithoutExtension($Path)
            $softwareFolder = Join-Path (Split-Path $Path -Parent) $folderName
            $items = ls "$($softwareFolder)\*"
            foreach ($item in $items)
            {
                $folder = Import-FolderUpdate $item
                $softwareDescriptor.Version.Structure.Folders.Add($folder)
            }

            if ($WhatIfPreference)
            {
                Write-Host -ForegroundColor DarkGreen "Import package $($softwareDescriptor.PackageId)"
            }
            else
            {
                Import-PackageVersion -UserCredentials $UserCredentials -Configuration $Configuration -PackageId $softwareDescriptor.PackageId -ProductName $softwareDescriptor.Name -SoftwareVersion $softwareDescriptor.Version.VersionNumber -Structure $softwareDescriptor -VersionDescription $softwareDescriptor.Version.Description | Out-Null
            }
        }
    }

    function Add-AuthorizationNotDuplicated
    {
        <#
            .SYNOPSIS
            Adds the default user roles to the background system if it not exists.
        #>
        [CmdletBinding(SupportsShouldProcess = $true)]
        param
        (
            [Parameter(Mandatory = $true)]
            [Gorba.Center.Common.ServiceModel.Security.UserCredentials]
            $UserCredentials,

            [Parameter(Mandatory = $true)]
            [Gorba.Center.Common.ServiceModel.BackgroundSystemConfiguration]
            $Configuration,
                        
            [Parameter(Mandatory = $true)]
            [Gorba.Center.Common.ServiceModel.AccessControl.Authorization]
            $Authorization
        )
        process
        {
            # check if authorization already exists
            $AuthorizationQuery = [Gorba.Center.Common.ServiceModel.Filters.AccessControl.AuthorizationQuery]::Create()
            
            $AuthorizationQuery = [Gorba.Center.Common.ServiceModel.Filters.AccessControl.AuthorizationFilterExtensions]::WithUserRole($AuthorizationQuery, $Authorization.UserRole)
            
            $existingAuthorization = Get-Authorization -UserCredentials $UserCredentials -Configuration $Configuration -Filter $AuthorizationQuery
            #workaround for filters
            $found = $false
            foreach ( $a in $existingAuthorization) 
            {
                if (($a.Permission -eq $Authorization.Permission) -and  ($a.DataScope -eq $Authorization.DataScope) -and ($a.UserRole.Id -eq $Authorization.UserRole.Id))
                {
                    $found = $true
                    break;
                }
            }

            if ($found)
            {
                return
            }

            $catcher = Add-Authorization $Authorization -UserCredentials $UserCredentials -Configuration $Configuration 
        }
    }

    function Add-TenantAdministrator
    {
        <#
            .SYNOPSIS
            Adds the default 'Tenant Administrator' to the background system
        #>
        [CmdletBinding(SupportsShouldProcess = $true)]
        param
        (
            [Parameter(Mandatory = $true)]
            [Gorba.Center.Common.ServiceModel.Security.UserCredentials]
            $UserCredentials,

            [Parameter(Mandatory = $true)]
            [Gorba.Center.Common.ServiceModel.BackgroundSystemConfiguration]
            $Configuration
        )
        process
        {
            #chek if Tenant Administrator is already defined
            $userRoleName = "Tenant Administrator"
            $userRoleDescription = "Default administrator for tenants."
            
            $query = [Gorba.Center.Common.ServiceModel.Filters.AccessControl.UserRoleQuery]::Create()
            $query = [Gorba.Center.Common.ServiceModel.Filters.AccessControl.UserRoleFilterExtensions]::WithName($query, $userRoleName, [Gorba.Center.Common.ServiceModel.Filters.StringComparison]::ExactMatch)

            $userRole = Get-UserRole -UserCredentials $UserCredentials -Configuration $Configuration -Filter $query
            if ($userRole) 
            {
                Write-Host "[Skip] '$userRoleName' (already exists)"
            }
            else
            {
                # create user role
                $newUserRole = New-UserRole
                $newUserRole.Name = $userRoleName
                $newUserRole.Description = $userRoleDescription
                $catcher = Add-UserRole $newUserRole -UserCredentials $UserCredentials -Configuration $Configuration

                # get the user from BS
                $UserRole = Get-UserRole -UserCredentials $UserCredentials -Configuration $Configuration -Filter $query

                # add authorizations
                $DataScopeList = @()
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::Tenant
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::User
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::Unit
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::Update
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::UnitConfiguration
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::MediaConfiguration
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::Meta
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::CenterAdmin
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::CenterMedia
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::CenterDiag

                $permissionList = @()
                $permissionList += [Gorba.Center.Common.ServiceModel.AccessControl.Permission]::Create
                $permissionList += [Gorba.Center.Common.ServiceModel.AccessControl.Permission]::Read
                $permissionList += [Gorba.Center.Common.ServiceModel.AccessControl.Permission]::Write
                $permissionList += [Gorba.Center.Common.ServiceModel.AccessControl.Permission]::Delete
                $permissionList += [Gorba.Center.Common.ServiceModel.AccessControl.Permission]::Interact
                $permissionList += [Gorba.Center.Common.ServiceModel.AccessControl.Permission]::Abort

                foreach ($scope in $DataScopeList)
                {
                    foreach ($permission in $permissionList)
                    {
                        $newAuthorization = New-Authorization 
                        $newAuthorization.DataScope = $scope
                        $newAuthorization.Permission = $permission
                        $newAuthorization.UserRole =$userRole
                        Add-AuthorizationNotDuplicated $newAuthorization -UserCredentials $UserCredentials -Configuration $Configuration 
                    }
                }
            }

        }
    }

    function Set-UnitType {
        [CmdletBinding(SupportsShouldProcess = $true)]
        param
        (
            <#
            .SYNOPSIS
            Sets the UnitType for a given product type by reading  the HardwareDescriptor.
            #>
            [Parameter(Mandatory = $true)]
            [Gorba.Center.Common.ServiceModel.Units.ProductType]
            [ref] $productType
            
        )
        process
        {
            $desciptorSerializer = New-Object System.Xml.Serialization.XmlSerializer([Gorba.Common.Configuration.HardwareDescription.HardwareDescriptor])
            $stringReader = New-Object System.IO.StringReader($productType.HardwareDescriptorXml)
            $xmlTextReader = New-Object System.Xml.XmlTextReader($stringReader)
            $descriptor = [Gorba.Common.Configuration.HardwareDescription.HardwareDescriptor]$desciptorSerializer.Deserialize($xmlTextReader)

            if ($descriptor.Platform -is [Gorba.Common.Configuration.HardwareDescription.ThorebPlatformDescriptor]) 
            {
                $productType.UnitType = [Gorba.Center.Common.ServiceModel.Units.UnitTypes]::Obu
            }
			elseif ($descriptor.Platform -is [Gorba.Common.Configuration.HardwareDescription.PowerUnitPlatformDescriptor])
			{
				$productType.UnitType = [Gorba.Center.Common.ServiceModel.Units.UnitTypes]::EPaper
			}
        }
    }

    function Add-UnitAdministrator
    {
        <#
            .SYNOPSIS
            Adds the default 'Unit Administrator' to the background system
        #>
        [CmdletBinding(SupportsShouldProcess = $true)]
        param
        (
            [Parameter(Mandatory = $true)]
            [Gorba.Center.Common.ServiceModel.Security.UserCredentials]
            $UserCredentials,

            [Parameter(Mandatory = $true)]
            [Gorba.Center.Common.ServiceModel.BackgroundSystemConfiguration]
            $Configuration
        )
        process
        {
            #chek if Unit Administrator is already defined
            $userRoleName = "Unit Administrator"
            $userRoleDescription = "Default administrator for units."
            
            $query = [Gorba.Center.Common.ServiceModel.Filters.AccessControl.UserRoleQuery]::Create()
            $query = [Gorba.Center.Common.ServiceModel.Filters.AccessControl.UserRoleFilterExtensions]::WithName($query, $userRoleName, [Gorba.Center.Common.ServiceModel.Filters.StringComparison]::ExactMatch)

            $userRole = Get-UserRole -UserCredentials $UserCredentials -Configuration $Configuration -Filter $query
            if ($userRole) 
            {
                Write-Host "[Skip] '$userRoleName' (already exists)"
            }
            else
            {
                # create user role
                $newUserRole = New-UserRole
                $newUserRole.Name = $userRoleName
                $newUserRole.Description = $userRoleDescription
                $catcher = Add-UserRole $newUserRole -UserCredentials $UserCredentials -Configuration $Configuration

                # get the user from BS
                $UserRole = Get-UserRole -UserCredentials $UserCredentials -Configuration $Configuration -Filter $query

                # add authorizations
                $DataScopeList = @()
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::Unit
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::Update
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::UnitConfiguration
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::MediaConfiguration
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::Meta
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::CenterAdmin
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::CenterMedia
                $DataScopeList += [Gorba.Center.Common.ServiceModel.AccessControl.DataScope]::CenterDiag

                $permissionList = @()
                $permissionList += [Gorba.Center.Common.ServiceModel.AccessControl.Permission]::Create
                $permissionList += [Gorba.Center.Common.ServiceModel.AccessControl.Permission]::Read
                $permissionList += [Gorba.Center.Common.ServiceModel.AccessControl.Permission]::Write
                $permissionList += [Gorba.Center.Common.ServiceModel.AccessControl.Permission]::Delete
                $permissionList += [Gorba.Center.Common.ServiceModel.AccessControl.Permission]::Interact
                $permissionList += [Gorba.Center.Common.ServiceModel.AccessControl.Permission]::Abort

                foreach ($scope in $DataScopeList)
                {
                    foreach ($permission in $permissionList)
                    {
                        $newAuthorization = New-Authorization 
                        $newAuthorization.DataScope = $scope
                        $newAuthorization.Permission = $permission
                        $newAuthorization.UserRole =$userRole
                        Add-AuthorizationNotDuplicated $newAuthorization -UserCredentials $UserCredentials -Configuration $Configuration 
                    }
                }
            }

        }
    }

    function Add-DefaultUserRoles
    {
        <#
            .SYNOPSIS
            Adds the default user roles to the background system
        #>
        [CmdletBinding(SupportsShouldProcess = $true)]
        param
        (
            [Parameter(Mandatory = $true)]
            [Gorba.Center.Common.ServiceModel.Security.UserCredentials]
            $UserCredentials,

            [Parameter(Mandatory = $true)]
            [Gorba.Center.Common.ServiceModel.BackgroundSystemConfiguration]
            $Configuration
        )

        process
        {
            $numberOfEntrys = 2
            $completed      = 0
            Write-Progress -Id 4 -ParentId 1 -Activity "Createing default user roles" -Status "Creating ($($completed) / $($numberOfEntrys))" -PercentComplete ($completed / $numberOfEntrys * 100)
            
            Add-TenantAdministrator -UserCredentials $UserCredentials -Configuration $Configuration
            $completed++
            Write-Progress -Id 4 -ParentId 1 -Activity "Createing default user roles" -Status "Creating ($($completed) / $($numberOfEntrys))" -PercentComplete ($completed / $numberOfEntrys * 100)

            Add-UnitAdministrator -UserCredentials $UserCredentials -Configuration $Configuration
            $completed++
            Write-Progress -Id 4 -ParentId 1 -Activity "Createing default user roles" -Status "Creating ($($completed) / $($numberOfEntrys))" -PercentComplete ($completed / $numberOfEntrys * 100)

            Write-Progress -Id 4 -ParentId 1 -Activity "Createing default user roles" -Status "Completed" -Completed
        }
    }

    $cmdletsDirectory = Join-Path $PSScriptRoot "Cmdlets"

    $cmdlets = Join-Path $cmdletsDirectory "BackgroundSystemCmdlets.psd1"
    if (-not(Test-Path $cmdlets))
    {
        throw "Can't find cmdlets (path: $($cmdlets))"
    }

    $binaries = Join-Path $PSScriptRoot "Binaries"
    if (-not(Test-Path $binaries))
    {
        throw "Can't find binaries (path: $($binaries))"
    }

    $software = Join-Path $PSScriptRoot "Software"
    if (-not(Test-Path $software))
    {
        throw "Can't find Software (path: $($software))"
    }

    Import-Module $cmdlets
    
    $update = Join-Path $cmdletsDirectory "Gorba.Common.Update.ServiceModel.dll"
    $updateAssembly = [System.Reflection.Assembly]::LoadFrom($update)
    $hardwareDescriptor = Join-Path $binaries "Gorba.Common.Configuration.HardwareDescription.dll"
    $hardwareDescriptorAssembly = [System.Reflection.Assembly]::LoadFrom($hardwareDescriptor)
    $softwareDescriptor = Join-Path $binaries "Gorba.Center.Admin.SoftwareDescription.dll"
    [System.Reflection.Assembly]::LoadFrom($softwareDescriptor) | Out-Null
    $productTypes = Get-ProductTypes $hardwareDescriptorAssembly
}

process
{
    $userCredentials = Get-UserCredentials -Password $Password
    Write-Host "Using Portal: $($CenterPortalAddress)"

    $systemConfiguration = Get-BackgroundSystemConfiguration -CenterPortalAddress $CenterPortalAddress
    Write-Verbose "Setup using following data services configuration:"
    $systemConfiguration.DataServices
    Write-Verbose "Setup using following data services configuration:"
    $systemConfiguration.FunctionalServices

    if ($UpdateProductTypes)
    {
        Write-Host "Updating product types"
        foreach ($i in 1..$productTypes.Length)
        {
            $productTypeDescriptor = $productTypes[$i - 1]
            $hardwareDescriptor = $productTypeDescriptor.GetValue($null)
    
            $productTypeName = $hardwareDescriptor.Name
            $query = [Gorba.Center.Common.ServiceModel.Filters.Units.ProductTypeQuery]::Create()
            $query = [Gorba.Center.Common.ServiceModel.Filters.Units.ProductTypeFilterExtensions]::WithName($query, $productTypeName, [Gorba.Center.Common.ServiceModel.Filters.StringComparison]::ExactMatch)
            
            $productType = Get-ProductType -UserCredentials $userCredentials -Configuration $systemConfiguration -Filter $query
            if ($productType)
            {
                Write-Host "Product type $($productTypeDescriptor.Name) found with Id $($productType.Id)"
                $productType.HardwareDescriptor = New-Object Gorba.Center.Common.ServiceModel.XmlData($hardwareDescriptor)
                Set-UnitType ([ref]$productType)
                Update-ProductType -UserCredentials $userCredentials -Configuration $systemConfiguration -InputObject $productType | Out-Null
                Write-Host "Product type updated"

                continue
            }
            
            $productType = New-ProductType
            $productType.Name = $productTypeName
            Write-Verbose "Adding product type '$($productTypeName)'"
            Write-Verbose "ProductTypeDescriptor: $($hardwareDescriptor.GetType().FullName)"
            $productType.HardwareDescriptor = New-Object Gorba.Center.Common.ServiceModel.XmlData($hardwareDescriptor)
            if ($WhatIfPreference)
            {
                Write-Host -ForegroundColor DarkGreen "Adding the following product type:"
                Write-Host -ForegroundColor DarkGreen $productType
            }
            else
            {
                Set-UnitType ([ref]$productType)
                $productType = Add-ProductType -UserCredentials $userCredentials -Configuration $systemConfiguration -InputObject $productType
            }
        }
    }
    elseif ($Check)
    {
        Write-Host "Check"
        Write-Progress -Id 1 -Activity Check -Status "Checking resources"
        $resources = ls "$($software)\*" -Recurse -Exclude "*.xml" | ?{ -not($_.PSIsContainer) }
        $resourcesCount = $resources.Length
        
        foreach ($i in 1..$resourcesCount)
        {
            Write-Progress -Id 2 -ParentId 1 -Activity Check -Status "Checking resource ($($i)/$($resourcesCount))" -PercentComplete ($i / $resourcesCount * 100)
            $resource = $resources[$i - 1]
            $hash = [Gorba.Common.Update.ServiceModel.Resources.ResourceHash]::Create($resource)
            $resourceQuery = [Gorba.Center.Common.ServiceModel.Filters.Resources.ResourceQuery]::Create()
            $resourceQuery = [Gorba.Center.Common.ServiceModel.Filters.Resources.ResourceFilterExtensions]::WithHash($resourceQuery, $hash, [Gorba.Center.Common.ServiceModel.Filters.StringComparison]::ExactMatch)
            $r = Get-Resource -UserCredentials $userCredentials -Configuration $systemConfiguration -Filter $resourceQuery
            if ($r)
            {
                Write-Host -ForegroundColor DarkGreen "Resource '$($resource)' exists with id $($r.Id)"
            }
            else
            {
                Write-Host -ForegroundColor Red "Resource '$($resource)' not found"
                Read-Host "Type <Enter> to continue"
            }
        }
        
        Write-Progress -Id 2 -ParentId 1 -Activity Check -Status "Completed" -Completed
        Write-Progress -Id 1 -Activity Check -Status "Completed" -Completed

    }
    else
    {

        $steps = 1
        if ($SkipProductTypes)
        {
            Write-Host "Skipping product types"
        }
        else
        {
            $steps += 1
        }

        Write-Progress -Id 1 -Activity Setup -Status "Setup (1/$($steps))"

        if (-not($SkipProductTypes))
        {
            foreach ($i in 1..$productTypes.Length)
            {
                $productTypeDescriptor = $productTypes[$i - 1]
                $hardwareDescriptor = $productTypeDescriptor.GetValue($null)
    
                $productTypeName = $hardwareDescriptor.Name
                Write-Progress -Id 2 -ParentId 1 -Activity "Adding product types" -Status "Adding '$($productTypeName)' ($($i) / $($productTypes.Length))" -PercentComplete ($i / $productTypes.Length * 100)
                $query = [Gorba.Center.Common.ServiceModel.Filters.Units.ProductTypeQuery]::Create()
                $query = [Gorba.Center.Common.ServiceModel.Filters.Units.ProductTypeFilterExtensions]::WithName($query, $productTypeName, [Gorba.Center.Common.ServiceModel.Filters.StringComparison]::ExactMatch)
                $productType = Get-ProductType -UserCredentials $userCredentials -Configuration $systemConfiguration -Filter $query
                if ($productType)
                {
                    Write-Host "Product type $($productTypeDescriptor.Name) already exists with Id $($productType.Id)"
                    continue
                }

                $productType = New-ProductType
                $productType.Name = $productTypeName
                Write-Verbose "Adding product type '$($productTypeName)'"
                Write-Verbose "ProductTypeDescriptor: $($hardwareDescriptor.GetType().FullName)"
                $productType.HardwareDescriptor = New-Object Gorba.Center.Common.ServiceModel.XmlData($hardwareDescriptor)
                if ($WhatIfPreference)
                {
                    Write-Host -ForegroundColor DarkGreen "Adding the following product type:"
                    Write-Host -ForegroundColor DarkGreen $productType
                }
                else
                {
                    Set-UnitType ([ref]$productType)
                    $productType = Add-ProductType -UserCredentials $userCredentials -Configuration $systemConfiguration -InputObject $productType
                }

                Write-Host "Added product type '$($productTypeName)' ($($productType.Id))"
            }

            Write-Progress -Id 2 -ParentId 1 -Activity "Adding product types" -Status "Completed" -Completed
        }

        if (-not($SkipProductTypes))
        {
            Write-Progress -Id 1 -Activity Setup -Status "Setup (2/2)"
        }

        $descriptors = @(Get-Item -Path "$($software)\*.xml")
        if ($descriptors.Length -gt 0) {
            foreach ($i in 1..$descriptors.Length)
            {
                $descriptor = $descriptors[$i - 1]
                $name = [System.IO.Path]::GetFileName($descriptor)
                Write-Progress -Id 3 -ParentId 1 -Activity "Importing software" -Status "Importing '$($name)' ($($i) / $($descriptors.Length))" -PercentComplete ($i / $descriptors.Length * 100)
                Write-Verbose "Importing software '$($descriptor.Name)'"
                Import-SoftwareFolder -UserCredentials $userCredentials -Configuration $systemConfiguration -Path $descriptor
                Write-Host "Imported software '$($descriptor.Name)'"
            }
        }
        Write-Progress -Id 3 -ParentId 1 -Activity "Importing software" -Status "Completed" -Completed

        # Addin default UserRoles
        if ($SkipAddingUserRoles)
        {
            Write-Host "Skipping adding user roles"
        }
        else
        {
            Write-Host "Adding user roles"
            Add-DefaultUserRoles -UserCredentials $userCredentials -Configuration $systemConfiguration
        }

        Write-Progress -Id 1 -Activity Setup -Status "Setup completed" -Completed
    }
}

end
{
}