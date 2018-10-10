:: Batch file having the task to update the updater.
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
:: so, here below I do the same (I'll ping localhost for 10 time, waiting max 1 sec for each ping).
ping 127.0.0.1 -n 5 -w 1000

:: 2)
:: the second thing to do is to make a back up of the actual updater
:: (by specification it is in "D:\Update").
:: here below I'll rename its directory with a name having a timestamp
:: and then I re-create the new one, ready to host the new files.
set originalName=D:\Update
set newName=Update
set bkpUpdateName=%newName%_%date:~-4,4%%date:~-7,2%%date:~-10,2%%time:~0,2%%time:~3,2%%time:~6,2%
set bkpUpdateAbsName=D:\%bkpUpdateName%
ren %originalName% %bkpUpdateName%

:: Attention:
:: do not change the parenthesis indentation used in the for loop above.
:: batch programming requires this indentation.
md %originalName%

:: 3)
:: now it's the time to fill the new directory created at the previous point with the new files.
:: as "the new files", I mean the set of binary files and config files that compose the new updater.
::
:: Starting after the arrows below, add all the files you need
:: 		||			||			||
::		\/			\/			\/
copy ..\Update\Gorba.Common.Configuration.dll %originalName%\Gorba.Common.Configuration.dll
copy ..\Update\Gorba.Motion.SystemManager.Update.Core.dll %originalName%\Gorba.Motion.SystemManager.Update.Core.dll
copy ..\Update\Gorba.Motion.SystemManager.Update.Core.XmlSerializers.dll %originalName%\Gorba.Motion.SystemManager.Update.Core.XmlSerializers.dll
copy ..\Update\NLog.config %originalName%\NLog.config
copy ..\Update\NLog.dll %originalName%\NLog.dll
copy ..\Update\Update.exe %originalName%\Update.exe
copy ..\Update\Update.xml %originalName%\Update.xml
:: do not add any other file after this line of code
::		/\			/\			/\
:: 		||			||			||

:: 4)
:: now it's the time to start SystemManager.
:: by specification, it's stored in the following directory:
set sysManagerName=SystemManager.exe
set sysManagerAbsDir=D:\Progs\SystemManager
set sysManagerAbsName=%sysManagerAbsDir%\%sysManagerName%

:: in order to start it properly, I've to give it its working directory.
:: to do this, I need to enter in its directory
set nextDrive=D:
%nextDrive%

cd %sysManagerAbsDir%
:: now, I sleep a little bit before really execute SystemManager
ping 127.0.0.1 -n 5 -w 1000

if exist %sysManagerAbsName% (
	START %sysManagerAbsName%
	rmdir /S /Q  %bkpUpdateAbsName%
)
if NOT exist %sysManagerAbsName% (
	rmdir /S /Q  %originalName%
	ren %bkpUpdateAbsName% %newName%
)
