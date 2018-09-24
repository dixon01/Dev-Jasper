## WES
## This script copies Acapela files into the correct directory structure for a software release
##
$acapelaRootDir = "\\gorba.com\daten\Softwareserver_Release\SW\99_common\90_Acapela\Acapela TTS for Windows 9.2"
$destinationSoftwareDir = "\\gorba.com\daten\Softwareserver_Delivery\Gorba\SW\01_icenter\50_icenter.suite\2.0.1450\Setup\Software"

$acapelaVersion = "9.200"

foreach ($languageDir in Get-ChildItem (Join-Path $acapelaRootDir "data")  | ?{ $_.PSIsContainer})  {
    $language = $languageDir.Name
    ##Write-Host $languageDir.FullName
    $languageFilesDir = Get-ChildItem $languageDir.FullName | ?{ $_.Name.StartsWith($language + "-") }
    ##Write-Host $languageFilesDir.FullName

    foreach ($voiceDir in Get-ChildItem $languageDir.FullName | ?{ $_.Name -ne $languageFilesDir.Name}) {
        ##Write-Host $voiceDir.FullName
        $voice = $voiceDir.Name
        $destinationName = "Acapela-" + $language + "-" + $voice + "-" + $acapelaVersion
        Write-Host "Creating $destinationName"
        $destinationXml = Join-Path $destinationSoftwareDir "$destinationName.xml"
        if (Test-Path $destinationXml) {
            Write-Host -ForegroundColor Yellow "File $destinationXml already exists, ignoring that voice"
            continue
        }

        Add-Content $destinationXml ('<?xml version="1.0" encoding="utf-8"?>
<SoftwarePackage xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" ID="Acapela.' + $language + '.' + $voice + '" Name="Acapela ' + $language + ' ' + $voice.Replace('_', ' ') + '">
  <Version Name="' + $acapelaVersion + '">
    <Description>Acapela TTS for Windows ' + $acapelaVersion + ' Voice ' + $language + ' ' + $voice.Replace('_', ' ') + '</Description>
  </Version>
</SoftwarePackage>')

        $languageDest = Join-Path $destinationSoftwareDir "$destinationName\Progs\Acapela\data\$language"
        mkdir $languageDest
        Copy-Item $languageFilesDir.FullName $languageDest -Recurse
        Copy-Item $voiceDir.FullName $languageDest -Recurse
        ##break
    }

    ##break
}