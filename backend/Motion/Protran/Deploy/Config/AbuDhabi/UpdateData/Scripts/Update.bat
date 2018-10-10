:: Batch file having the task to move in C:\ the "UpdaterOfUpdater.bat"
:: and then execute it
:: 20 July 2012
:: Gorba AG,
:: Erlenstrasse 35
:: Bruegg 2555, Bern (Switzerland)

:: 1)
:: this batch file starts with a reasonable sleep
:: in order to be sure that no other update's relevant executables are running.
::
:: unfortunately, in Windows there's no proper way to sleep in a batch file.
:: the most used way to sleep is invoking the ping command,
:: so, here below I do the same (I'll ping localhost for 5 times, waiting max 1 sec for each ping).
ping 127.0.0.1 -n 5 -w 1000

:: now I want to understand which is the source
:: (a pen drive or the local directory in D:\ ?)
set isLocal="false"
set sourceDrive=%~d0
if /I %sourceDrive%==D: (
	set isLocal=true
)

:: 2)
:: I copy "UpdaterOfUpdater.bat" to C:\
set newRoot=C:
set updaterName=UpdaterOfUpdater.bat
set newUpdaterAbsName=%newRoot%\%updaterName%
set updaterOriginalAbsPath=%CD%\%updaterName%
copy %updaterName% %newUpdaterAbsName%

:: 3)
:: and now I execute it.
:: Attention !!!
:: Here I start the second batch file BUT without using the parameter /WAIT
:: That parameter causes problems in the TopBox (the shell is blocked and asks
:: the user a confirmation before exiting).
:: So, the easiest way to "understand" that the other batch file has finished its tasks
:: is to wait for a reasonable amount of time (again, make other ping commands).
if exist %newUpdaterAbsName% (
	START %newUpdaterAbsName%
	ping 127.0.0.1 -n 50 -w 1000

	:: 4)
	:: if I reach this line of code, it means that the second batch file
	:: has finished its tasks.
	:: it's the time now to delete it.
	if %isLocal%==true (
		DEL %updaterOriginalAbsPath%
	)
)

set sysManagerName=SystemManager.exe
set sysManagerAbsDir=D:\Progs\SystemManager
set sysManagerAbsName=%sysManagerAbsDir%\%sysManagerName%
if NOT exist %newUpdaterAbsName% (
	if exist %sysManagerAbsName% (
		cd %sysManagerAbsDir%
		START %sysManagerAbsName%
	)
)
