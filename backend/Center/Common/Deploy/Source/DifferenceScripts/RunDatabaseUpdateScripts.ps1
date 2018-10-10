<#
    .SYNOPSIS
    Runs database scripts in the specified folder.

    .PARAM ScriptsFolder
    Required. Folder containing sql scripts to be run. Accepted name format: Version_x_x_x_x.sql where x_x_x_x is valid version number.

    .PARAM MinVersion
    Optional (inclusive) minimum version. If specified, only scripts with version equals or higher than this value will be run; otherwise, all scripts will be run.

    .PARAM MaxVersion
    Optional (inclusive) maximum version. If specified, only scripts with version equals or lower than this value will be run; otherwise, all scripts will be run.

    .PARAM ServerInstance
    Database to run scripts. Default is localhost (".\").

    .PARAM WhatIf
    Switch to check which scripts would be run. When specified, scripts ARE NOT actually run.

#>
param
(
    [Parameter(Mandatory = $true)]
    [ValidateScript({Test-Path $_})]
    [string]
    $ScriptsFolder,

    [Parameter()]
    [System.Version]
    $MinVersion = $null,

    [Parameter()]
    [System.Version]
    $MaxVersion = $null,

    [Parameter()]
    [string]
    $ServerInstance = ".",

    [Parameter()]
    [switch]
    $WhatIf
)

begin
{
    function Get-ScriptUpdateInfo
    {
        param
        (
            [ValidateScript({Test-Path $_})]
            [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
            $Path
        )

        process
        {
            if ($Path -match "Version_(?<Major>[\d]+)_(?<Minor>[\d]+)_(?<Build>[\d]+)_(?<Revision>[\d]+)\.sql$")
            {
                $version = New-Object System.Version($Matches["Major"], $Matches["Minor"], $Matches["Build"], $Matches["Revision"])
                $obj = New-Object PSObject -Property @{
                    Version = $version
                    Path = $Path
                    }
                return $obj
            }

            Write-Warning "File '$($Path)' doesn't match the standard pattern"
        }
    }

    Get-PSSnapin -Registered SqlServerCmdletSnapin100 -ErrorAction Stop | Add-PSSnapin -ErrorAction SilentlyContinue
    $WhatIfPreference = $WhatIf.ToBool()
}

process
{
    $items = Get-ChildItem -Path $ScriptsFolder -Recurse -Include "*.sql" | Get-ScriptUpdateInfo | sort -Property Version | ?{ -not($MinVersion) -or ($_.Version -ge $MinVersion) } | ?{ -not($MaxVersion) -or ($_.Version -le $MaxVersion) }
    if ($WhatIfPreference)
    {
        Write-Host "Files to be executed: "
        $items | %{ Write-Host "[$($_.Version)] $($_.Path)" }
        return
    }

    $items | %{ Write-Host "Executing script '$($_.Path)'"; Invoke-Sqlcmd -ServerInstance $ServerInstance -InputFile $_.Path }
}

end
{
}