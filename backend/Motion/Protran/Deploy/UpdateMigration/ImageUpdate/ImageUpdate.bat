::Script Configuration
::Paths with blank space need to be defined in "..."
set SOURCE_PATH=E:\ImageUpdate\
set SOURCE_PROG_PATH=E:\ImageUpdate\PROGS\
set PROG_PATH=D:\PROGS\
set INFOMEDIA_PATH=D:\INFOMEDIA\
set SOURCE_INFOMEDIA_PATH=E:\ImageUpdate\INFOMEDIA\
set STARTUP_PATH="C:\Documents and Settings\infomedia\Start Menu\Programs\Startup\"
set DOT_NET_INSTALLER="dotnetfx.exe"
set SOURCE_COMMIT_PATH="E:\ImageUpdate\COMMIT\"
set COMMIT_PATH="C:\WINEMB21\system32\"
set DB_PATH=D:\IMOTION\


cls
@echo OFF
echo.Image Update Script Arriva
echo.__________________________
echo.


::Delete SM database
del %DB_PATH% /s /q /f
rmdir %DB_PATH% /s /q


::Replace commit.bat
xcopy /y /E /K /S %SOURCE_COMMIT_PATH%* %COMMIT_PATH%

::Update registry
echo.
echo.Registry update (new window opens, press [yes] button)
start /W /B /WAIT /MIN regedit.exe /s %SOURCE_PATH%image.reg


del %INFOMEDIA_PATH% /s /q
rmdir %INFOMEDIA_PATH% /s /q
del %PROG_PATH% /s /q
rmdir %PROG_PATH% /s /q


::Create Directories
mkdir %PROG_PATH%
mkdir %INFOMEDIA_PATH%


::Copy Files
echo.
echo.Copy all files to %PROG_PATH% (overwrites existing files)
xcopy /y /E /K /S %SOURCE_PROG_PATH%* %PROG_PATH%
xcopy /y /E /K /S %SOURCE_INFOMEDIA_PATH%* %INFOMEDIA_PATH%

netsh int ip set address name = "Local Area Connection" source = static addr = 10.0.20.70 mask = 255.255.255.0
netsh int ip set address name = "Local Area Connection" gateway = 10.0.20.254 gwmetric = 1
netsh int ip set dns name = "Local Area Connection" source = static addr = 10.0.20.254
netsh int ip add dns name = "Local Area Connection" addr = 10.0.20.254
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

SHUTDOWN -r -t 10
:End