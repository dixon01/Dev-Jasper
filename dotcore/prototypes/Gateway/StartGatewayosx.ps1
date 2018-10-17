 <# Get current directory path #>
$src = (Get-Item -Path ".\" -Verbose).FullName;

echo "current dir : $src"
<# Iterate all directories present in the current directory path #>
 $count =0;
Get-ChildItem $src -directory | where {$_.PsIsContainer} | Select-Object -Property Name | ForEach-Object {
    $cdProjectDir = [string]::Format("cd {0}/{1}",$src, $_.Name);
    echo "cdProjectDir : $cdProjectDir"
    <# Get project's bundle config file path #>    
    $projectDir = [string]::Format("{0}\{1}\appsettings.json",$src, $_.Name); 
    echo "projectDir : $projectDir"
    $fileExists = Test-Path $projectDir;
    echo "appsettings fileExists : $fileExists"
    <# Check project having bundle config file #>
    if($fileExists -eq $true){
        $count = $count + 1;
        <# Start cmd process and execute 'dotnet run' #>
         <# $params=@( $cdProjectDir; " && dotnet run"; ) #>
        $params=@( $cdProjectDir;  ) + " dotnet run" + " ";
        echo "runas params : $params" + " start$count.command"
         $params   > start$count.command; chmod +x start$count.command; open start$count.command;
    }
} 