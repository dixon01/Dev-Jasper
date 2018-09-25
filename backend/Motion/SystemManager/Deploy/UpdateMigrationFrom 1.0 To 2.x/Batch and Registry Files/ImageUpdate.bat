::Script Configuration
::Paths with blank space need to be defined in "..."
set SOURCE_PATH=E:\ImageUpdate\
set INFOMEDIA_PATH=D:\INFOMEDIA\
set SOURCE_PROG_PATH=E:\ImageUpdate\Progs
set PROG_PATH=D:\Progs\
set SOURCE_CONFIG_PATH=E:\ImageUpdate\Config
set CONFIG_PATH=D:\Config\
set STARTUP_PATH="C:\Documents and Settings\infomedia\Start Menu\Programs\Startup\"
set D_DRIVE_PATH=D:\
set TASKILL_PATH="C:\WINEMB21\system32\"
cls
@echo OFF
echo.Image Update Script Update 1.0 or 1.2 to Update 2.2
echo.__________________________
echo.

set Processor_Identifier
if NOT "%Processor_Identifier%"=="%Processor_Identifier:Model 28 Stepping 2, GenuineIntel=%" (
goto :StartInstall
) else (
goto :DoNotInstall
)

:StartInstall
TASKKILL /F /IM TftUpdate.exe

::Configure startup
echo.
%SystemDrive%
cd \
cd %UserProfile%\Start Menu\Programs\Startup\
del "*.lnk"
echo.COPY startup LINK
copy /V /Y E:\ImageUpdate\*.lnk %STARTUP_PATH%

::Delete everything under D drive
del %INFOMEDIA_PATH% /s /q
rmdir %INFOMEDIA_PATH% /s /q
del %PROG_PATH% /s /q
rmdir %PROG_PATH% /s /q

del %D_DRIVE_PATH% /s /q /f
rmdir %D_DRIVE_PATH% /s /q

::Create Directories
mkdir %PROG_PATH%
mkdir %CONFIG_PATH%

::Copy Files
echo.
echo.Copy all files to %PROG_PATH%
xcopy %SOURCE_PROG_PATH% %PROG_PATH% /y /E /K /S
xcopy %SOURCE_CONFIG_PATH% %CONFIG_PATH% /y /E /K /S

EwfMgr c: -commit
echo.commit done - restart follows ...
set sysManagerName=SystemManagerShell.exe
set sysManagerAbsDir=D:\Progs\SystemManager
set sysManagerAbsName=%sysManagerAbsDir%\%sysManagerName%
start %sysManagerAbsName%
shutdown.exe -r -t 60 -d p
goto End

:DoNotInstall
echo.Install not intended for this hardware! Please remove the USB stick.
set /P DoCommit=Restart the device [y/n], (type "y" to restart device)? : 
if /i %DoCommit%==y goto DoCommit
goto End
:DoCommit
shutdown.exe -r -t 05 -d p
:End