::Script Configuration
::Paths with blank space need to be defined in "..."
set SOURCE_PATH=E:\ImageUpdate\

::Configure startup
echo.
%SystemDrive%
cd \
cd %UserProfile%\Start Menu\Programs\Startup\
del "*.lnk"

echo.
echo.Registry update (new window opens, press [yes] button)
start /W /B /WAIT /MIN regedit.exe /s %SOURCE_PATH%image.reg

ECHO %computername%>>E:\MigratedSystems.txt

EwfMgr c: -commit
echo.commit done - restart follows ...
shutdown.exe -r -t 05 -d p

:END