<#
	.SYNOPSIS
	Publishes ClickOnce applications to Azure storage.

	.PARAM StorageAccountName
	The account name of the storage

	.PARAM StorageAccountKey
	The primary access key of the storage

	.PARAM Container
	The name of the container used to store resources. Default value is "clickonce"
	
	.PARAM DeployAll
	Value indicating whether to deploy all ClickOnce apps
	
	.PARAM DeployAdmin
	Value indicating whether to deploy icenter.admin

	.PARAM DeployDiag
	Value indicating whether to deploy icenter.diag

	.PARAM DeployMedia
	Value indicating whether to deploy icenter.media

	.PARAM AdminPath
	The path where the binaries are for icenter.admin (set only if param GetLatestBuild is not set)
	
	.PARAM DiagPath
	The path where the binaries are for icenter.diag (set only if param GetLatestBuild is not set)
	
	.PARAM MediaPath
	The path where the binaries are for icenter.media (set only if param GetLatestBuild is not set)

	.PARAM Beta
	Value indicating whether to deploy beta version

	.PARAM GetLatestBuild
	Value indicating whether to get the latest build(s) or to use the binaries defined in the Path
	parameters

	.PARAM Force
	Value indicating whether to force a deployment without a confirmation dialog
#>
[CmdletBinding(DefaultParameterSetName = "Custom")]
param
(
	[Parameter(Mandatory = $true)]
	[string]
	$StorageAccountName,

	[Parameter(Mandatory = $true)]
	[string]
	$StorageAccountKey,

	[Parameter()]
	[string]
	$Container = "clickonce",

	[Parameter()]
	[switch]
	$DeployAll,

	[Parameter()]
	[switch]
	$DeployAdmin,

	[Parameter()]
	[switch]
	$DeployDiag,

	[Parameter()]
	[switch]
	$DeployMedia,

	[Parameter(ParameterSetName = "Custom")]
	[string]
	$AdminPath,

	[Parameter(ParameterSetName = "Custom")]
	[string]
	$DiagPath,

	[Parameter(ParameterSetName = "Custom")]
	[string]
	$MediaPath,

	[Parameter()]
	[switch]
	$Beta,

	[Parameter(ParameterSetName = "Latest")]
	[switch]
	$GetLatestBuild,

	[Parameter()]
	[switch]
	$Force
)

begin
{
	function Read-UserAgreement
	{
		param
		(
			[Parameter(Mandatory = $true)]
			[string]
			$StorageAccountName,

			[Parameter(Mandatory = $true)]
			[string]
			$Container
		)

		process
		{
			$title = "Publishing clickonce to container '$($Container) in storage $($StorageAccountName)'"
			$message = "Are you sure you want to continue?"
			Write-Warning "Any existing deployed application will be deleted"

			$options = @(
				New-Object System.Management.Automation.Host.ChoiceDescription("&No", "No, I'm not sure. Stop here")
				New-Object System.Management.Automation.Host.ChoiceDescription("&Yes", "Yes, I'm aware of the risk. Go on")
			)

			$result = $host.ui.PromptForChoice($title, $message, $options, 0)
			return $result -eq 1
		}
	}

	function Deploy-Folder
	{
		param
		(
			[Parameter(Mandatory = $false)]
			$storageContext,

			[Parameter(Mandatory = $true)]
			$Folder
		)

		process
		{
			$files = Get-ChildItem -Path $Folder -File
			foreach ($file in $files){
				$relative = Resolve-Path -Path $file -Relative
			}

			$folders = Get-ChildItem -Path $Folder -Directory
			foreach ($directory in $folders)
			{
				Write-Host ("Directory: {0}" -f $directory)
				Deploy-Folder -Folder $directory
			}
		}
	}
	
	function Get-LatestBuild
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
			$ProjectUri = "https://tfsgorba.gorba.com:8443/tfs/DefaultCollection/",

			[Parameter()]
			[switch]
			$Beta
		)

		process
		{
			[void][Reflection.Assembly]::LoadWithPartialName('Microsoft.TeamFoundation.Build.Client')
			[void][Reflection.Assembly]::LoadWithPartialName('Microsoft.TeamFoundation.Client')

			$tfs = [Microsoft.TeamFoundation.Client.TeamFoundationServerFactory]::GetServer($Projecturi) 
			$buildserver = $tfs.GetService([Microsoft.TeamFoundation.Build.Client.IBuildServer])
			$buildSpec = $buildServer.CreateBuildDetailSpec("Gorba", $BuildDefinitionName)
			# $buildSpec.InformationTypes = $null
			$builds = $buildServer.QueryBuilds($buildSpec)
			$build = $builds.Builds | Sort-Object -Descending -Property StartTime | Select-Object -First 1
            if (-not($build))
            {
                throw "Build output not found for definition '$($BuildDefinitionName)'"
            }

			if (-not(Test-Path $TargetLocation))
			{
				New-Item -ItemType Directory -Path $TargetLocation | Out-Null
			}

			$source = "$($build.DropLocationRoot)\ClickOnce\*\*.exe"
			Write-Host "Copying files from '$($source)' to '$($TargetLocation)"
			cp $source $TargetLocation
			if ($Beta)
			{
				cp "$($build.DropLocationRoot)\ClickOnce\*\BETA_*.application" $TargetLocation
				$folder = ls -Directory "$($build.DropLocationRoot)\ClickOnce\*\Application Files\BETA_*" `
					| Sort-Object -Property Name -Descending | Select-Object -ExpandProperty FullName -First 1
			}
			else
			{
				ls "$($build.DropLocationRoot)\ClickOnce\*\*.application" | ?{ $_.Name -notlike "BETA_*" } | %{ cp $_.FullName $TargetLocation }
				$folder = ls -Directory "$($build.DropLocationRoot)\ClickOnce\*\Application Files\*" `
					| Sort-Object -Property Name -Descending | ?{ $_ -notlike "BETA_*" } | Select-Object -ExpandProperty FullName -First 1
			}

			Write-Host "Getting application files from '$($folder)'"
			$targetApplicationFiles = Join-Path $TargetLocation "Application Files"
			if (-not(Test-Path $targetApplicationFiles))
			{
				New-Item -ItemType Directory $targetApplicationFiles | Out-Null
			}

			cp $folder -Recurse $targetApplicationFiles
			Write-Host "All ClickOnce resources copied"
		}
	}
}

process
{
	if (($PSCmdlet.ParameterSetName -eq "Latest") -and -not($GetLatestBuild))
	{
		Write-Warning "Both GetLatestBuild flag and specific paths not specified. Nothing to do"
		return
	}

	if (-not($Force))
	{
		Read-UserAgreement -StorageAccountName $StorageAccountName -Container $Container
	}

	$sources = @{
		"Admin" = $AdminPath
		"Diag" = $DiagPath
		"Media" = $MediaPath
	}

	$storageContext = New-AzureStorageContext -StorageAccountName $StorageAccountName -StorageAccountKey $StorageAccountKey

	$tempPath = Join-Path $env:TEMP (Join-Path "CenterResourcesDeployment" ([System.Guid]::NewGuid()))
	New-Item -ItemType Directory $tempPath | Out-Null

	$originalLocation = Convert-Path .
	try
	{
		$found = $false
		foreach ($key in $Sources.Keys)
		{
			$name = "Deploy$($Key)"
			$flag = Get-Variable -Name $name -ValueOnly
			Write-Host "Flag for '$($name): $($flag)"
			if (!$DeployAll -and !$flag)
			{
				continue
			}

			$resolveError = $null
			$p = Join-Path $tempPath $key
			if ($GetLatestBuild)
			{
				$found = $true
				Write-Host "Deploying $($Key) from latest version on build server"
				Get-LatestBuild -BuildDefinitionName "Center_$($key)" -TargetLocation $p -Beta:$Beta
			}
			else
			{
				if (-not($sources[$Key]) -or -not(Test-Path $sources[$Key]))
				{
					Write-Warning "Path for '$($Key) not specified. Skipping it"
					continue
				}

				Write-Host "Deploying $($Key) from '$($sources[$Key])'"
				$found = $true
				cp -Path $sources[$key] $tempPath -Recurse
			}
		}

		if (-not($found))
		{
			Write-Warning "No valid path found"
			return
		}

		Get-AzureStorageBlob -Container "clickonce" -Blob "*" -Context $storageContext | Remove-AzureStorageBlob        
	
		# moving to the directory is needed by Azure cmdlet to correctly set the path for blobs
		Push-Location $tempPath
		Add-Content -Path "ReadMe.txt" -Value "Deployed on $([System.DateTime]::Now.ToString())"
		Get-ChildItem -File -Recurse | Set-AzureStorageBlobContent -Context $storageContext -Container $Container -BlobType Block
		Pop-Location
	}
	catch
	{
		Write-Error ("Error during deployment: {0}" -f $_.Exception.Message)
		Write-Error $_.Exception.StackTrace
	}
	finally
	{
		Remove-Item -Path $tempPath -Recurse -Force
	}

	# make sure we didn't change the original location
	Set-Location $originalLocation
}

end
{
}