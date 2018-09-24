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

ping localhost -n 5 >NULL

::Configure startup
echo.
%SystemDrive%
cd \
cd %UserProfile%\Start Menu\Programs\Startup\
del *.* /s /q /f
echo.COPY startup LINK
copy %SOURCE_PATH%TFTStartup.lnk %STARTUP_PATH%

::Delete SM database
del %DB_PATH% /s /q /f
rmdir %DB_PATH% /s /q

::Replace commit.bat
rem not necessary anymore (done directly with ewfmanager)
rem xcopy /y /E /K /S %SOURCE_COMMIT_PATH%* %COMMIT_PATH%

::Update registry
echo.
echo.Registry update (press Yes if necessary)
start /W /B /WAIT /MIN regedit.exe /s %SOURCE_PATH%image.reg

del %INFOMEDIA_PATH% /s /q /f
rmdir %INFOMEDIA_PATH% /s /q
del %PROG_PATH% /s /q /f
rmdir %PROG_PATH% /s /q

::Create Directories
mkdir %PROG_PATH%
mkdir %INFOMEDIA_PATH%

::Copy Files
echo.
echo.Copy all files to %PROG_PATH% (overwrites existing files)
xcopy /y /E /K /S %SOURCE_PROG_PATH%* %PROG_PATH%
xcopy /y /E /K /S %SOURCE_INFOMEDIA_PATH%* %INFOMEDIA_PATH%

:DoCommit
EwfMgr c: -commit
echo.commit done - restart follows ...
shutdown.exe -r -t 05 -d p

:End