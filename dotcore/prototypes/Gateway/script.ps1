 <# Get current directory path #>
$src = (Get-Item -Path ".\" -Verbose).FullName;

echo "current dir : $src"
<# Iterate all directories present in the current directory path #>
Get-ChildItem $src -directory | where {$_.PsIsContainer} | Select-Object -Property Name | ForEach-Object {
    $cdProjectDir = [string]::Format("cd /d {0}\{1}",$src, $_.Name);
    echo "cdProjectDir : $cdProjectDir"
    <# Get project's bundle config file path #>    
    $projectDir = [string]::Format("{0}\{1}\appsettings.json",$src, $_.Name); 
    echo "projectDir : $projectDir"
    $fileExists = Test-Path $projectDir;
    echo "appsettings fileExists : $fileExists"
    <# Check project having bundle config file #>
    if($fileExists -eq $true){
        <# Start cmd process and execute 'dotnet run' #>
        $params=@("/C"; $cdProjectDir; " && dotnet run"; )
        echo "runas params : $params"
        Start-Process -Verb runas "cmd.exe" $params;
    }
} 