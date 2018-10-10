set delpdb=""
set target=%2

Rem working folder set to....

rem cd "C:\Development\TFTDisplayReleaseBuild"

If "%target%"=="" set target="Release"
If "%~3"=="DelPdb" set delpdb="true"

IF "%~1"=="" GOTO Master
IF "%~1"=="standalone" GOTO StandAlone
IF "%~1"=="master" GOTO Master
IF "%~1"=="slave" GOTO Slave

GOTO Master

REM --------------------------------------------------------------------------
REM TODO: Refactor the directory creation and file copying
:StandAlone

del /s /q Deploy
mkdir Deploy
Cd Deploy

mkdir Config
cd Config
mkdir AudioRenderer
mkdir Composer
mkdir DirectXRenderer
mkdir Dimmer
mkdir HardwareManager
mkdir Protran
mkdir SystemManager
mkdir Update

cd ..

mkdir Progs
cd Progs
mkdir AudioRenderer
mkdir Composer
mkdir DirectXRenderer
mkdir Dimmer
mkdir HardwareManager
mkdir Protran
mkdir SystemManager
mkdir Update

cd ..

mkdir Data
cd Data
mkdir AudioRenderer
mkdir Composer
mkdir DirectXRenderer
mkdir HardwareManager
mkdir Protran
mkdir SystemManager
mkdir Update

cd ..

mkdir Presentation
cd Presentation
mkdir Audio
mkdir Fonts
mkdir Images
mkdir Pools
mkdir Videos
cd ..

mkdir Support
cd Support
mkdir AudioSwitchTool
mkdir Sentinel
mkdir OnBoot
cd ..

mkdir Resources 
cd ..

XCopy /c /s /e .\Tools\Support\OnBoot\*.* .\Deploy\Support\OnBoot\*.*
XCopy /c /s /e .\Tools\Sentinel\*.* .\Deploy\Support\Sentinel\*.*
XCopy /c /s /e .\Tools\Support\Luminator.Motion.WpfIntegratedTester\bin\%target%\*.* .\Deploy\Support\AudioSwitchTool\*.*

XCopy /c .\Motion\HardwareManager\Source\ConsoleApp\bin\%target%\*.* .\Deploy\Progs\HardwareManager\*.*
if "%delpdb%"=="true" del /q .\Deploy\Progs\HardwareManager\*.pdb
XCopy /c .\Motion\Infomedia\Source\AudioRendererApp\bin\%target%\*.* .\Deploy\Progs\AudioRenderer\*.*
if "%delpdb%"=="true" del /q .\Deploy\Progs\AudioRenderer\*.pdb
Copy /y .\Motion\Infomedia\Source\ComposerApp\bin\%target%\*.* .\Deploy\Progs\Composer\*.*
if "%delpdb%"=="true" del /q .\Deploy\Progs\Composer\*.pdb
XCopy /c .\Motion\Infomedia\Source\DirectXRendererApp\bin\%target%\*.* .\Deploy\Progs\DirectXRenderer\*.*
if "%delpdb%"=="true" del /q .\Deploy\Progs\DirectXRenderer\*.pdb

XCopy /c .\Motion\Dimmer\Source\PeripheralDimmerProtocol\Luminator.PeripheralDimmer.Console\bin\%target%\*.* .\Deploy\Progs\Dimmer\*.*
if "%delpdb%"=="true" del /q .\Deploy\Progs\Dimmer\*.pdb

XCopy /c .\Motion\Protran\Source\ConsoleApp\bin\%target%\*.* .\Deploy\Progs\Protran\*.*
if "%delpdb%"=="true" del /q .\Deploy\Progs\Protran\*.pdb

XCopy /c .\Motion\SystemManager\Source\ShellApp\bin\%target%\*.* .\Deploy\Progs\SystemManager\*.*
if "%delpdb%"=="true" del /q .\Deploy\Progs\SystemManager\*.pdb
XCopy /c /y .\Motion\SystemManager\Source\ConsoleApp\bin\%target%\*.* .\Deploy\Progs\SystemManager\*.*
if "%delpdb%"=="true" del /q .\Deploy\Progs\SystemManager\*.pdb
XCopy /c .\Motion\Update\Source\ConsoleApp\bin\%target%\*.* .\Deploy\Progs\Update\*.*
if "%delpdb%"=="true" del /q .\Deploy\Progs\Update\*.pdb

move /y .\Deploy\Progs\HardwareManager\Nlog.config .\Deploy\Config\HardwareManager\.
move /y .\Deploy\Progs\HardwareManager\HardwareManager.xml .\Deploy\Config\HardwareManager\.
move /y .\Deploy\Progs\HardwareManager\medi.config .\Deploy\Config\HardwareManager\.

move /y .\Deploy\Progs\AudioRenderer\Nlog.config .\Deploy\Config\AudioRenderer\.
move /y .\Deploy\Progs\AudioRenderer\AudioRenderer.xml .\Deploy\Config\AudioRenderer\.
move /y .\Deploy\Progs\AudioRenderer\medi.config .\Deploy\Config\AudioRenderer\.

move /y .\Deploy\Progs\Composer\Nlog.config .\Deploy\Config\Composer\.
move /y .\Deploy\Progs\Composer\Composer.xml .\Deploy\Config\Composer\.
move /y .\Deploy\Progs\Composer\medi.config .\Deploy\Config\Composer\.

move /y .\Deploy\Progs\DirectXRenderer\Nlog.config .\Deploy\Config\DirectXRenderer\.
move /y .\Deploy\Progs\DirectXRenderer\DirectXRenderer.xml .\Deploy\Config\DirectXRenderer\.
move /y .\Deploy\Progs\DirectXRenderer\medi.config .\Deploy\Config\DirectXRenderer\.

move /y .\Deploy\Progs\Protran\Nlog.config .\Deploy\Config\Protran\.
move /y .\Deploy\Progs\Protran\Protran.xml .\Deploy\Config\Protran\.
move /y .\Deploy\Progs\Protran\medi.config .\Deploy\Config\Protran\.

move /y .\Deploy\Progs\SystemManager\Nlog.config .\Deploy\Config\SystemManager\.
move /y .\Deploy\Progs\SystemManager\SystemManager.xml .\Deploy\Config\SystemManager\.
move /y .\Deploy\Progs\SystemManager\medi.config .\Deploy\Config\SystemManager\.

move /y .\Deploy\Progs\Update\Nlog.config .\Deploy\Config\Update\.
move /y .\Deploy\Progs\Update\Update.xml .\Deploy\Config\Update\.
move /y .\Deploy\Progs\Update\medi.config .\Deploy\Config\Update\.

copy .\Motion\Infomedia\Source\ComposerApp\main.im2 .\Deploy\Presentation\.
copy .\Motion\Infomedia\Source\ComposerApp\Audio\*.* .\Deploy\Presentation\Audio\.
copy .\Motion\Infomedia\Source\ComposerApp\Fonts\*.* .\Deploy\Presentation\Fonts\.
copy .\Motion\Infomedia\Source\ComposerApp\Images\*.* .\Deploy\Presentation\Images\.
xcopy /c /s /e  .\Motion\Infomedia\Source\ComposerApp\Pools\*.* .\Deploy\Presentation\Pools\.
copy .\Motion\Infomedia\Source\ComposerApp\Videos\*.* .\Deploy\Presentation\Videos\.

cd Deploy

GOTO End

REM --------------------------------------------------------------------------
REM TODO: Refactor the directory creation and file copying
:Master

del /s /q DeployMaster
mkdir DeployMaster
Cd DeployMaster

mkdir Config
cd Config
mkdir AudioRenderer
mkdir Composer
mkdir DirectXRenderer
mkdir Dimmer
mkdir HardwareManager
mkdir Protran
mkdir SystemManager
mkdir Update

cd ..

mkdir Progs
cd Progs
mkdir AudioRenderer
mkdir Composer
mkdir DirectXRenderer
mkdir Dimmer
mkdir HardwareManager
mkdir Protran
mkdir SystemManager
mkdir Update

cd ..

mkdir Data
cd Data
mkdir AudioRenderer
mkdir Composer
mkdir DirectXRenderer
mkdir HardwareManager
mkdir Protran
mkdir SystemManager
mkdir Update

cd ..

mkdir Presentation
cd Presentation
mkdir Audio
mkdir Fonts
mkdir Images
mkdir Pools
mkdir Videos
cd ..

mkdir Support
cd Support
mkdir AudioSwitchTool
mkdir Sentinel
mkdir OnBoot
cd ..

mkdir Resources 
cd ..

XCopy /c /s /e .\Tools\Support\OnBoot\*.* .\DeployMaster\Support\OnBoot\*.*
XCopy /c /s /e .\Tools\Sentinel\*.* .\DeployMaster\Support\Sentinel\*.*
XCopy /c /s /e .\Tools\Support\Luminator.Motion.WpfIntegratedTester\bin\%target%\*.* .\DeployMaster\Support\AudioSwitchTool\*.*

XCopy /c /s /e .\Motion\HardwareManager\Source\ConsoleApp\bin\%target%\*.* .\DeployMaster\Progs\HardwareManager\*.*
if "%delpdb%"=="true" del /q .\DeployMaster\Progs\HardwareManager\*.pdb
XCopy /c /s /e .\Motion\Infomedia\Source\AudioRendererApp\bin\%target%\*.* .\DeployMaster\Progs\AudioRenderer\*.*
if "%delpdb%"=="true" del /q .\DeployMaster\Progs\AudioRenderer\*.pdb
Copy /y .\Motion\Infomedia\Source\ComposerApp\bin\%target%\*.* .\DeployMaster\Progs\Composer\*.*
if "%delpdb%"=="true" del /q .\DeployMaster\Progs\Composer\*.pdb
XCopy /c /s /e .\Motion\Infomedia\Source\DirectXRendererApp\bin\%target%\*.* .\DeployMaster\Progs\DirectXRenderer\*.*
if "%delpdb%"=="true" del /q .\DeployMaster\Progs\DirectXRenderer\*.pdb

XCopy /c /s /e .\Motion\Dimmer\Source\PeripheralDimmerProtocol\Luminator.PeripheralDimmer.Console\bin\%target%\*.* .\DeployMaster\Progs\Dimmer\*.*
if "%delpdb%"=="true" del /q .\DeployMaster\Progs\Dimmer\*.pdb

XCopy /c /s /e .\Motion\Protran\Source\ConsoleApp\bin\%target%\*.* .\DeployMaster\Progs\Protran\*.*
if "%delpdb%"=="true" del /q .\DeployMaster\Progs\Protran\*.pdb

XCopy /c /s /e .\Motion\SystemManager\Source\ShellApp\bin\%target%\*.* .\DeployMaster\Progs\SystemManager\*.*
if "%delpdb%"=="true" del /q .\DeployMaster\Progs\SystemManager\*.pdb
XCopy /c /s /e /y .\Motion\SystemManager\Source\ConsoleApp\bin\%target%\*.* .\DeployMaster\Progs\SystemManager\*.*
if "%delpdb%"=="true" del /q .\DeployMaster\Progs\SystemManager\*.pdb
XCopy /c /s /e .\Motion\Update\Source\ConsoleApp\bin\%target%\*.* .\DeployMaster\Progs\Update\*.*
if "%delpdb%"=="true" del /q .\DeployMaster\Progs\Update\*.pdb

move /y .\DeployMaster\Progs\HardwareManager\Nlog.config .\DeployMaster\Config\HardwareManager\.
move /y .\DeployMaster\Progs\HardwareManager\Master\HardwareManager.xml .\DeployMaster\Config\HardwareManager\.
move /y .\DeployMaster\Progs\HardwareManager\Master\medi.config .\DeployMaster\Config\HardwareManager\.

move /y .\DeployMaster\Progs\AudioRenderer\Nlog.config .\DeployMaster\Config\AudioRenderer\.
move /y .\DeployMaster\Progs\AudioRenderer\AudioRenderer.xml .\DeployMaster\Config\AudioRenderer\.
move /y .\DeployMaster\Progs\AudioRenderer\medi.config .\DeployMaster\Config\AudioRenderer\.

move /y .\DeployMaster\Progs\Composer\Nlog.config .\DeployMaster\Config\Composer\.
move /y .\DeployMaster\Progs\Composer\Composer.xml .\DeployMaster\Config\Composer\.
move /y .\DeployMaster\Progs\Composer\medi.config .\DeployMaster\Config\Composer\.

move /y .\DeployMaster\Progs\DirectXRenderer\Nlog.config .\DeployMaster\Config\DirectXRenderer\.
move /y .\DeployMaster\Progs\DirectXRenderer\DirectXRenderer.xml .\DeployMaster\Config\DirectXRenderer\.
move /y .\DeployMaster\Progs\DirectXRenderer\medi.config .\DeployMaster\Config\DirectXRenderer\.

move /y .\DeployMaster\Progs\Protran\Nlog.config .\DeployMaster\Config\Protran\.
move /y .\DeployMaster\Progs\Protran\Protran.xml .\DeployMaster\Config\Protran\.
move /y .\DeployMaster\Progs\Protran\medi.config .\DeployMaster\Config\Protran\.
move /y .\DeployMaster\Progs\Protran\dictionary.xml .\DeployMaster\Config\Protran\.
move /y .\DeployMaster\Progs\Protran\gorba.xml .\DeployMaster\Config\Protran\.
move /y .\DeployMaster\Progs\Protran\io.xml .\DeployMaster\Config\Protran\.

move /y .\DeployMaster\Progs\SystemManager\Nlog.config .\DeployMaster\Config\SystemManager\.
move /y .\DeployMaster\Progs\SystemManager\Master\SystemManager.xml .\DeployMaster\Config\SystemManager\.
move /y .\DeployMaster\Progs\SystemManager\Master\medi.config .\DeployMaster\Config\SystemManager\.

move /y .\DeployMaster\Progs\Update\Nlog.config .\DeployMaster\Config\Update\.
move /y .\DeployMaster\Progs\Update\Master\Update.xml .\DeployMaster\Config\Update\.
move /y .\DeployMaster\Progs\Update\medi.config .\DeployMaster\Config\Update\.

copy .\Motion\Infomedia\Source\ComposerApp\main.im2 .\DeployMaster\Presentation\.
copy .\Motion\Infomedia\Source\ComposerApp\Audio\*.* .\DeployMaster\Presentation\Audio\.
copy .\Motion\Infomedia\Source\ComposerApp\Fonts\*.* .\DeployMaster\Presentation\Fonts\.
copy .\Motion\Infomedia\Source\ComposerApp\Images\*.* .\DeployMaster\Presentation\Images\.
xcopy /c /s /e  .\Motion\Infomedia\Source\ComposerApp\Pools\*.* .\DeployMaster\Presentation\Pools\.
copy .\Motion\Infomedia\Source\ComposerApp\Videos\*.* .\DeployMaster\Presentation\Videos\.

cd DeployMaster

GOTO End

REM --------------------------------------------------------------------------
REM TODO: Refactor the directory creation and file copying
:Slave

del /s /q DeploySlave
mkdir DeploySlave
Cd DeploySlave

mkdir Config
cd Config
mkdir AudioRenderer
mkdir Composer
mkdir DirectXRenderer
mkdir Dimmer
mkdir HardwareManager
mkdir Protran
mkdir SystemManager
mkdir Update

cd ..

mkdir Progs
cd Progs
mkdir AudioRenderer
mkdir Composer
mkdir DirectXRenderer
mkdir Dimmer
mkdir HardwareManager
mkdir Protran
mkdir SystemManager
mkdir Update

cd ..

mkdir Data
cd Data
mkdir AudioRenderer
mkdir Composer
mkdir DirectXRenderer
mkdir HardwareManager
mkdir Protran
mkdir SystemManager
mkdir Update

cd ..

mkdir Presentation
cd Presentation
mkdir Audio
mkdir Fonts
mkdir Images
mkdir Pools
mkdir Videos
cd ..

mkdir Support
cd Support
mkdir AudioSwitchTool
mkdir Sentinel
mkdir OnBoot
cd ..

mkdir Resources 
cd ..

XCopy /c /s /e .\Tools\Support\OnBoot\*.* .\DeploySlave\Support\OnBoot\*.*
XCopy /c /s /e .\Tools\Sentinel\*.* .\DeploySlave\Support\Sentinel\*.*
XCopy /c /s /e .\Tools\Support\Luminator.Motion.WpfIntegratedTester\bin\%target%\*.* .\DeploySlave\Support\AudioSwitchTool\*.*

XCopy /c /s /e .\Motion\HardwareManager\Source\ConsoleApp\bin\%target%\*.* .\DeploySlave\Progs\HardwareManager\*.*
if "%delpdb%"=="true" del /q .\DeploySlave\Progs\HardwareManager\*.pdb

XCopy /c /s /e .\Motion\Infomedia\Source\DirectXRendererApp\bin\%target%\*.* .\DeploySlave\Progs\DirectXRenderer\*.*
if "%delpdb%"=="true" del /q .\DeploySlave\Progs\DirectXRenderer\*.pdb

XCopy /c /s /e .\Motion\Dimmer\Source\PeripheralDimmerProtocol\Luminator.PeripheralDimmer.Console\bin\%target%\*.* .\DeploySlave\Progs\Dimmer\*.*
if "%delpdb%"=="true" del /q .\DeploySlave\Progs\Dimmer\*.pdb

XCopy /c /s /e .\Motion\SystemManager\Source\ShellApp\bin\%target%\*.* .\DeploySlave\Progs\SystemManager\*.*
if "%delpdb%"=="true" del /q .\DeploySlave\Progs\SystemManager\*.pdb
XCopy /c /s /e /y .\Motion\SystemManager\Source\ConsoleApp\bin\%target%\*.* .\DeploySlave\Progs\SystemManager\*.*
if "%delpdb%"=="true" del /q .\DeploySlave\Progs\SystemManager\*.pdb

XCopy /c /s /e .\Motion\Update\Source\ConsoleApp\bin\%target%\*.* .\DeploySlave\Progs\Update\*.*
if "%delpdb%"=="true" del /q .\DeploySlave\Progs\Update\*.pdb

move /y .\DeploySlave\Progs\HardwareManager\Nlog.config .\DeploySlave\Config\HardwareManager\.
move /y .\DeploySlave\Progs\HardwareManager\Slave\HardwareManager.xml .\DeploySlave\Config\HardwareManager\.
move /y .\DeploySlave\Progs\HardwareManager\Slave\medi.config .\DeploySlave\Config\HardwareManager\.

move /y .\DeploySlave\Progs\DirectXRenderer\Nlog.config .\DeploySlave\Config\DirectXRenderer\.
move /y .\DeploySlave\Progs\DirectXRenderer\DirectXRenderer.xml .\DeploySlave\Config\DirectXRenderer\.
move /y .\DeploySlave\Progs\DirectXRenderer\medi.config .\DeploySlave\Config\DirectXRenderer\.

move /y .\DeploySlave\Progs\SystemManager\Nlog.config .\DeploySlave\Config\SystemManager\.
move /y .\DeploySlave\Progs\SystemManager\Slave\SystemManager.xml .\DeploySlave\Config\SystemManager\.
move /y .\DeploySlave\Progs\SystemManager\Slave\medi.config .\DeploySlave\Config\SystemManager\.

move /y .\DeploySlave\Progs\Update\Nlog.config .\DeploySlave\Config\Update\.
move /y .\DeploySlave\Progs\Update\Slave\Update.xml .\DeploySlave\Config\Update\.
move /y .\DeploySlave\Progs\Update\medi.config .\DeploySlave\Config\Update\.

copy .\Motion\Infomedia\Source\ComposerApp\main.im2 .\DeploySlave\Presentation\.
copy .\Motion\Infomedia\Source\ComposerApp\Audio\*.* .\DeploySlave\Presentation\Audio\.
copy .\Motion\Infomedia\Source\ComposerApp\Fonts\*.* .\DeploySlave\Presentation\Fonts\.
copy .\Motion\Infomedia\Source\ComposerApp\Images\*.* .\DeploySlave\Presentation\Images\.
xcopy  /c /s /e  .\Motion\Infomedia\Source\ComposerApp\Pools\*.* .\DeploySlave\Presentation\Pools\.
copy .\Motion\Infomedia\Source\ComposerApp\Videos\*.* .\DeploySlave\Presentation\Videos\.

cd DeploySlave

GOTO End

REM --------------------------------------------------------------------------
:End

del /q .\Progs\HardwareManager\HardwareManager.xml
del /q .\Progs\HardwareManager\medi.config
del /q .\Progs\HardwareManager\NLog.config
del /q .\Progs\AudioRenderer\AudioRenderer.xml
del /q .\Progs\AudioRenderer\medi.config
del /q .\Progs\AudioRenderer\NLog.config
del /q .\Progs\Composer\Composer.xml
del /q .\Progs\Composer\medi.config
del /q .\Progs\Composer\NLog.config
del /q .\Progs\DirectXRenderer\DirectXRenderer.xml
del /q .\Progs\DirectXRenderer\medi.config
del /q .\Progs\DirectXRenderer\NLog.config
del /q .\Progs\Protran\dictionary.xml
del /q .\Progs\Protran\gorba.xml
del /q .\Progs\Protran\io.xml
del /q .\Progs\Protran\protran.xml
del /q .\Progs\Protran\medi.config
del /q .\Progs\Protran\NLog.config
del /q .\Progs\SystemManager\SystemManager.xml
del /q .\Progs\SystemManager\medi.config
del /q .\Progs\SystemManager\NLog.config
del /q .\Progs\Update\Update.xml
del /q .\Progs\Update\SystemManager.xml
del /q .\Progs\Update\medi.config
del /q .\Progs\Update\NLog.config

cd ..