function Read-UserChoice
{
	param
	(
		[Parameter(Mandatory = $true)]
		[Object[]]
		$Configurations
	)

	process
	{
		$title = "Select deployment"
		$message = "Please select the desired environment"

		$options = @()
		$i = 0
		foreach ($configuration in $Configurations)
		{
			$description = Split-Path -Leaf $configuration
			$option = "&$($i) - $($description)"
			$options += New-Object System.Management.Automation.Host.ChoiceDescription($option, $configuration)
			$i++
		}

		$result = $host.ui.PromptForChoice($title, $message, $options, -1)
		return $configurations[$result]
	}
}

function Read-DeploymentConfiguration
{
	param
	(
		[Parameter(Mandatory = $true)]
		[System.IO.DirectoryInfo]
		$Path,

		[Parameter()]
		[string]
		$PackageLocation,

		[Parameter(Mandatory = $true)]
		[string]
		[ValidateSet( "Debug", "Release")]
		$BuildConfiguration
	)

	process
	{
		Write-Host $Path
		$configPath = Join-Path $Path "DeploymentConfiguration.xml"
		if (-not(Test-Path $configPath))
		{
			throw "Can't find DeploymentConfiguration.xml for system '$($configPath)'"
		}
			
		[Gorba.PowerShell.Azure.DeploymentConfiguration] $config = [Gorba.PowerShell.Azure.DeploymentConfiguration]::Load($configPath)
		
		if ($PackageLocation)
		{
			if (-not(Test-Path $PackageLocation))
			{
				throw "Specified package location '$($PackageLocation)' not found"
			}

			Write-Host "Package location specified as input parameter"
		}
		else
		{
			$resolveError = $null
			Write-Host "Package location path not passed as parameter. Config location: $($config.PackageLocation)"
			if ((-not ($config.PackageLocation)))
			{
				Write-Verbose "Package location not yet valid. Trying to find the locally built package"
				$localBuildPath = Join-Path $PSScriptRoot "..\..\..\..\..\Source\AzureCloudService\bin\$($BuildConfiguration)\app.publish\BackgroundSystem.AzureCloudService.cspkg"
				$PackageLocation = Resolve-Path $localBuildPath -ErrorAction Continue -ErrorVariable resolveError
			}
			else
			{
				if (Test-Path $config.PackageLocation)
				{
					$PackageLocation = Resolve-Path $config.PackageLocation
				}
				else
				{
					Write-Verbose "Package location not specified. Trying to resolve it as relative to '$($Path)'"
					$PackageLocation = Join-Path $Path $config.PackageLocation
					$PackageLocation = Resolve-Path $PackageLocation -ErrorAction Continue -ErrorVariable resolveError
				}
			}

			if ($resolveError)
			{
				throw "Can't find a valid path"
			}
		}

		$fileName = Split-Path $PackageLocation -Leaf
		if (!$fileName.EndsWith(".cspkg"))
		{
			Write-Warning "The package location '$($PackageLocation)' doesn't seem to be a valid cspkg file"
		}

		$config.PackageLocation = $PackageLocation
		return $config
	}
}

function Initialize-DeploymentConfiguration
{
	process
	{
		Add-Type -TypeDefinition @"
namespace Gorba.PowerShell.Azure
{
	using System;
	using System.Collections.Generic;
	using System.Xml;

	public class DeploymentConfigurationResult
	{
		public DeploymentConfigurationResult(string name, string path, DeploymentConfiguration configuration)
		{
			this.Name = name;
			this.Path = path;
			this.Configuration = configuration;
		}

		public DeploymentConfiguration Configuration { get; private set; }

		public string Name { get; private set; }

		public string Path { get; private set; }
	}
	
	[System.Xml.Serialization.XmlRoot("DeploymentConfiguration")]
	public class DeploymentConfiguration
	{
		public string Description { get; set; }

		public string Slot { get; set; }

		public string ServiceName { get; set; }

		public string StorageAccountName { get; set; }

		public string StorageAccountKey { get; set; }

		public string SubscriptionName { get; set; }

		public string DeploymentLabelFormat { get; set; }

		public string PackageLocation { get; set; }

		public string ServiceDefinitionPath { get; set; }

		public string WorkerRole { get; set; }

		public string HttpsCertificatePath { get; set; }

		public string HttpsCertificatePassword { get; set; }
		
		public static DeploymentConfiguration Load(string path)
		{
			using (var file = System.IO.File.OpenRead(path))
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(typeof(DeploymentConfiguration));
				return (DeploymentConfiguration)serializer.Deserialize(file);
			}
		}

		public void Save(string path)
		{
			using (var writer = System.IO.File.OpenWrite(path))
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(typeof(DeploymentConfiguration));
				serializer.Serialize(writer, this);
			}
		}
	}
}
"@ -ReferencedAssemblies "System.Xml" | Out-Null
	}
}

function Get-DeploymentConfiguration
{
	[CmdletBinding()]
	param
	(
		[Parameter()]
		[string]
		$DeploymentConfigurationName,

		[Parameter()]
		[string]
		$PackageLocation,

		[Parameter()]
		[string]
		[ValidateSet( "Debug", "Release" )]
		$BuildConfiguration = "Release"
	)

	begin
	{
	}

	process
	{
		$configsDir = Join-Path $PSScriptRoot "..\Configurations"
		if (-not($configsDir))
		{
			throw "Can't find configurations directory '$($configsDir)'"
		}

		$configs = Get-ChildItem $configsDir -Directory | ?{ (Test-Path (Join-Path $_.FullName `
			"DeploymentConfiguration.xml")) -and (Test-Path (Join-Path $_.FullName `
			"ServiceConfiguration.$($_.Name).cscfg")) -and `
			(!$DeploymentConfigurationName -or $_.Name -eq $DeploymentConfigurationName) } `
			| Select-Object -ExpandProperty FullName
		if ($DeploymentConfigurationName)
		{
			$configPath = Join-Path $configsDir $DeploymentConfigurationName
			if (-not(Test-Path $configPath))
			{
				throw "Can't find the specified configuration '$($DeploymentConfigurationName)' in folder '$($configsDir)'"
			}
		}
		else
		{
			if ($configs.Length -gt 1)
			{
				$configPath = Read-UserChoice $configs
			}
			elseif ($configs.Length -eq 1)
			{
				$configPath = $configs[0]
			}
			else
			{
				throw "Can't find any valid configuration"
			}

			$DeploymentConfigurationName = Split-Path -Leaf $configPath
		}

		Write-Verbose "The user selected the deployment configuration $($configPath)"
		$DeploymentConfiguration = Read-DeploymentConfiguration -Path $configPath -BuildConfiguration $BuildConfiguration -PackageLocation $PackageLocation
		Write-Host $configPath
		New-Object Gorba.PowerShell.Azure.DeploymentConfigurationResult($DeploymentConfigurationName, $configPath, $DeploymentConfiguration)
	}

	end
	{
	}
}

function Get-DeploymentConfigurationXml
{
	param
	(
		[Parameter(Mandatory = $true)]
		[string]
		$Path
	)

	process
	{
		[xml] $definition = Get-Content $Path -ErrorAction Stop
		return $definition
	}
}

function New-ServiceCertificate
{
	<#
		.SYNOPSIS
		Creates a new certificate to use with a service.

		.REMARKS
		Adapted from a script by Freist Li
	#>

	param
	(
		[Parameter(Mandatory = $true)]
		[string]
		$CertificateCn,

		[Parameter(Mandatory = $true)]
		[string]
		$Password,

		[Parameter(Mandatory = $true)]
		[string]
		$CertificateFilePath,

        [Parameter()]
        [switch]
        $NoTfs
	)

	process
	{

		#Check if the certificate name was used before
		$securePassword = ConvertTo-SecureString -String $Password -Force –AsPlainText 

        [string] $thumbprint = New-SelfSignedCertificate -DnsName $CertificateCn -CertStoreLocation cert:\CurrentUser\My | Select-Object -ExpandProperty Thumbprint

		if ($thumbprint) 
		{ 
			# query the new installed cerificate again
			$storedCertificate = ls cert:\currentuser\My -Recurse | ?{ ($_.Subject -match "CN=$($CertificateCn)")  -and ($_.Thumbprint -match $thumbprint) } | Select-Object -Last 1 -ExpandProperty Thumbprint
			#If new cert installed sucessfully with the same thumbprint 

			if($storedCertificate)
			{
				Write-Host "$($CertificateCn) installed into CurrentUser\My successfully with thumprint '$($thumbprint)'"
				Write-Host "Exporting Certificate as .pfx file"
				Export-PfxCertificate -FilePath $CertificateFilePath -Cert "cert:\currentuser\My\$($thumbprint)" -Password $securePassword | Out-Null
                if (-not($NoTfs))
                {
                    try
                    {
                        $oldPreference = $ErrorActionPreference
                        $ErrorActionPreference = 'SilentlyContinue'
                        Write-Host "Trying to add the certificate path '$($certificateFilePath)' to TFS"
                        tf add $CertificateFilePath | Out-Null
                        if ($LASTEXITCODE -ne 0)
                        {
                            Write-Warning "Couldn't add the certificate to TFS"
                        }
                    }
                    catch
                    {
                        Write-Warning "Couldn't Add the certificate to TFS. Error message: '$($_.Exception.Message)'"
                    }
                    finally
                    {
                        $ErrorActionPreference = $oldPreference
                    }
                }

                Write-Host "Certificate created with thumbprint '$($thumbprint)'"
				return $thumbprint
			}
 
			throw "Thumbprint is not the same between new cert and installed cert."
		}

		throw "$($CertificateCn) was not created"
	}
}

function Update-CertificateThumbprint
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]
        $Path,

        [Parameter(Mandatory = $true)]
        [string]
        $WorkerRole,

        [Parameter(Mandatory = $true)]
        [string]
        $Thumbprint,

        [Parameter()]
        [switch]
        $NoTfs
    )

    process
    {
        Write-Host "Updating certificate thumbprint"
		[xml] $config = Get-DeploymentConfigurationXml $Path
		$httpsCertificate = Select-DeploymentConfigurationXmlNode -Content $config -Path "/ns:ServiceConfiguration[@serviceName='AzureCloudService']/ns:Role[@name='$($WorkerRole)']/ns:Certificates/ns:Certificate[@name='Https']"
		$httpsCertificate.Attributes["thumbprint"].Value = $Thumbprint
		$readOnly = (Get-ItemProperty -Path $Path -Name IsReadOnly).IsReadOnly
		if ($readOnly)
		{
            if ($NoTfs)
            {
                throw "The file is readonly. Can't save it, the deployment would fail."
            }
            else
            {
                # the file is readonly. Trying to check it out from TFS
				tf checkout $Path
				if ($LASTEXITCODE -ne 0)
				{
					throw "Error while checking out the configuration file. TFS tools must be installed"
				}

				$config.Save($Path)
            }
		}
        else
        {
			$config.Save($Path)
        }
    }
}

function Publish-HttpsCertificate
{
	<#
		.SYNOPSIS
		Publishes the Https certificate.
	#>
	param
	(
		[Parameter(Mandatory = $true)]
		[string]
		$Path,

		[Parameter(Mandatory = $true)]
		[string]
		$WorkerRole,

		[Parameter(Mandatory = $true)]
		$DeploymentConfiguration,

        [Parameter()]
        [switch]
        $NoTfs
	)

	process
	{
		[xml] $config = Get-DeploymentConfigurationXml $Path
		try
		{
			$httpsCertificate = Select-DeploymentConfigurationXmlNode -Content $config -Path "/ns:ServiceConfiguration[@serviceName='AzureCloudService']/ns:Role[@name='$($WorkerRole)']/ns:Certificates/ns:Certificate[@name='Https']"

			$certificateName = "{0}.cloudapp.net" -f $DeploymentConfiguration.ServiceName

            # setting the path to the default location (same folder as the configuration)
			$certificatePath = Join-Path (Split-Path $Path -Parent) "$($certificateName).Https.pfx"

			if ($DeploymentConfiguration.HttpsCertificatePath)
			{
                # the certificate is specified in the cscfg file
                $certificatePath = $DeploymentConfiguration.HttpsCertificatePath
				if (-not(Test-Path $certificatePath))
				{
                    # the path was not found. Trying to resolve it as relative to the cscfg file location
                    $certificatePath = Resolve-Path (Join-Path (Split-Path $Path -Parent) $certificatePath) -ErrorAction SilentlyContinue
				}

                if (-not(Test-Path $certificatePath))
                {
                    throw "The certificate specified in the cscfg file was not found"
                }
			}

			if ($DeploymentConfiguration.HttpsCertificatePassword)
			{
				$certificatePassword = $DeploymentConfiguration.HttpsCertificatePassword				
			}
			else
			{
				$certificatePassword = "Center.Portal.Https"
			}

			if (-not($httpsCertificate.thumbprint))
			{
                if (Test-Path $certificatePath)
                {
                    Write-Host "Using the certificate found at '$($certificatePath)'. Please enter the password when requested"
                    $certificateFromPath = Get-PfxCertificate -FilePath $certificatePath -ErrorAction Stop
                    $thumbprint = $certificateFromPath.Thumbprint
                }
                else
                {
				    Write-Host "Generating the certificate"
				    $thumbprint = New-ServiceCertificate -CertificateCn $certificateName -Password $certificatePassword -CertificateFilePath $certificatePath -NoTfs:$NoTfs
                }

                Write-Host "Thumbprint type: $($thumbprint.GetType().FullName)"
                Update-CertificateThumbprint -Path $Path -WorkerRole $WorkerRole -Thumbprint $thumbprint -NoTfs:$NoTfs
                <#
				$httpsCertificate.Attributes["thumbprint"].Value = $thumbprint
				$readOnly = (Get-ItemProperty -Path $Path -Name IsReadOnly).IsReadOnly
				if ($readOnly)
				{
                    if ($NoTfs)
                    {
                        throw "The file is readonly. Can't save it, the deployment would fail"
                    }
                    else
                    {
                        # the file is readonly. Trying to check it out from TFS
					    tf checkout $Path
					    if ($LASTEXITCODE -ne 0)
					    {
						    Write-Error "Error while checking out the configuration file. TFS tools must be installed"
						    return $false
					    }

				        $config.Save($Path)
                    }
				}
                else
                {
				    $config.Save($Path)
                }
                #>

				Add-AzureCertificate -ServiceName $DeploymentConfiguration.ServiceName -CertToDeploy $certificatePath -Password $certificatePassword -ErrorAction Stop | Out-Null
				return $true
			}
			
            if (-not(Test-Path $certificatePath))
			{
				Write-Error ("Certificate not found at predefined location {0}" -f $certificatePath)
			}

			$certificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
			$certificate.Import($certificatePath, $certificatePassword, [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]"DefaultKeySet")
			if ($certificate.Thumbprint -ne $httpsCertificate.thumbprint)
			{
				Write-Error "The existing certificate file '$($certificatePath)' has a thumbprint ($($certificate.Thumbprint) that doesn't match the specified one ($($httpsCertificate.thumbprint))"
				return $false
			}

			Write-Host "Using valid existing certificate with thumbprint '$($certificate.Thumbprint)' located at '$($certificatePath)'. Publishing it"

			Add-AzureCertificate -ServiceName $DeploymentConfiguration.ServiceName -CertToDeploy $certificatePath -Password $certificatePassword -ErrorAction Stop | Out-Null
			Write-Host "Certificate published"
		}
		catch
		{
			Write-Error "It was not possible to publish the certificate. Exception message: '$($_.Exception.Message)'"
			return $false
		}

		return $true
	}
}

function Test-HttpsCertificateRequired
{
	<#
		.SYNOPSIS
		Tests the Https certificate if needed.

		.DESCRIPTION
		Tests the presence of the Https certificate if it is enabled in the configuration.
	#>
	param
	(
		[Parameter(Mandatory = $true)]
		[string]
		$Path,

		[Parameter(Mandatory = $true)]
		[string]
		$WorkerRole,

		[Parameter(Mandatory = $true)]
		$DeploymentConfiguration
	)

	process
	{
		[xml] $config = Get-DeploymentConfigurationXml $Path
		$httpsCertificate = Select-DeploymentConfigurationXmlNode -Content $config -Path "/ns:ServiceConfiguration[@serviceName='AzureCloudService']/ns:Role[@name='$($WorkerRole)']/ns:Certificates/ns:Certificate[@name='Https']"

		if ($httpsCertificate)
		{
			return $true
		}

		Write-Debug "No certificate defined in the configuration"
		[bool] $enableHttps = Select-DeploymentConfigurationXmlValue -Content $config -Path "/ns:ServiceConfiguration[@serviceName='AzureCloudService']/ns:Role[@name='$($WorkerRole)']/ns:ConfigurationSettings/ns:Setting[@name='Portal.EnableHttps']"
		if ($enableHttps)
        {
            throw "Invalid configuration. 'Portal.EnableHttps' is set to true but the Https certificate is not specified in the configuration"
        }

        return $false
	}
}

function Test-HttpsCertificate
{
	<#
		.SYNOPSIS
		Tests the Https certificate if needed.

		.DESCRIPTION
		Tests the presence of the Https certificate if it is enabled in the configuration.
	#>
	param
	(
		[Parameter(Mandatory = $true)]
		[string]
		$Path,

		[Parameter(Mandatory = $true)]
		[string]
		$WorkerRole,

		[Parameter(Mandatory = $true)]
		$DeploymentConfiguration,

        [Parameter()]
        [switch]
        $NoTfs
	)

	process
	{
		[xml] $config = Get-DeploymentConfigurationXml $Path
		$httpsCertificate = Select-DeploymentConfigurationXmlNode -Content $config -Path "/ns:ServiceConfiguration[@serviceName='AzureCloudService']/ns:Role[@name='$($WorkerRole)']/ns:Certificates/ns:Certificate[@name='Https']"

		if (-not($httpsCertificate))
		{
		    throw "No certificate defined in the configuration. The Test-HttpsCertificate must be used only with a valid configuration where the certificate entry is vailable in the service configuration file (.cscfg)"
        }

        if ($DeploymentConfiguration.HttpsCertificatePath)
        {
            Write-Host "Getting the certificate from '$($certificateFromPath)'. "
            $certificateFromPath = Get-PfxCertificate -FilePath $DeploymentConfiguration.HttpsCertificatePath
            $thumbprintFromPath = $certificateFromPath.Thumbprint
            Update-CertificateThumbprint -Path $Path -WorkerRole $WorkerRole -Thumbprint $thumbprintFromPath -NoTfs:$NoTfs
        }
        else
        {
            $thumbprintFromPath = $null
        }

		if ($httpsCertificate.thumbprint)
		{
            if ($thumbprintFromPath)
            {
                if ($thumbprintFromPath -ne $httpsCertificate.thumbprint)
                {
                    throw "The thumbprint specified in the cscfg file ('$($httpsCertificate)') doesn't match the one in the file ('$($thumbprintFromPath)'). Leave the value in csfg empty or make sure that they match."
                }
            }

			$certificate = Get-AzureCertificate -ServiceName $DeploymentConfiguration.ServiceName -ThumbprintAlgorithm $httpsCertificate.thumbprintAlgorithm -Thumbprint $httpsCertificate.thumbprint -ErrorAction SilentlyContinue -ErrorVariable certificateError
			if (-not($certificateError))
			{
				Write-Host ("Https certificate '{0}' already uploaded" -f $httpsCertificate.thumbprint)
				return $true
			}

			Write-Warning "Certificate with thumbprint '$($httpsCertificate.thumbprint) not found on the cloud service. It needs to be uploaded"
			return $false
		}

		Write-Warning "Certificate thumbrprint not specified. It must be generated"
		return $false
	}
}

function Test-AzureAccount
{
	<#
		.SYNOPSIS
		Verifies that the Azure account is available in the current PowerShell session.
	#>
	process
	{
		$account = Get-AzureAccount
	}
}

function Select-DeploymentConfigurationXmlNode
{
	param
	(
		[Parameter(Mandatory = $true)]
		[xml]
		$Content,

		[Parameter(Mandatory = $true)]
		[string]
		$Path,

		[Parameter()]
		[string]
		$Prefix = "ns"
	)

	process
	{
		$ns = New-Object System.Xml.XmlNamespaceManager($Content.NameTable)
		$ns.AddNamespace("ns", $Content.DocumentElement.NamespaceURI)
		return $Content.SelectNodes($Path, $ns)
	}
}

function Select-DeploymentConfigurationXmlValue
{
	param
	(
		[Parameter(Mandatory = $true)]
		[xml]
		$Content,

		[Parameter(Mandatory = $true)]
		[string]
		$Path,

		[Parameter()]
		[string]
		$Prefix = "ns"
	)

	process
	{
		return Select-DeploymentConfigurationXmlNode -Content $Content -Path $Path -Prefix $Prefix | Select-Object -ExpandProperty Value
	}
}

function Test-DeploymentConfiguration
{
	param
	(
		[Parameter(Mandatory = $true)]
		[string]
		$Path,

		[Parameter(Mandatory = $true)]
		[string]
		$WorkerRole,

		[Parameter()]
		[string]
		$ServiceDefinitionPath
	)

	process
	{
		if (-not($ServiceDefinitionPath))
		{
			$ServiceDefinitionPath = Join-Path $PSScriptRoot "..\..\..\..\..\Source\AzureCloudService\ServiceDefinition.csdef"
		}

		if (-not(Test-Path $ServiceDefinitionPath))
		{
			throw "Can't find the service definition. Path '$($ServiceDefinitionPath)' was not found"
		}

		$ServiceDefinitionPath = Resolve-Path $ServiceDefinitionPath
		[xml] $definition = Get-DeploymentConfigurationXml $ServiceDefinitionPath
		$definitionSettings = Select-DeploymentConfigurationXmlValue -Content $definition -Path "/ns:ServiceDefinition[@name='AzureCloudService']/ns:WorkerRole[@name='$($WorkerRole)']/ns:ConfigurationSettings/ns:Setting/@name"
		if (-not($definitionSettings))
		{
			Write-Warning "Can't find configuration settings"
			return $false
		}

		$definitionCertificates = Select-DeploymentConfigurationXmlValue -Content $definition -Path "/ns:ServiceDefinition[@name='AzureCloudService']/ns:WorkerRole[@name='$($WorkerRole)']/ns:Certificates/ns:Certificate/@name"
		if (-not($definitionCertificates))
		{
			Write-Host "No certificate defined"
		}

		[xml] $config = Get-DeploymentConfigurationXml $Path
		$configSettings = Select-DeploymentConfigurationXmlValue -Content $config -Path "/ns:ServiceConfiguration[@serviceName='AzureCloudService']/ns:Role[@name='$($WorkerRole)']/ns:ConfigurationSettings/ns:Setting/@name"
		
		if (-not ($configSettings -contains "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString"))
		{
			Write-Warning "Diagnostics Connection string not specified"
		}

		$names = $configSettings | ?{ $_ -ne "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" }
		$result = Compare-Object -ReferenceObject $definitionSettings -DifferenceObject $names
		$i = 0
		foreach ($p in $result.InputObject)
		{
			if ($result.SideIndicator[$i] -eq '=>')
			{
				Write-Warning "Configuration has setting '$($p)' which is not in the definition of the service"
			}
			else
			{
				Write-Warning "Configuration is missing the setting '$($p)'"
			}

			$i++
		}
		
		$configCertificates = Select-DeploymentConfigurationXmlValue -Content $config -Path "/ns:ServiceConfiguration[@serviceName='AzureCloudService']/ns:Role[@name='$($WorkerRole)']/ns:Certificates/ns:Certificate/@name"
		if (-not($definitionCertificates))
		{
			if (-not($configCertificates))
			{
				return $true
			}

			return $false
		}

		if (-not($configCertificates))
		{
			return $false
		}

		$certificatesResult = Compare-Object -ReferenceObject $configCertificates -DifferenceObject $definitionCertificates
		$i = 0
		foreach ($p in $certificatesResult.InputObject)
		{
			if ($certificatesResult.SideIndicator[$i] -eq '=>')
			{
				Write-Warning "Configuration has certificate '$($p)' which is not in the definition of the service"
			}
			else
			{
				Write-Warning "Configuration is missing the certificate '$($p)'"
			}

			$i++
		}



		return -not($result) -and -not($certificatesResult)
	}
}
	
function Get-TftBuildOutput
{
	[CmdletBinding()]
	param
	(
		[Parameter(Mandatory = $true)]
		[string]
		$BuildDefinitionName,

		[Parameter(Mandatory = $true)]
		[System.IO.DirectoryInfo]
		$TargetLocation,
			
		[Parameter()]
		[System.Uri]
		$ProjectUri = "https://tfsgorba.gorba.com:8443/tfs/DefaultCollection/"
	)

    begin
    {
		[Reflection.Assembly]::LoadWithPartialName('Microsoft.TeamFoundation.Build.Client') | Out-Null
		[Reflection.Assembly]::LoadWithPartialName('Microsoft.TeamFoundation.Client') | Out-Null
    }

	process
	{

		$tfs = [Microsoft.TeamFoundation.Client.TeamFoundationServerFactory]::GetServer($Projecturi) 
		$buildserver = $tfs.GetService([Microsoft.TeamFoundation.Build.Client.IBuildServer])
		$buildSpec = $buildServer.CreateBuildDetailSpec("Gorba", $BuildDefinitionName)
		# $buildSpec.InformationTypes = $null
		$builds = $buildServer.QueryBuilds($buildSpec)
		$build = $builds.Builds | Sort-Object -Descending -Property StartTime | Select-Object -First 1
        if (-not($build))
        {
            throw "No build found for definition name '$($BuildDefinitionName)'"
        }

		if (-not(Test-Path $TargetLocation))
		{
			New-Item -ItemType Directory -Path $TargetLocation | Out-Null
		}

		return $build.DropLocation
	}
}