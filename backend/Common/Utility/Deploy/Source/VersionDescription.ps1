<#
	.SYNOPSIS
	   Script to get the files (exe and dll) in order to update the version description. 
       You can created a simple text file or update directly the Version description excel file. In that case, the script adds a new sheet with all found information. 
    
    .DESCRIPTION
    	Gets recursively all the dll and exe from the base directory and insert them into the excel sheet if the ExcelFileName name is defined, otherwize,
        creates only a simple text file with CSV format containing all the found information: Product name;File name;File version; sorted into
        alphabetic order.
    
    .PARAMETER Path
        The path of the base directory where the script starts to list files.
        
    .PARAMETER OutputDir 
    	The path where you would like the text file saved. The spreadsheet will be called Generated_VD.txt.
        
    .PARAMETER ExcelFileName		
        The full file name of the Version description spreadsheet file.
        
    .NOTES
     Author : Jerome Coutant
     Requires : These dlls ClosedXML.dll (v 0.66.1.0) and DocumentFormat.OpenXml.dll (v2.0.5022.0) must be located where the VersionDescription.ps1 script is executed.
     
    .EXAMPLE 
    This example create a file Generated_VD.txt into c:\temp\ and update the excel spreadsheet 
    c:\temp\VD\VD_TEST.xlsx adding a new sheet on 2nd position with all exe and dll found into c:\temp\.
    
    [ps] .\VersionDescription -Path "c:\temp\" -ExcelFileName "c:\temp\VD\VD_TEST.xlsx"
    
#>
param
(
    [Parameter(Mandatory = $true)] [string] $Path,
    [string] $OutputDir = "",
    [string] $ExcelFileName = ""
)

begin
{
	function Get-ScriptDirectory()
	{
		$Invocation = (Get-Variable MyInvocation -Scope 1).Value
		Split-Path $Invocation.MyCommand.Path
	}
    
    Add-Type -AssemblyName "System.Xml.Linq"
    $scriptDirectory = Get-ScriptDirectory

    $openXml = Join-Path $scriptDirectory "DocumentFormat.OpenXml.dll"
    [System.Reflection.Assembly]::LoadFrom($openXml)

    $closedXml = Join-Path $scriptDirectory "ClosedXml.dll"
    [System.Reflection.Assembly]::LoadFrom($closedXml)
}

    
process
{
    #
    # Input path validation
    # 
    if(-not(Test-Path $Path))
    {
        throw "Please provide a valid path. '$($Path)' not found."
    }

    #
    # Output dir validation
    # 
    if (![string]::IsNullOrEmpty($OutputDir))
    {
        if(-not(Test-Path $OutputDir))
        {
            throw "Please provide a valid path for output directiry. '$($OutputDir)' not found."
        }
    }
    else
    {
        $OutputDir = $Path
    }

    #
    # Excel file name validation
    # 
    if (![string]::IsNullOrEmpty($ExcelFileName))
    {
        if(-not(Test-Path $ExcelFileName))
        {
            throw "The $($ExcelFileName) is not found. Please provide a valid file name."
        }
        $toExcel = $true
    }
    else
    {
        $toExcel = $false
    }

     
    $VersionDescription = "$($OutputDir)\Generated_VD.txt"

    if(Test-Path $VersionDescription)
    {
        
        $answer = Read-Host "The version description '$($VersionDescription)' already exists. Are sure you want to replace it? (y/n)"
        if ($answer -ne "y")
        {
            Write-Warning "Ok, nothing will be done"
            exit
        }

        rm $VersionDescription -Force
    }


    $result = Get-ChildItem "$($Path)" -Recurse  -include *.dll,*.exe -exclude *.vshost.exe | sort-object 
    $currentProduct = ""

        # Into the text file. 
        ForEach ($item in $result)
        {	
            $product = "$($item.VersionInfo.ProductName)"
            if ($currentProduct -ne $product)
            {
                $currentProduct = $product
                $msg = "$($currentProduct)" 
            }
            else
            {
                $msg = "" 
            }

            $msg = "$($msg);$($item.Name);$($item.VersionInfo.FileVersion);"
            $msg | out-File $VersionDescription -append -encoding ASCII
        }

    if ($toExcel)
    {   
        $workbook = New-Object "ClosedXML.Excel.XLWorkbook"($ExcelFileName)
        
        # Insert sheet into excel file. 
        $sheetNumber = $workbook.Worksheets.Count + 1
        $sheetName = "<Version> $($sheetNumber)"
        $worksheet = $workbook.worksheets.Add($sheetName)
        
        # Move the new sheet ot the 2nd position
        $worksheet.Position = 2               
        
        $rowIndex = 6
        ForEach ($item in $result)
        {	
            $product = "$($item.VersionInfo.ProductName)"
            if ($currentProduct -ne $product)
            {
                $currentProduct = $product
                $worksheet.Cell($rowIndex, 1).Value = "$($currentProduct)" 
            }
            

            $worksheet.Cell($rowIndex, 2).Value = $item.Name
            $worksheet.Cell($rowIndex, 3).Value = $item.VersionInfo.FileVersion
            $rowIndex ++
        }
         
        
    }

    write-Host "File '$($VersionDescription)' written successfully"

}

end
{
    $workbook.Save()
}