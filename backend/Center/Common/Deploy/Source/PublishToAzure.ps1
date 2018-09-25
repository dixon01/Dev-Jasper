<#
	.SYNOPSIS
	Deploys a package to Azure.

	.REMARKS
	Requires elevation to access the certificate store.
#>
param
(
	[Parameter(Mandatory = $true)]
	$ServiceName,

	[Parameter(Mandatory = $true)]
	$StorageAccountName,

	[Parameter(Mandatory = $true)]
	[ValidateScript({ Test-Path $_ })]
	$PackageLocation,

	[Parameter(Mandatory = $true)]
	[ValidateScript({ Test-Path $_ })]
	$CloudConfigLocation,

	[Parameter(Mandatory = $true)]
	$DeploymentLabel,

	[Parameter(Mandatory = $true)]
	[ValidateSet("Production", "Staging")]
	$Slot = "Staging",

	[Parameter()]
	$TimeStampFormat = "g",

	[Parameter()]
	$SelectedSubscription = "Gorba Azure Subscription",

	[Parameter()]
	[switch]
	$AlwaysDeleteExistingDeployments,

	[Parameter()]
	[switch]
	$DisableDeploymentUpgrade,

	[Parameter()]
	[switch]
	$Force
)

begin
{
	function Write-LogMessage
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$TimeStampFormat,

			[Parameter(Mandatory = $true)]
			[string]
			$Message
		)

		process
		{
			Write-Host "$(Get-Date -f $TimeStampFormat) - $($Message)"
		}
	}

	function Publish
	{
		param
		(
			[Parameter(Mandatory = $true)]
			$ServiceName,

			[Parameter(Mandatory = $true)]
			$StorageAccountName,

			[Parameter(Mandatory = $true)]
			$PackageLocation,

			[Parameter(Mandatory = $true)]
			$CloudConfigLocation,

			[Parameter(Mandatory = $true)]
			$DeploymentLabel,

			[Parameter(Mandatory = $true)]
			$Slot,

			[Parameter(Mandatory = $true)]
			$TimeStampFormat,

			[Parameter()]
			[bool]
			$AlwaysDeleteExistingDeployments,

			[Parameter()]
			[bool]
			$DisableDeploymentUpgrade
		)

		process
		{
			$deployment = Get-AzureDeployment -ServiceName $ServiceName -Slot $Slot -ErrorVariable a -ErrorAction SilentlyContinue
			if ($a[0] -ne $null)
			{
				Write-LogMessage -TimeStampFormat $TimeStampFormat "No deployment is detected. Creating a new deployment. "
			}

			#check for existing deployment and then either upgrade, delete + deploy, or cancel according to $alwaysDeleteExistingDeployments and $enableDeploymentUpgrade boolean variables
			if ($deployment.Name -ne $null)
			{
				if ($AlwaysDeleteExistingDeployments)
				{
					if ($DisableDeploymentUpgrade)
					{
						# Delete then create new deployment
						Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Deployment exists in $($ServiceName). Deleting deployment."
						DeleteDeployment -ServiceName $ServiceName -Slot $Slot -TimeStampFormat $TimeStampFormat
						CreateNewDeployment -ServiceName $ServiceName -PackageLocation $PackageLocation -CloudConfigLocation $CloudConfigLocation -DeploymentLabel $DeploymentLabel -Slot $Slot -TimeStampFormat $TimeStampFormat
					}
					else
					{
						# Update deployment inplace (usually faster, cheaper, won't destroy VIP)
						Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Deployment exists in $($ServiceName). Upgrading deployment."
						UpgradeDeployment -ServiceName $ServiceName -PackageLocation $PackageLocation -CloudConfigLocation $CloudConfigLocation -DeploymentLabel $DeploymentLabel -Slot $Slot -TimeStampFormat $TimeStampFormat
					}
				}
				else
				{
					Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "ERROR: Deployment exists in $ServiceName. Script execution cancelled."
					exit
				}
			}
			else
			{
				CreateNewDeployment -ServiceName $ServiceName -PackageLocation $PackageLocation -CloudConfigLocation $CloudConfigLocation -DeploymentLabel $DeploymentLabel -Slot $Slot -TimeStampFormat $TimeStampFormat
			}
		}
	}

	function CreateNewDeployment
	{
		param
		(
			[Parameter(Mandatory = $true)]
			$ServiceName,

			[Parameter(Mandatory = $true)]
			$PackageLocation,

			[Parameter(Mandatory = $true)]
			$CloudConfigLocation,

			[Parameter(Mandatory = $true)]
			$DeploymentLabel,

			[Parameter(Mandatory = $true)]
			$Slot,

			[Parameter(Mandatory = $true)]
			$TimeStampFormat
		)

		process
		{
			Write-Progress -Id 3 -Activity "Creating New Deployment" -Status "In progress"
			Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Creating New Deployment: In progress"

			$opstat = New-AzureDeployment -Slot $Slot -Package $PackageLocation -Configuration $CloudConfigLocation -label $DeploymentLabel -ServiceName $ServiceName
		
			$completeDeployment = Get-AzureDeployment -ServiceName $ServiceName -Slot $Slot
			$completeDeploymentID = $completeDeployment.deploymentid

			write-progress -id 3 -activity "Creating New Deployment" -completed -Status "Complete"
			Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Creating New Deployment: Complete, Deployment ID: $completeDeploymentID"
	
			StartInstances -ServiceName $ServiceName -Slot $Slot -TimeStampFormat $TimeStampFormat
		}
	}

	function UpgradeDeployment
	{
		param
		(
			[Parameter(Mandatory = $true)]
			$ServiceName,

			[Parameter(Mandatory = $true)]
			$PackageLocation,

			[Parameter(Mandatory = $true)]
			$CloudConfigLocation,

			[Parameter(Mandatory = $true)]
			$DeploymentLabel,

			[Parameter(Mandatory = $true)]
			$Slot,

			[Parameter(Mandatory = $true)]
			$TimeStampFormat
		)

		process
		{
			Write-Progress -Id 3 -Activity "Upgrading Deployment '$($ServiceName) | $($Slot)'" -Status "In progress"
			Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Upgrading Deployment: In progress"

			# perform Update-Deployment
			$setdeployment = Set-AzureDeployment -Upgrade -Slot $Slot -Package $PackageLocation -Configuration $CloudConfigLocation -Label $DeploymentLabel -ServiceName $ServiceName -Force -Verbose
	
			$completeDeployment = Get-AzureDeployment -ServiceName $ServiceName -Slot $Slot
			$completeDeploymentID = $completeDeployment.DeploymentId
	
			Write-Progress -Id 3 -Activity "Upgrading Deployment" -Completed -Status "Complete"
			Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Upgrading Deployment: Complete, Deployment ID: $($completeDeploymentID)"
		}
	}

	function DeleteDeployment
	{
		param
		(
			[Parameter(Mandatory = $true)]
			$ServiceName,

			[Parameter(Mandatory = $true)]
			$Slot,

			[Parameter(Mandatory = $true)]
			$TimeStampFormat
		)

		process
		{
			Write-Progress -Id 2 -Activity "Deleting Deployment" -Status "In progress"
			Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Deleting Deployment: In progress"

			#WARNING - always deletes with force
			$removeDeployment = Remove-AzureDeployment -Slot $Slot -ServiceName $ServiceName -Force

			Write-Progress -Id 2 -Activity "Deleting Deployment: Complete" -Completed -Status $removeDeployment
			Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Deleting Deployment: Complete"
		}
	}

	function StartInstances
	{
		param
		(
			[Parameter(Mandatory = $true)]
			$ServiceName,

			[Parameter(Mandatory = $true)]
			$Slot,

			[Parameter(Mandatory = $true)]
			$TimeStampFormat
		)

		process
		{
			Write-Progress -Id 4 -Activity "Starting Instances" -Status "In progress"
			Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Starting Instances: In progress"

			$deployment = Get-AzureDeployment -ServiceName $ServiceName -Slot $Slot
			$runstatus = $deployment.Status

			if ($runstatus -ne 'Running') 
			{
				$run = Set-AzureDeployment -Slot $Slot -ServiceName $ServiceName -Status Running
			}

			$deployment = Get-AzureDeployment -ServiceName $ServiceName -Slot $Slot
			$oldStatusStr = @("") * $deployment.RoleInstanceList.Count

			$retryTimeout = [System.TimeSpan]::FromSeconds(5)	
			while (-not(Test-AllInstancesRunning -RoleInstanceList $deployment.RoleInstanceList))
			{
				$i = 1
				foreach ($roleInstance in $deployment.RoleInstanceList)
				{
					$instanceName = $roleInstance.InstanceName
					$instanceStatus = $roleInstance.InstanceStatus

					if ($oldStatusStr[$i - 1] -ne $roleInstance.InstanceStatus)
					{
						$oldStatusStr[$i - 1] = $roleInstance.InstanceStatus
						Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Starting Instance '$instanceName': $instanceStatus"
					}

					Write-Progress -Id (4 + $i) -Activity "Starting Instance '$instanceName'" -Status "$instanceStatus"
					$i = $i + 1
				}

				Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Instances not ready. Trying again in $($retryTimeout)"
				Start-Sleep -Seconds $retryTimeout.TotalSeconds

				$deployment = Get-AzureDeployment -ServiceName $ServiceName -Slot $Slot
			}

			$i = 1
			foreach ($roleInstance in $deployment.RoleInstanceList)
			{
				$instanceName = $roleInstance.InstanceName
				$instanceStatus = $roleInstance.InstanceStatus

				if ($oldStatusStr[$i - 1] -ne $roleInstance.InstanceStatus)
				{
					$oldStatusStr[$i - 1] = $roleInstance.InstanceStatus
					Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Starting Instance '$($instanceName)': $($instanceStatus)"
				}

				$i = $i + 1
			}
	
			$deployment = Get-AzureDeployment -ServiceName $ServiceName -Slot $Slot
			$opstat = $deployment.Status 
	
			Write-Progress -Id 4 -Activity "Starting Instances" -Completed -Status $opstat
			Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Starting Instances: $opstat"
		}
	}

	function Test-AllInstancesRunning
	{
		param
		(
			[Parameter(Mandatory = $true)]
			$RoleInstanceList
		)

		process
		{
			foreach ($roleInstance in $RoleInstanceList)
			{
				if ($roleInstance.InstanceStatus -ne "ReadyRole")
				{
					Write-Verbose "Role $($roleInstance) is not yet ready"
					return $false
				}
			}
	
			return $true
		}
	}

	function Read-UserAgreement
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$ServiceName,

			[Parameter(Mandatory = $true)]
			[string]
			[ValidateSet( "Production", "Staging")]
			$Slot,

			[Parameter(Mandatory = $true)]
			[string]
			$StorageAccountName,

			[Parameter(Mandatory = $true)]
			[string]
			$PackagePath
		)

		process
		{
			$title = "Deploying to '$($ServiceName) - $($Slot)'"
			$message = "Are you sure you want to continue?"
			Write-Warning "Potentially dangerous operation!"
			$package = Get-ChildItem $PackagePath
			Write-Warning ("Current '$($Slot)' deployment on '$($ServiceName)' will be overridden using storage '$($StorageAccountName)'. The package was last updated on {0}" -f $package.LastWriteTime)
			if ([System.DateTime]::UtcNow.Subtract($package.LastWriteTimeUtc).TotalMinutes -gt 30)
			{
				Write-Warning "The package was built more than 30 minutes ago. Maybe you forgot to package it... ?"
			}

			$options = @(
				New-Object System.Management.Automation.Host.ChoiceDescription("&No", "No, I'm not sure. Stop here")
				New-Object System.Management.Automation.Host.ChoiceDescription("&Yes", "Yes, I'm aware of the risk. Go on")
			)

			$result = $host.ui.PromptForChoice($title, $message, $options, 0)
			return $result -eq 1
		}
	}
}

process
{
	Select-AzureSubscription $SelectedSubscription

	#main driver - publish & write progress to activity log
	Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Azure Cloud Service deploy script started."
	Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Preparing deployment of $($DeploymentLabel)"
	Get-AzureDeployment -ServiceName $ServiceName -ErrorAction SilentlyContinue -ErrorVariable DeploymentError | Out-Null
	if (-not ($DeploymentError) -and -not ($Force) -and (-not(Read-UserAgreement -ServiceName $ServiceName -Slot $Slot `
		-StorageAccountName $StorageAccountName -PackagePath $PackageLocation -ErrorAction Stop)))
	{
		Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Exiting as requested"
		exit
	}
	
	if ($Force)
	{
		Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Deploying because of the -Force flag"
	}
	else
	{
		Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Deploying because the target service doesn't exist or because of user agreement"
	}

	$fileInfo = Get-ChildItem -Path $PackageLocation
	Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Package last edited on '$($fileInfo.LastWriteTime)'"

	Publish -ServiceName $ServiceName -Slot $Slot -StorageAccountName $StorageAccountName -PackageLocation $PackageLocation -CloudConfigLocation $CloudConfigLocation -DeploymentLabel $DeploymentLabel -TimeStampFormat $TimeStampFormat -AlwaysDeleteExistingDeployments $AlwaysDeleteExistingDeployments -DisableDeploymentUpgrade $DisableDeploymentUpgrade

	$deployment = Get-AzureDeployment -Slot $Slot -ServiceName $ServiceName
	$deploymentUrl = $deployment.Url

	Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Created Cloud Service with URL $deploymentUrl."
	Write-LogMessage -TimeStampFormat $TimeStampFormat -Message "Azure Cloud Service deploy script finished."
}

end
{
}