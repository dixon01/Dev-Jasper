function Publish-CenterProduct
{
    <#
        .SYNOPSIS
        Publishes the assemblies from the source to the destination and optionally copies configuration files.

        .DESCRIPTION
        Copies all files (except .pdb and .config files, directories named logs and CodeContracts) from the Source to the Destination.
        If the path to an Extra folder is provided, the entire content of the folder is copied to the destination as LAST operation.
        Copies are recursive and forced.

        .PARAMETER Source
        The source directory

        .PARAMETER Destination
        The destination directory

        .PARAMETER Extra
        The directory with the extra files to be copied. A NULL value will be ignored.
    #>
    param
    (
        [Parameter(Mandatory = $true, Position = 1)] [string] $Source,
        [Parameter(Mandatory = $true, Position = 2)] [string] $Destination,
        [Parameter(Mandatory = $false, Position = 3)] [string] $Extra = $null,
        [switch] $ClearDestination
    )

    begin
    {
        if (-not(Test-Path $Source))
        {
            throw "The specified source path '$($Source)' doesn't exist"
        }

        if (-not(Test-Path $Destination))
        {
            throw "The specified destination path '$($Destination)' doesn't exist"
        }

        if($Extra -and -not(Test-Path $Extra))
        {
            throw "The specified Extra path '$($Extra)' doesn't exit"
        }
    }

    process
    {
        if($ClearDestination)
        {
            ls "$($Destination)\*" | rm -Recurse -Force
        }

        $tempDir = New-TempDir

        cp "$($Source)\*" $tempDir -Force -Recurse -Exclude "*.pdb", "*.config", "CodeContracts", "logs"
        if($Extra)
        {
            Write-Host "Copying Extra from '$($Extra) to '$($tempDir)'"
            cp "$($Extra)\*" $tempDir -Force -Recurse
        }
        else
        {
            Write-Host "Extra directory not specified"
        }

        mv "$($tempDir)\*" $Destination
    }
}

function Get-LastSuccessfulBuild
{
    <#
        .SYNOPSIS
        Returns the details of the last successful build for the specified definition        

        .DESCRIPTION
        Gets the details of the last successful build for the specified definition.
        It uses Microsoft.TeamFoundation.* assemblies that must be loadable only with partial name

        .PARAMETER BuildDefinition
        Name of the build definition

        .PARAMETER Tfs
        Address of the tfs collection services

        .PARAMETER Project
        Name of the project in the collection
    #>
    param
    (
        [Parameter(Mandatory = $true, Position = 1)] [string] $BuildDefinition,
        [Parameter(Mandatory = $false, Position = 2)] [System.Uri] $Tfs = "https://tfsgorba.gorba.com:8443/tfs/DefaultCollection",
        [string] $Project = "Gorba"
    )

    process
    {
        try
        {
            [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.TeamFoundation.Client")   | Out-Null
            [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.TeamFoundation.Build.Client") | Out-Null
            [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.TeamFoundation.Build.Common") | Out-Null
        }
        catch
        {
            throw "Can't load TFS assemblies from GAC. Please install them. Full Error: $($_.Exception)"
        }
   
         $server = $server = [Microsoft.TeamFoundation.Client.TeamFoundationServerFactory]::GetServer($Tfs)
         $buildService = $server.GetService([Microsoft.TeamFoundation.Build.Client.IBuildServer])
         if(-not($buildService))
         {
            throw "Build server not found"
         }

         $buildDetail = $buildService.QueryBuilds($Project, $BuildDefinition) | where { $_.BuildDefinition.LastGoodBuildUri -eq $_.Uri }
         if (-not($buildDetail))
         {
            throw "Successful build not found for definition '$($BuildDefinition)'"
         }

         Write-Output $buildDetail
    }
}

function Publish-LastCenterProductBuild
{
    <#
        .SYNOPSIS
        Publishes a CenterProduct taking the output of last successful build from TFS

        .DESCRIPTION
        This command gets the last version of a center product on TFS and copies it to a destination directory, optionally copying at the end the content
        of an Extra folder

        .PARAMETER BuildDefinition
        The name of the build definition which produces the output

        .PARAMETER Destination
        The target path where the build output should be copied

        .PARAMETER Extra
        Optional folder containing files and directories that should be copied to the destination AFTER the build output (everything will be overridden)

        .PARAMETER RelativePath
        Useful when the output to be taken from the Build is under a specific path

        .PARAMETER ClearDestination
        If set, files and folders are removed from the destination before copying the new output

        .PARAMETER Tfs
        Address of the TFS collection services

        .PARAMETER Project
        Name of the project in the TFS collection
    #>
    param
    (
        [Parameter(Mandatory = $true)] [string] $BuildDefinition,
        [Parameter(Mandatory = $true)] [string] $Destination,
        [Parameter(Mandatory = $false)] [string] $Extra,
        [Parameter(Mandatory = $false)] [string] $RelativePath = $null,
        [switch] $ClearDestination,
        [Parameter(Mandatory = $false)] [System.Uri] $Tfs = "https://tfsgorba.gorba.com:8443/tfs/DefaultCollection",
        [string] $Project = "Gorba"
    )

    process
    {
        $buildDetail = Get-LastSuccessfulBuild -Tfs $Tfs -BuildDefinition $BuildDefinition -Project $Project
        if ($RelativePath)
        {
            $path = Join-Path $buildDetail.DropLocation $RelativePath
        }
        else
        {
            $path = $buildDetail.DropLocation
        }

        Write-Host "Copying from $($path) to $($Destination)"
        Publish-CenterProduct -Source $path -Destination $Destination -Extra $Extra -ClearDestination
    }
}

function Publish-ItcsClients
{
    <#
        .SYNOPSIS
        Publishes the specified clients to the Products directory

        .DESCRIPTION
        This command copies the output of the Center_ItcsClient build to the specified products directory.
        For each client, it searches for a corresponding directory in the ItcsClientProductDeployment for Extra content

        .PARAMETER ProductsRoot
        Root of the folder containint Center products

        .PARAMETER ItcsClients
        List of names of the Itcs clients to be deployed

        .PARAMETER ItcsClientProductDeployment
        Optional folder containing deployment extra directories for specified Itcs clients

        .PARAMETER Release
        Optional version of the release to be considered for the build definition
    #>
    param
    (
        [Parameter(Mandatory = $true)] [string] $ProductsRoot,
        [Parameter(Mandatory = $true)] [string[]] $ItcsClients,
        [Parameter(Mandatory = $false)] [string] $ItcsClientProductDeployment,
        [Parameter(Mandatory = $false)] [string] $Release
    )

    begin
    {
        $ItcsClientsRoot = Join-Path $ProductsRoot "ItcsClient"
        $BuildDefinition = "Center_ItcsClient"
        if ($Release)
        {
            $BuildDefinition += "_$($Release)"
        }
    }

    process
    {

        foreach($ItcsClient in $ItcsClients)
        {
            Write-Host "Processing Itcs client '$($ItcsClient)'"
            if ($ItcsClientProductDeployment)
            {
                $ItcsClientExtra = Join-Path $ItcsClientProductDeployment $ItcsClient
                if(-not(Test-Path $ItcsClientExtra))
                {
                    Write-Warning "ItcsClient deployment folder '$($ItcsClientExtra)' not found. No extra folder will be specified"
                    $ItcsClientExtra = $null
                }
            }
            else
            {
                Write-Warning "ItcsClient deployment not specified. No extra folder will be specified"
                $ItcsClientExtra = $null
            }

            $ItcsClientDestination = Join-Path $ItcsClientsRoot $ItcsClient
            if(-not(Test-Path $ItcsClientDestination))
            {
                New-Item $ItcsClientDestination -ItemType Directory | Out-Null
            }

            Write-Host "Publishing '$($ItcsClient)' to destination '$($ItcsClientDestination)' using build definition '$($BuildDefinition)'. This could require some time"
            Publish-LastCenterProductBuild -BuildDefinition $BuildDefinition -Destination $ItcsClientDestination -Extra $ItcsClientExtra -ClearDestination
        }
    }
}

function Publish-CommS
{
    <#
        .SYNOPSIS
        Deploys the last version of Comm.S

        .DESCRIPTION
        Copies the output of the CommsApp from the last successful Center_CommS build to the specified Products target directory.
        It is possible to specify a directory for Extra content that will be copied over the output and a release version.
        WARNING: content of the target directory is completely cleared

        PARAMETER .ProductsRoot
        Root directory for target Products

        PARAMETER .CommSExtra
        Optional directory for extra content to be copied to the target after the build output. It will override existing files and directories

        PARAMETER .Release
        Optional version of a release (used for the build definition)
    #>
    param
    (
        [Parameter(Mandatory = $true, Position = 1)] [string] $ProductsRoot,
        [Parameter(Mandatory = $false, Position = 2)] [string] $CommSExtra,
        [Parameter(Mandatory = $false, Position = 3)] [string] $Release
    )

    begin
    {
        $CommSDestination = Join-Path $ProductsRoot "Comm.S"

        $BuildDefinition = "Center_CommS"
        if ($Release)
        {
            $BuildDefinition += "_$($Release)"
        }

        if(-not(Test-Path $CommSDestination))
        {
            Write-Host "Destination path for Comm.S '$($CommSDestination)' doesn't exist. Trying to creat it"
            New-Item $CommSDestination -ItemType Directory | Out-Null
        }
    }

    process
    {
        Write-Host "Publishing Comm.S to destination '$($CommSDestination)' using output of build definition '$($BuildDefinition)'. This could require some time"
        
        Write-Host "CommSDestination: $($CommSDestination)"
        Write-Host "CommSExtra: $($CommSExtra)"
        rm "$($CommSDestination)\*" -Recurse -Force
        Publish-LastCenterProductBuild -BuildDefinition $BuildDefinition -Destination $CommSDestination -RelativePath "Gorba.Center.CommS\CommsApp\Release" -Extra $CommSExtra -ClearDestination
    }
}

function New-TempDir
{
    <#
        .SYNOPSIS
        Creates a new temp dir

        .DESCRIPTION
        Creates a new temp dir and returns the path
    #>
    process
    {
        $tmpDir = [System.IO.Path]::GetTempPath()
        $tmpDir = [System.IO.Path]::Combine($tmpDir, [System.IO.Path]::GetRandomFileName())
  
        [System.IO.Directory]::CreateDirectory($tmpDir) | Out-Null

        Write-Output $tmpDir
    }
}

Export-ModuleMember -Function "Publish-CenterProduct", "Get-LastSuccessfulBuild", "Publish-LastCenterProductBuild", "Publish-ItcsClients", "Publish-CommS"