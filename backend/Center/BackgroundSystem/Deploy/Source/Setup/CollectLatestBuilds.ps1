<#
    .SYNOPSIS
    Fills the folders required by Setup.ps1 with the latest builds relative to this scripts path.

    .DESCRIPTION
    The script copys required files from the current project folder. Existing files will be replaced.

    .PARAMETER SkipBinaries
    If set, binarys will not be collected.

    .PARAMETER SkipCmdlets
    If set, cmdlets will not be collected.

    .PARAMETER SkipSoftware
    If set, software will not be collected.

    .PARAMETER BuildConfiguration
    Choose the build type, default is Release.
#>
[CmdLetBinding()]
param
(
    [Parameter()]
    [switch]
    $SkipBinaries,

    [Parameter()]
    [switch]
    $SkipCmdlets,

    [Parameter()]
    [switch]
    $SkipSoftware,

    [Parameter()]
    [string]
    [ValidateSet("Debug", "Release")]
    $BuildConfiguration = "Release"

)

begin
{
    function Test-FolderValid
    {
        param
        (
            [Parameter(Mandatory = $true)]
            [string]
            $path,
            
            [Parameter(Mandatory = $true)]
            [string]
            $software
        )

        process
        {
            if (-Not (Test-Path $path))
            {
               Write-Host "Source folder for " $software " does not exist. Did you build the project?" -foreground "red"
               $global:hasError = $true
               return $false
            }

            if (-Not (Test-Path $($path + "\*")))
            {
               Write-Host "Source folder for " $software " is empty. Did you build the project?" -foreground "red"
               $global:hasError = $true
               return $false
            }
          
            return $true
        }
    }

    function Get-Binaries
    {
        param
        (
            [Parameter(Mandatory = $true)]
            [string]
            $ReleaseFolderName
        )

        process
        {
            if ($SkipBinaries) {
               Write-Host "Skipped binaries"
               return
            }
            
            Write-Host "[Binaries]" -foreground "green"
            Write-Host "[ SoftwareDescription ]"
            $binaryFolder = "..\..\..\..\Admin\Source\SoftwareDescription\bin\" + $ReleaseFolderName
            Write-Host "Collecting from: " $binaryFolder
            if(Test-FolderValid $binaryFolder "SoftwareDescription")
            {
                Copy-Item $($binaryFolder + "\*.dll") Binaries\
            }

            Write-Host "[ HardwareDescription ]"
            $binaryFolder = "..\..\..\..\..\Common\Configuration\Source\HardwareDescription\bin\" + $ReleaseFolderName
            Write-Host "Collecting from: " $binaryFolder
            if(Test-FolderValid $binaryFolder "HardwareDescription")
            {
                Copy-Item $($binaryFolder + "\*.dll") Binaries\
            }
            Write-Host
        }
    }

    function Get-CmdLets
    {
        param
        (
            [Parameter(Mandatory = $true)]
            [string]
            $ReleaseFolderName
        )

        process
        {
            if ($SkipCmdlets) {
               Write-Host "Skipped cmdlets"
               return
            }
            
            Write-Host "[Cmdlets]" -foreground "green"
            $binaryFolder = "..\..\..\Source\PowerShell\bin\" + $ReleaseFolderName
            Write-Host "Collecting from: " $binaryFolder
            if(Test-FolderValid $binaryFolder "Cmdlets")
            {
                Copy-Item $($binaryFolder + "\*.dll") Cmdlets\
                Copy-Item $($binaryFolder + "\*.psd1") Cmdlets\
            }
            Write-Host
        }
    }

    function Get-SofwareDescriptor
    {
        param
        (
            [Parameter(Mandatory = $true)]
            [string]
            $AssemblyFileName,

            [Parameter(Mandatory = $true)]
            [string]
            $Name,

		
            [Parameter()]
            [bool]
            $Is32BitAssembly,

			[Parameter()]
			[string]
			$PackageId
        )

        process
        {
            $assembly = $null
            if ($Is32BitAssembly)
            { 
                $call = {
                    $assembly = [System.Reflection.Assembly]::LoadFrom($args[0]);
                    return $assembly.GetName().Name, $assembly.GetName().Version
                }
                $assemblyInfo = start-job $call -RunAs32 -Arg $AssemblyFileName | wait-job | Receive-Job
            }
            else
            {
                $assembly = [System.Reflection.Assembly]::LoadFrom($AssemblyFileName);
                $assemblyInfo = $assembly.GetName().Name, $assembly.GetName().Version
            }

            if (!$assemblyInfo) 
            {
                exit
            }

            $descriptor = New-Object Gorba.Center.Admin.SoftwareDescription.SoftwarePackageDescriptor
			if ($PackageId)
			{
				$descriptor.PackageId = $PackageId
			}
			else
			{
				$descriptor.PackageId = $assemblyInfo[0]
			}
            
            $descriptor.Version.VersionNumber = $assemblyInfo[1]
            $descriptor.Version.Description = $("Loaded automatically " + (get-date).ToString('d.M.yyyy')) 
            $descriptor.Name = $Name

            return $descriptor
        }
    }

     function Create-SoftwareDescriptionXml
    {
        param
        (
            [Parameter(Mandatory = $true)]
            [string]
            $FileName,

            [Parameter(Mandatory = $true)]
            [Gorba.Center.Admin.SoftwareDescription.SoftwarePackageDescriptor]
            $descriptor
        )

        process
        {
            $desciptorSerializer = New-Object System.Xml.Serialization.XmlSerializer([Gorba.Center.Admin.SoftwareDescription.SoftwarePackageDescriptor])

            $Encoding = New-Object System.Text.UTF8Encoding
            $xmlWriter = New-Object System.Xml.XmlTextWriter($FileName, $Encoding);
            $xmlWriter.Formatting = [System.Xml.Formatting]::Indented

            $desciptorSerializer.Serialize($xmlWriter, $descriptor)
            $xmlWriter.Close();
        }
    }

    function Create-SoftwarePackage
    {
        param
        (
            [Parameter(Mandatory = $true)]
            [string]
            $Name,

            [Parameter(Mandatory = $true)]
            [string]
            $HumanreadableName,

            [Parameter(Mandatory = $true)]
            [string]
            $BinaryPath,

            [Parameter(Mandatory = $true)]
            [string]
            $ReleaseFolderName,

            [Parameter(Mandatory = $true)]
            [string]
            $AssemblyName,

            [Parameter(Mandatory = $true)]
            [string[]]
            $CopyItems,

            [Parameter()]
            [bool]
            $Is32BitAssembly,

			[Parameter()]
			[string]
			$PackageId
        )
        process
        {
            $motionFolder = $PSScriptRoot + "..\..\..\..\..\..\Motion\"
           
            Write-Host "["$HumanreadableName "]"
            $binaryFolder = $motionFolder + $BinaryPath + $ReleaseFolderName
            Write-Host "Source folder: " $binaryFolder
            if (!$(Test-FolderValid $binaryFolder $HumanreadableName))
            {
                return
            }

            $FullAssemblyName = $binaryFolder + $AssemblyName
            $SoftwarePackageDescriptor = Get-SofwareDescriptor $FullAssemblyName $HumanreadableName $Is32BitAssembly $PackageId

            $xmlFilename = $PSScriptRoot + "\Software\" + $Name + "-" + $SoftwarePackageDescriptor.Version.VersionNumber +".xml"
            Create-SoftwareDescriptionXml $xmlFilename $SoftwarePackageDescriptor
 
            # copy files
            $targetfolder = "Software\" + $Name + "-" + $SoftwarePackageDescriptor.Version.VersionNumber + "\Progs\"+$Name+"\"
            New-Item -ItemType Directory -Force -Path $targetfolder | Out-Null

            foreach($item in $CopyItems) {
                Copy-Item $($binaryFolder + "\" +$item) $targetfolder -Recurse -Force
            }
        }
    }

    function Get-Software
    {
        param
        (
            [Parameter(Mandatory = $true)]
            [string]
            $ReleaseFolderName
        )

        process
        {
            if ($SkipSoftware) {
               Write-Host "Skipped software"
               return
            }

            #load to get acces to SoftwarePackageDescriptor type
            $Assembly = [System.Reflection.Assembly]::LoadFrom($PSScriptRoot + "\Binaries\Gorba.Center.Admin.SoftwareDescription.dll");

            Write-Host "[Software]" -foreground "green"

            # AhdlcRenderer
            $CopyItems = ("*.dll", "*.exe")
            Create-SoftwarePackage "AhdlcRenderer" "AHDLC Renderer" "Infomedia\Source\AhdlcRendererApp\bin\" $ReleaseFolderName "\Gorba.Motion.Infomedia.AhdlcRenderer.dll" $CopyItems -PackageId "Gorba.Motion.Infomedia.AhdlcRenderer"

             # AudioRenderer
            $CopyItems = ("*.dll", "*.exe")
            Create-SoftwarePackage "AudioRenderer" "Audio Renderer" "Infomedia\Source\AudioRendererApp\bin\" $ReleaseFolderName "\Gorba.Motion.Infomedia.AudioRenderer.dll" $CopyItems $true -PackageId "Gorba.Motion.Infomedia.AudioRenderer"

            # Composer
            $CopyItems = ("*.dll", "*.exe")
            Create-SoftwarePackage "Composer" "Composer" "Infomedia\Source\ComposerApp\bin\" $ReleaseFolderName "\Composer.exe" $CopyItems $true -PackageId "Gorba.Motion.Infomedia.Composer"

            # DirectXRenderer
            $CopyItems = ("*.dll", "*.exe")
            Create-SoftwarePackage "DirectXRenderer" "DirectX Renderer" "Infomedia\Source\DirectXRendererApp\bin\" $ReleaseFolderName "\Gorba.Motion.Infomedia.DirectXRenderer.dll" $CopyItems $true -PackageId "Gorba.Motion.Infomedia.DirectXRenderer"

             # HardwareManager
            $CopyItems = ("*.dll", "*.exe")
            Create-SoftwarePackage "HardwareManager" "Hardware Manager" "HardwareManager\Source\ConsoleApp\bin\" $ReleaseFolderName "\Gorba.Motion.HardwareManager.Core.dll" $CopyItems -PackageId "Gorba.Motion.HardwareManager"

             # Protran
            $CopyItems = ("*.dll", "*.exe")
            Create-SoftwarePackage "Protran" "Protran" "Protran\Source\ConsoleApp\bin\" $ReleaseFolderName "\Gorba.Motion.Protran.Core.dll" $CopyItems -PackageId "Gorba.Motion.Protran"

             # SystemManager
            $CopyItems = ("*.dll", "*.exe")
            Create-SoftwarePackage "SystemManager" "System Manager" "SystemManager\Source\ConsoleApp\bin\" $ReleaseFolderName "\Gorba.Common.SystemManagement.Core.dll" $CopyItems -PackageId "Gorba.Motion.SystemManager"
             
            # Update
            $CopyItems = ("*.dll", "*.exe")
            Create-SoftwarePackage "Update" "Update" "Update\Source\ConsoleApp\bin\" $ReleaseFolderName "\Gorba.Motion.Update.Core.dll" $CopyItems $true -PackageId "Gorba.Motion.Update"

            # Bus
            $CopyItems = ("*.dll", "*.exe")
            Create-SoftwarePackage "Bus" "Bus module" "Obc\Source\Bus\ConsoleApp\bin\x86\" $ReleaseFolderName "\Gorba.Motion.Obc.Bus.Core.dll" $CopyItems $true -PackageId "Gorba.Motion.Obc.Bus"

            # Terminal
            $CopyItems = ("*.dll", "*.exe")
            Create-SoftwarePackage "Terminal" "Terminal" "Obc\Source\Terminal\GuiApplication\bin\x86\" $ReleaseFolderName "\Gorba.Motion.Obc.Terminal.Core.dll" $CopyItems $true -PackageId "Gorba.Motion.Obc.Terminal"

            # IbisControl
            $CopyItems = ("*.dll", "*.exe")
            Create-SoftwarePackage "IbisControl" "Ibis Control" "Obc\Source\Ibis\ConsoleApp\bin\" $ReleaseFolderName "\Gorba.Motion.Obc.Ibis.Core.dll" $CopyItems -PackageId "Gorba.Motion.Obc.IbisControl"
        }
    }
}

process
{
    Write-Host "Collecting files:"
    
    $global:hasError = $false

    Get-Binaries $BuildConfiguration
    
    Get-CmdLets $BuildConfiguration

    Get-Software $BuildConfiguration


    if ($global:hasError) 
    {
        Write-Host "There was an error, check the output." -ForegroundColor "red"
    }
}

end
{
}