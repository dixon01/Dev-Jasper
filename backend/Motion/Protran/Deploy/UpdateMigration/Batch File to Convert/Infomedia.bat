xcopy D:\Infomedia\Program\ReplaceUpdate\taskkill.exe C:\WINEMB21\system32 /Y
TASKKILL /IM TftUpdate.exe /f
del D:\Progs\ /s /q /f
rmdir D:\Progs\ /s /q
xcopy D:\Infomedia\Program\ReplaceUpdate\Startup D:\Progs\Startup\ /R /S /E /Y
del "C:\Documents and Settings\infomedia\Start Menu\Programs\StartUp\*.lnk" /F
xcopy "D:\Progs\Startup\*.lnk" "C:\Documents and Settings\infomedia\Start Menu\Programs\StartUp" /I /E /H /Q
EwfMgr c: -commit
shutdown.exe -r -t 03 -d p
EXIT 