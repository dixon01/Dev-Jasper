::Script configuration
set UPDATESCRIPT=E:\ImageUpdate\ImageUpdate.bat
set STARTUP=D:\PROGS\SystemManager\SystemManager.exe

::Clear Screen
cls
@echo OFF

:: Start with parameter n to skip update: callback from update
if "%1" == "n" goto NoUpdate

:: Execute updates if available
if not exist %UPDATESCRIPT% goto NoUpdate
echo.Executing update script on external drive . . .

start /B /MIN %UPDATESCRIPT%
Goto END

:NoUpdate
::START TFT Update if installed
if not exist %STARTUP% goto END
echo.Starting %STARTUP% . . .
start /B /MIN %STARTUP%

:END