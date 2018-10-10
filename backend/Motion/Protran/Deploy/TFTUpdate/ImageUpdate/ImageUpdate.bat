xcopy E:\ImageUpdate\Update D:\Update\ /Y
xcopy E:\ImageUpdate\UpdateData D:\UpdateData\ /R /S /E /Y
xcopy E:\ImageUpdate\*.ttf C:\WINEMB21\Fonts /Y
xcopy E:\ImageUpdate\*.otf C:\WINEMB21\Fonts /Y
D:\Progs\TFTUpdate\TFTFontControl.exe fonts
del "C:\Documents and Settings\infomedia\Start Menu\Programs\StartUp\*.lnk" /F
xcopy "D:\Update\*.lnk" "C:\Documents and Settings\infomedia\Start Menu\Programs\StartUp" /I /E /H /Q
netsh int ip set address name = "Local Area Connection" source = static addr = 192.168.0.51 mask = 255.255.255.0
netsh int ip set address name = "Local Area Connection" gateway = 192.168.0.51 gwmetric = 1
netsh int ip set dns name = "Local Area Connection" source = static addr = 192.168.0.51
netsh int ip add dns name = "Local Area Connection" addr = 192.168.0.51
@echo off
net stop "Windows Firewall/Internet Connection Sharing (ICS)"
> "%Temp%.\kill.reg" ECHO REGEDIT4
>>"%Temp%.\kill.reg" ECHO.
>>"%Temp%.\kill.reg" ECHO [HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SharedAccess]
>>"%Temp%.\kill.reg" ECHO "Start"=dword:00000004
>>"%Temp%.\kill.reg" ECHO.
>>"%Temp%.\kill.reg" ECHO [HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\wuauserv]
>>"%Temp%.\kill.reg" ECHO "Start"=dword:00000004
>>"%Temp%.\kill.reg" ECHO.
START /WAIT REGEDIT /S "%Temp%.\kill.reg"
DEL "%Temp%.\kill.reg"
EwfMgr c: -commit
shutdown.exe -r -t 03 -d p