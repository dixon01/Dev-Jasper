Function Validate-Xml
{ 
    <#
        .SYNOPSIS
        Validate-Xml enables to validate an XML file according to the xsd file.
        Verifie d’un port tcp distant

        .DESCRIPTION
        Used to validate an XML file against a given xsd schema.

        .PARAMETER xml
        type="XmlDocument" 
        mandatory="true" 
        position="0" 
        pipeline="true"  

        .PARAMETER schema
        type="String" 
        mandatory="true" 
        position="1" 
        pipeline="false" 

        .EXAMPLE
        $schema = "C:\user\xml\TTR-FBS-1-1.xsd" 
        [xml]$xml = gc 'C:\user\xml\TTR-FBS2010012201.xml' 
        Validate-Xml $xml $schema 
    #>
    param(     
        [Parameter( 
            Mandatory = $true, 
            Position = 0, 
            ValueFromPipeline = $true, 
            ValueFromPipelineByPropertyName = $true)] 
        [xml]$xml, 
        [Parameter( 
            Mandatory = $true, 
            Position = 1, 
            ValueFromPipeline = $false)] 
        [string]$schema 
        ) 
 
    #Array to hold our error objects 
    $ValidationErrors = @() 
         
    #Get the schema from xsd file
    [xml]$xmlschema = Get-Content $schema                 
    
    #Extract the targetNamespace from the schema
    $namespace = $xmlschema.get_DocumentElement().targetNamespace       

    #Define the script block that will act as the validation event handler
$code = @' 
    param($sender,$a) 
    $ex = $a.Exception 
    #<c>Trim out the useless,irrelevant parts of the message</c> 
    $msg = $ex.Message -replace " in namespace 'http.*?'","" 
    #<c>Create the custom error object using a hashtable</c> 
    $properties = @{LineNumber=$ex.LineNumber; LinePosition=$ex.LinePosition; Message=$msg} 
    $o = New-Object PSObject -Property $properties 
    #<c>Add the object to the $validationerrors array</c> 
    $validationerrors += $o 
'@ 
    #Convert the code block to as ScriptBlock
    $validationEventHandler = [scriptblock]::Create($code) 
     
    #Create a new XmlReaderSettings object
    $rs = new-object System.Xml.XmlreaderSettings 
    
    #Load the schema into the XmlReaderSettings object    
    [Void]$rs.schemas.add($namespace,(new-object System.Xml.xmltextreader($schema))) 
    
    #Instruct the XmlReaderSettings object to use Schema validation
    $rs.validationtype = "Schema" 
    $rs.ConformanceLevel = "Auto" 
    
    #Add the scriptblock as the ValidationEventHandler 
    $rs.add_ValidationEventHandler($validationEventHandler) 
     
    #Create a temporary file and save the Xml into it 
    $xmlfile = [System.IO.Path]::GetTempFileName() 
    $xml.Save($xmlfile) 
     
    #Create the XmlReader object using the settings defined previously
    $reader = [System.Xml.XmlReader]::Create($xmlfile,$rs) 
     
    #Temporarily set the ErrorActionPreference to SilentlyContinue, 
    #as we want to use our validation event handler to handle errors 
    $previousErrorActionPreference = $ErrorActionPreference 
    $ErrorActionPreference = "SilentlyContinue" 
     
    #Read the Xml using the XmlReader
    while ($reader.read()) 
    {
        $null
    }  
    $reader.close() 
     
    Remove-Item $xmlfile 
     
    #Reset the ErrorActionPreference back to the previous value 
    $ErrorActionPreference = $previousErrorActionPreference  
     
    return $ValidationErrors 
} 