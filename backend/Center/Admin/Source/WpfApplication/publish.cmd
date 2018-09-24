echo off
cls
echo Test
rem Put my cmdline here
set build.counter="222"


"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MsBuild.exe" Admin.WpfApplication.csproj /m /target:publish /property:Configuration=Release /property:Platform=AnyCPU /property:AllowUntrustedCertificate=True /property:ApplicationVersion=1.3.0.%build.counter%
echo Done
