<#
	.SYNOPSIS
	Performs the setup of an Azure system based on Center version 2.0

	.DESCRIPTION
	Performs the setup of an Azure system based on Center version 2.0.
	It uses the existing setup script to publish the content to the created system.
	The system is always created on the Staging slot of the used service.

	.EXAMPLE
	.\SetupSystem20.ps1 -Path path_to_setup_directory -ServiceName icenter-dev -DatabaseConnectionString some_database_connection_string -DatabaseServerName some_database_server_name -DatabaseName some_database_name -StorageConnectionString some_storage_connection_string
#>
param
(
	[Parameter()]
	[string]
	$Path,

	[Parameter(Mandatory = $true)]
	[string]
	$ServiceName,

	[Parameter(Mandatory = $true)]
	[string]
	$DatabaseServerName,

	[Parameter(Mandatory = $true)]
	[string]
	$DatabaseName,

	[Parameter(Mandatory = $true)]
	[string]
	$DatabaseUser,

	[Parameter(Mandatory = $true)]
	[string]
	$DatabasePassword,

	[Parameter(Mandatory = $true)]
	[string]
	$StorageConnectionString,

	[Parameter()]
	[switch]
	$Force,

	[Parameter()]
	[switch]
	$SkipStartPortal,

	[Parameter()]
	[switch]
	$SkipStartBackgroundSystem
)

begin
{
	function Read-UserConfirmation
	{
		<#
			.SYNOPSIS
			Prompts the user for confirmation

			.DESCRIPTION
			Prompts the user for confirmation and returns true if the user selected Yes, false otherwise
		#>
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$Title,

			[Parameter(Mandatory = $true)]
			[string]
			$Message,

			[Parameter()]
			[string]
			$YesDescription,

			[Parameter()]
			[string]
			$NoDescription
		)

		process
		{
			$yes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", $YesDescription

			$no = New-Object System.Management.Automation.Host.ChoiceDescription "&No", $NoDescription

			$options = [System.Management.Automation.Host.ChoiceDescription[]]($yes, $no)

			$result = $host.ui.PromptForChoice($title, $message, $options, 0)
			switch ($result)
			{
				0 { return $true }
				1 { return $false}
			}
		}
	}

	function Write-Message
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$Message,

			[Parameter()]
			[switch]
			$Warn
		)

		process
		{
			$msg = "[$([System.DateTime]::Now.ToString('G'))] $($Message)"
			if ($Warn)
			{
				Write-Warning $msg
				return
			}

			Write-Host $msg
		}
	}

	function Test-AzureDatabase
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ServerName,

			[Parameter(Mandatory = $true)]
			[string]
			$DatabaseName
		)

		process
		{
			Write-Message "Verifying database '$($DatabaseName)' from server '$($ServerName)'"
			$result = Get-AzureSqlDatabase -ServerName $ServerName -DatabaseName $DatabaseName -ErrorAction SilentlyContinue -ErrorVariable databaseError
			if ($databaseError)
			{
				return $false
			}

			return $true
		}
	}

	function Remove-AzureDatabase
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ServerName,

			[Parameter(Mandatory = $true)]
			[string]
			$DatabaseName
		)

		process
		{
			Write-Message "Removing database '$($DatabaseName)' from server '$($ServerName)'"
			$result = Remove-AzureSqlDatabase -ServerName $DatabaseServerName -DatabaseName $DatabaseName -Force
		}
	}

	function Test-AzureStorageContainer
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ConnectionString,

			[Parameter(Mandatory = $true)]
			[string]
			$ContainerName
		)

		process
		{
			Write-Message -Message "Verifying container '$($ContainerName)'"
			$azureStorageContext = New-AzureStorageContext -ConnectionString $ConnectionString
			$items = Get-AzureStorageBlob -Container $ContainerName -Context $azureStorageContext -ErrorAction SilentlyContinue -ErrorVariable containerNotFound
			if ($containerNotFound)
			{
				Write-Message -Message "Container '$($ContainerName)' not found on storage (it's good for testing :))"
				return $false
			}

			if ($items)
			{
				Write-Message -Message "The storage already contains some items in the container '$($ContainerName)'. It is suggested to remove existing resources before proceeding" -Warn
				return $true
			}

			Write-Message "The storage container '$($ContainerName)' is empty and can be used for testing"
			return $false
		}
	}

	function Publish-CloudRole
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			[ValidateScript({ Test-Path $_ })]
			$PackagePath,

			[Parameter(Mandatory = $true)]
			[string]
			[ValidateScript({ Test-Path $_ })]
			$ConfigPath,

			[Parameter(Mandatory = $true)]
			[string]
			$ServiceName,

			[Parameter(Mandatory = $true)]
			[string]
			$Slot
		)

		process
		{
			$deploymentLabel = "Test deployment $([System.DateTime]::Now.ToString('G'))"
			Write-Message -Message "Publishing to '$($ServiceName).$($Slot)'"
			$result = New-AzureDeployment -Slot $Slot -Package $PackagePath -Configuration $ConfigPath -label $deploymentLabel -ServiceName $ServiceName -DoNotStart
		}
	}

	function Test-CloudService
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$Name
		)

		process
		{
			try
			{
				$service = Get-AzureService -ServiceName $Name -ErrorAction Stop
				return $true
			}
			catch
			{
				#throw "Error while getting information about service '$($ServiceName)'. The service might be unavailable or there could be another issue."
				return $false
			}
		}
	}

	function Remove-CloudStorage
	{
		param
		(
			[Parameter()]
			$ConnectionString,

			[Parameter()]
			[string]
			$Name
		)

		process
		{
			Write-Message "Removing cloud storage '$($Name)"
			$context = New-AzureStorageContext -ConnectionString $ConnectionString
			Remove-AzureStorageContainer -Name $Name -Context $context
		}
	}

	function Test-CloudRole
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ServiceName,

			[Parameter(Mandatory = $true)]
			[string]
			$SlotName
		)

		process
		{
			$role = Get-AzureDeployment -ServiceName $ServiceName -Slot $SlotName -ErrorAction SilentlyContinue -ErrorVariable noRole
			if ($noRole -ne $null)
			{
				Write-Message -Message "The '$($SlotName)' role on service '$($ServiceName)' doesn't exist, we can safely deploy"
				return $false
			}

			return $true
		}
	}

	function Stop-CloudRole
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ServiceName,

			[Parameter(Mandatory = $true)]
			[string]
			$SlotName
		)

		process
		{
			Write-Message -Message "Stopping '$($ServiceName).$($SlotName)'"
			Stop-AzureService -ServiceName $ServiceName -Slot $SlotName | Out-Null
		}
	}

	function Remove-CloudRole
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ServiceName,

			[Parameter(Mandatory = $true)]
			[string]
			$Slot
		)

		process
		{
			Write-Message "Removing Role '$($ServiceName).$($Slot)" -Warn
			$result = Remove-AzureDeployment -ServiceName $ServiceName -Slot $Slot -Force
		}
	}

	function Invoke-EnsureStaging
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ServiceName,

			[Parameter()]
			[switch]
			$Force
		)

		process
		{
			Write-Message "Verifying $($ServiceName).Staging"
			$roleTest = Test-CloudRole -ServiceName $ServiceName -SlotName "Staging"
			if ($roleTest)
			{
				Write-Debug "No error while getting the role. The role exists"
				# There wasn't an error, meaning that the service already exists. Checking the Force flag
				if (-not($Force))
				{
					$userChoice = Read-UserConfirmation -Title "Delete cloud role" -Message "Are you sure you want to delete cloud role $($ServiceName).Staging?" `
						-YesDescription "Delete the role $($ServiceName).Staging" -NoDescription "Keep the role $($ServiceName).Staging and exit"
					if (-not($userChoice))
					{
						Write-Message "Exiting"
						exit
					}
				}

				Remove-CloudRole -ServiceName $ServiceName -Slot "Staging"
			}
			else
			{
				Write-Message "The Staging role on service '$($ServiceName)' doesn't exist, we can safely deploy"
			}
		}
	}

	function Invoke-EnsureProduction
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ServiceName
		)

		process
		{
			$roleTest = Test-CloudRole -ServiceName $ServiceName -SlotName "Production"
			if ($roleTest)
			{
				Write-Message -Message "$($ServiceName).Production exists. It will be stopped, if needed" -Warn
				Stop-CloudRole -ServiceName $ServiceName -SlotName "Production"
				return
			}

			Write-Message -Message "$($ServiceName).Production doesn't exist"
		}
	}

	function Invoke-EnsureService
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ServiceName
		)

		process
		{
			Write-Message -Message "Verifying the cloud service '$($ServiceName)'..."
			$serviceTest = Test-CloudService -Name $ServiceName
			if ($serviceTest)
			{
				Write-Message -Message "The cloud service exists"
			}
			else
			{
				Write-Message -Message "The service '$($ServiceName)' doesn't exist. Creating it" -Warn
				$service = New-AzureService -ServiceName $ServiceName -Location "West Europe" -Label "Test deployment" -ErrorAction SilentlyContinue -ErrorVariable serviceError
				if ($serviceError)
				{
					throw "Error while creating service '$($ServiceName)"
				}

				Write-Message -Message "Service '$($ServiceName)' correctly created"
			}
		}
	}

	function Invoke-EnsureStorage
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ConnectionString,

			[Parameter(Mandatory = $true)]
			[string]
			$Name
		)

		process
		{
			Write-Message "Verifying the storage container '$($Name)'..."
			$containerTest = Test-AzureStorageContainer -ConnectionString $ConnectionString -ContainerName $Name
			if (-not($containerTest))
			{
				return
			}

			$confirmation = Read-UserConfirmation -Title "Delete storage container" -Message "Do you want to delete the container '$($Name)'?" `
				-YesDescription "Delete the container" -NoDescription "Keep the container"
			if ($confirmation)
			{
				Remove-CloudStorage -ConnectionString $ConnectionString -Name $Name
				return
			}

			Write-Message -Message "The container '$($Name)' wasn't deleted" -Warn
		}
	}

	function Update-ConfigurationSetting
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[xml]
			$Configuration,

			[Parameter(Mandatory = $true)]
			[string]
			$Key,

			[Parameter(Mandatory = $true)]
			[string]
			$Value
		)

		process
		{
			Write-Message "Updating configuration setting"

			$configuration.ServiceConfiguration.Role | % { 
					$settingElement = $_.ConfigurationSettings.Setting | ? { $_.name -eq $Key }
					if ($settingElement -ne $null -and $settingElement.value -ne $Value)
					{
						$settingElement.value = $Value
					}
				}
		}
	}

	function Update-PortalConfig
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$Path,

			[Parameter(Mandatory = $true)]
			[string]
			$DiagnosticsConnectionString,

			[Parameter(Mandatory = $true)]
			[string]
			$CertificateThumbprint
		)

		process
		{
			$configuration = [xml](Get-Content $Path)
			Update-ConfigurationSetting -Configuration $configuration -Key "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" -Value $StorageConnectionString
			
			$configuration.ServiceConfiguration.Role.Certificates.Certificate.thumbprint = $thumbprint
			$tempFile = [System.IO.Path]::GetTempFileName()
			$configuration.Save($tempFile)
			notepad $tempFile
			return $tempFile
		}
	}

	function Update-BackgroundSystemConfig
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$Path,

			[Parameter(Mandatory = $true)]
			[string]
			$ServiceName,

			[Parameter(Mandatory = $true)]
			[string]
			$DatabaseConnectionString,

			[Parameter(Mandatory = $true)]
			[string]
			$StorageConnectionString
		)

		process
		{
			$configuration = [xml](Get-Content $Path)
			Update-ConfigurationSetting -Configuration $configuration -Key "BackgroundSystem.CenterDataContext" -Value $DatabaseConnectionString
			Update-ConfigurationSetting -Configuration $configuration -Key "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" -Value $StorageConnectionString
			Update-ConfigurationSetting -Configuration $configuration -Key "BackgroundSystem.NotificationsConnectionString" -Value "medi://$($ServiceName).cloudapp.net"
			$tempFile = [System.IO.Path]::GetTempFileName()
			$configuration.Save($tempFile)
			return $tempFile
		}
	}

	function Add-ServiceCertificate
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
			$ServiceName
		)

		process
		{
			$certificateCn = "$($ServiceName).cloudapp.net"
			$certificateFilePath = [System.IO.Path]::GetTempFileName()
			#Check if the certificate name was used before
			$securePassword = ConvertTo-SecureString -String "Gorba" -Force –AsPlainText 

			[string] $thumbprint = New-SelfSignedCertificate -DnsName $certificateCn -CertStoreLocation cert:\CurrentUser\My | Select-Object -ExpandProperty Thumbprint

			if ($thumbprint) 
			{ 
				# query the new installed cerificate again
				$storedCertificate = ls cert:\currentuser\My -Recurse | ?{ ($_.Subject -match "CN=$($certificateCn)")  -and ($_.Thumbprint -match $thumbprint) } | Select-Object -Last 1 -ExpandProperty Thumbprint
				#If new cert installed sucessfully with the same thumbprint 

				if($storedCertificate)
				{
					Write-Host "$($certificateCn) installed into CurrentUser\My successfully with thumprint '$($thumbprint)'"
					Write-Host "Exporting Certificate as .pfx file"
					Export-PfxCertificate -FilePath $certificateFilePath -Cert "cert:\currentuser\My\$($thumbprint)" -Password $securePassword | Out-Null
					Add-AzureCertificate -ServiceName $ServiceName -CertToDeploy $certificateFilePath -Password "Gorba" -ErrorAction Stop | Out-Null

					Write-Host "Certificate created with thumbprint '$($thumbprint)'"
					return $thumbprint
				}
 
				throw "Thumbprint is not the same between new cert and installed cert."
			}

			throw "Certificate for service $($ServiceName) was not created"
		}
	}

	function Invoke-EnsureCertificate
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ServiceName
		)

		process
		{
			$certs = Get-AzureCertificate -ServiceName $ServiceName
			foreach ($cert in $certs) 
			{ 
				$x509Cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 
				$x509Cert.Import([System.Convert]::FromBase64String($cert.Data)) 
				if ($x509Cert.Subject -eq "CN=$($ServiceName).cloudapp.net")
				{
					Write-Message "Found existing certificate with thumbprint '$($cert.Thumbprint)'"
					return $cert.Thumbprint
				}
			}
			
			return Add-ServiceCertificate -ServiceName $ServiceName
		}
	}

	function Start-CloudRole
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ServiceName,

			[Parameter(Mandatory = $true)]
			[string]
			$Slot
		)

		process
		{
			Write-Message -Message "Stopping '$($ServiceName).$($Slot)'"
			Stop-AzureService -ServiceName $ServiceName -Slot $Slot | Out-Null
		}
	}

	function Invoke-CloudServiceSwap
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ServiceName
		)

		process
		{
			Write-Message -Message "Swapping '$($ServiceName)'"
			Move-AzureDeployment -ServiceName $ServiceName | Out-Null
		}
	}

	Function Convert-ToProperties
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ConnectionString
		)

		process
		{
			$p = New-Object PSObject
			foreach ($o in $ConnectionString.Split(';'))
			{
				$key, $value = $o.Split('=')
				$p = $p | Add-Member -PassThru Noteproperty ($key -Replace " ", "") $value.Trim()
			}

			return $p
		}
	}

	function Update-CloudDatabase
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$DatabaseServerName,

			[Parameter(Mandatory = $true)]
			[string]
			$DatabaseName,

			[Parameter(Mandatory = $true)]
			[string]
			$DatabaseUser,

			[Parameter(Mandatory = $true)]
			[string]
			$DatabasePassword
		)

		begin
		{
			Import-Module -Name Sqlps
		}

		process
		{
			$serverInstance = "tcp:$($DatabaseServerName).database.windows.net,1433"
			Write-Message "Updating database password for Admin to 'test' on server '$($properties.Server)'"
			$query = "UPDATE [dbo].[Users] SET [HashedPassword]='098f6bcd4621d373cade4e832627b4f6' WHERE [Username]='Admin'"
			Invoke-Sqlcmd -ServerInstance $serverInstance -Database $DatabaseName -Username $DatabaseUser -Password $DatabasePassword -Query $query
			$query = "SELECT * FROM [dbo].[Users] WHERE [Username]='Admin'"
			Invoke-Sqlcmd -ServerInstance $serverInstance -Database $DatabaseName -Username $DatabaseUser -Password $DatabasePassword -Query $query
			pushd (Split-Path -Path $PSScriptRoot -Parent)
		}

		end
		{
		}
	}

	function Stop-CloudServices
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[array]
			$Services
		)

		process
		{
			$jobs = @()
				foreach ($vm in $vms)
				{
				$params = @($vm.Name, $vm.ServiceName)
				$job = Start-Job -ScriptBlock {
				param($ComputerName, $ServiceName)
					Start-AzureService -ServiceName
				start-Azurevm -Name $ComputerName -ServiceName $ServiceName
					} -ArgumentList $params 
				$jobs = $jobs + $job
			}
			# Wait for it all to complete
			Wait-Job -Job $jobs
			# Getting the information back from the jobs
			Get-Job | Receive-Job
		}
	}

	function Start-CloudServices
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[array]
			$Services
		)

		process
		{
			$jobs = @()
			foreach ($vm in $Services)
			{
				$params = @($vm.ServiceName, $vm.Slot)
				$job = Start-Job -ScriptBlock {
				param($ServiceName, $Slot)
					Write-Host "Starting $($ServiceName).$($Slot)"
					Start-AzureService -ServiceName $ServiceName -Slot $Slot
					} -ArgumentList $params
				$jobs = $jobs + $job
			}

			# Wait for it all to complete
			Write-Host "Starting $($jobs.Length) job(s)"
			Wait-Job -Job $jobs
			# Getting the information back from the jobs
			Get-Job | Receive-Job
			Write-Host "Completed"
		}
	}
	
	if ((Get-Module -ListAvailable Azure) -eq $null) 
    { 
        throw "Windows Azure Powershell not found! Please install from http://www.windowsazure.com/en-us/downloads/#cmd-line-tools" 
    }
	
	if ((Get-Module -ListAvailable Azure) -eq $null) 
    { 
        throw "Windows Azure Powershell not found! Please install from http://www.windowsazure.com/en-us/downloads/#cmd-line-tools" 
    }

	# setup paths
	$servicesPath = Join-Path $Path "CloudServices"
	if (-not(Test-Path $servicesPath))
	{
		throw "Path '$($servicesPath)' not found"
	}

	$portalPath = Join-Path $servicesPath "Portal"
	if (-not(Test-Path $portalPath))
	{
		throw "Path '$($portalPath)' not found"
	}

	$portalPackagePath = Join-Path $portalPath "AzureCloudService.cspkg"
	if (-not(Test-Path $portalPackagePath))
	{
		throw "Path '$($portalPackagePath)' not found"
	}

	$portalConfigPath = Join-Path $portalPath "ServiceConfiguration.Cloud.cscfg"
	if (-not(Test-Path $portalConfigPath))
	{
		throw "Path '$($portalConfigPath)' not found"
	}

	$backgroundSystemPath = Join-Path $servicesPath "BackgroundSystem"
	if (-not(Test-Path $backgroundSystemPath))
	{
		throw "Path '$($backgroundSystemPath)' not found"
	}

	$backgroundSystemPackagePath = Join-Path $backgroundSystemPath "AzureCloudService.cspkg"
	if (-not(Test-Path $backgroundSystemPackagePath))
	{
		throw "Path '$($backgroundSystemPackagePath)' not found"
	}

	$backgroundSystemConfigPath = Join-Path $backgroundSystemPath "ServiceConfiguration.Cloud.cscfg"
	if (-not(Test-Path $backgroundSystemConfigPath))
	{
		throw "Path '$($backgroundSystemConfigPath)' not found"
	}

	$setupScriptPath = Join-Path $Path "Setup.ps1"
	if (-not(Test-Path $setupScriptPath))
	{
		throw "Path '$($setupScriptPath)' not found"
	}
}

process
{
	# storage check
	
	Invoke-EnsureStorage -ConnectionString $StorageConnectionString -Name "resources"
	Invoke-EnsureStorage -ConnectionString $StorageConnectionString -Name "clickonce"

	# service check

	Invoke-EnsureService -ServiceName $ServiceName

	$bgsService = "$($ServiceName)-bs"
	Invoke-EnsureService -ServiceName $bgsService

	# production check

	Invoke-EnsureProduction -ServiceName $ServiceName
	Invoke-EnsureProduction -ServiceName $bgsService

	# staging check

	Invoke-EnsureStaging -ServiceName $ServiceName
	Invoke-EnsureStaging -ServiceName $bgsService

	$testDatabase = Test-AzureDatabase -ServerName $DatabaseServerName -DatabaseName $DatabaseName
	if ($testDatabase)
	{
		$result = Read-UserConfirmation -Title "Delete database" -Message "Are you sure you want to delete database $($DatabaseName) on server $($DatabaseServerName)?" `
			-YesDescription "Delete the database" -NoDescription "Keep the database and exit"
		if ($result)
		{
			Remove-AzureDatabase -ServerName $DatabaseServerName -DatabaseName $DatabaseName
		}

		Write-Message "The database won't be deleted" -Warn
	}

	# all tests executed in the begin section. We can safely upload the services
	$thumbprint = Invoke-EnsureCertificate -ServiceName $ServiceName
	$updatedPortalConfigPath = Update-PortalConfig -Path $portalConfigPath -DiagnosticsConnectionString $StorageConnectionString -CertificateThumbprint $thumbprint
	Publish-CloudRole -PackagePath $portalPackagePath -ConfigPath $updatedPortalConfigPath -ServiceName $ServiceName -Slot "Staging"

	Invoke-CloudServiceSwap -ServiceName $ServiceName

	$updatedBackgroundSystemConfigPath = Update-BackgroundSystemConfig -Path $backgroundSystemConfigPath -ServiceName $bgsService -DatabaseConnectionString $DatabaseConnectionString -StorageConnectionString $StorageConnectionString
	Publish-CloudRole -PackagePath $backgroundSystemPackagePath -ConfigPath $updatedBackgroundSystemConfigPath -ServiceName $bgsService -Slot "Staging"

	Invoke-CloudServiceSwap -ServiceName $bgsService

	Write-Message "Please do the following:"
	Write-Message "1. Enable remote desktop on Portal ($($ServiceName))"
	Read-Host "Type <Enter> when done"

	$services = @(@{
		"ServiceName" = $ServiceName
		"Slot" = "Production"
	},
	@{
		"ServiceName" = $bgsService
		"Slot" = "Production"
	})

	Write-Message "Starting $($services.Count) service(s)"
	Start-CloudServices -Services $services

	Write-Message "2. As soon as the $($ServiceName).Production service is running, copy the content of the AppDataContent directory to AppData directory on $($ServiceName) (remember to edit the BackgroundSystemConfiguration.xml file!)"
	Read-Host "Type <Enter> when done"

	Write-Message "3. Verify that the $($bgsService).Production service is running"
	Read-Host "Type <Enter> when done"

	Update-CloudDatabase -DatabaseServerName $DatabaseServerName -DatabaseName $DatabaseName -DatabaseUser $DatabaseUser -DatabasePassword $DatabasePassword
	
	$address = "http://$($ServiceName).cloudapp.net"
	Write-Message "3. Wait until both services are running and test the portal ($($address))"
	Read-Host "Type <Enter> when done"

	Write-Message "Running the setup"
	& $setupScriptPath -CenterPortalAddress $address
}

end
{

}