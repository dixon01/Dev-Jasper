@echo off
set RootFolder=%1
if "%RootFolder%"=="" set RootFolder=..\..
Echo RootFolder=%RootFolder%

REM ...............Remove the click-once stuff that is automatically generated in the build
rmdir %RootFolder%\Center\Admin\Source\WpfApplication\bin\Release\app.publish /s /q
rmdir %RootFolder%\Center\Diag\Source\WpfApplication\bin\Release\app.publish /s /q
rmdir %RootFolder%\Center\Media\Source\WpfApplication\bin\Release\app.publish /s /q
del %RootFolder%\Center\Admin\Source\WpfApplication\bin\Release\*.pdb /f /q
del %RootFolder%\Center\Diag\Source\WpfApplication\bin\Release\*.pdb /f /q
del %RootFolder%\Center\Media\Source\WpfApplication\bin\Release\*.pdb /f /q
del %RootFolder%\Center\Admin\Source\WpfApplication\bin\Release\*.application /f /q
del %RootFolder%\Center\Diag\Source\WpfApplication\bin\Release\*.application /f /q
del %RootFolder%\Center\Media\Source\WpfApplication\bin\Release\*.application /f /q
del %RootFolder%\Center\Admin\Source\WpfApplication\bin\Release\*.manifest /f /q
del %RootFolder%\Center\Diag\Source\WpfApplication\bin\Release\*.manifest /f /q
del %RootFolder%\Center\Media\Source\WpfApplication\bin\Release\*.manifest /f /q
del %RootFolder%\Center\Admin\Source\WpfApplication\bin\Release\publish.cmd /f /q

"C:\Program Files\7-Zip\7z.exe" a %RootFolder%\Center\Admin\Deploy\setup.zip %RootFolder%\Center\Admin\Source\WpfApplication\bin\Release\*
"C:\Program Files\7-Zip\7z.exe" a %RootFolder%\Center\Diag\Deploy\setup.zip %RootFolder%\Center\Diag\Source\WpfApplication\bin\Release\*
"C:\Program Files\7-Zip\7z.exe" a %RootFolder%\Center\Media\Deploy\setup.zip %RootFolder%\Center\Media\Source\WpfApplication\bin\Release\*