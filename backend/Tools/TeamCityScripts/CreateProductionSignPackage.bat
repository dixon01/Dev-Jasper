IF "%~1"=="" GOTO Master
IF "%~1"=="standalone" GOTO StandAlone
IF "%~1"=="master" GOTO Master
IF "%~1"=="slave" GOTO Slave
GOTO Master

REM --------------------------------------------------------------------------
REM TODO: Refactor the directory creation and file copying
:StandAlone
:Master

del /s /q DeployProduction
mkdir DeployProduction
Cd DeployProduction

mkdir Config
cd Config
mkdir AudioRenderer
mkdir Composer
mkdir DirectXRenderer
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

XCopy /c /s /e .\Tools\Support\OnBoot\*.* .\DeployProduction\Support\OnBoot\*.*
XCopy /c /s /e .\Tools\Sentinel\*.* .\DeployProduction\Support\Sentinel\*.*
XCopy /c /s /e .\Tools\Support\Luminator.Motion.WpfIntegratedTester\bin\Release\*.* .\DeployProduction\Support\AudioSwitchTool\*.*

XCopy /c /s /e .\Motion\HardwareManager\Source\ConsoleApp\bin\Release\*.* .\DeployProduction\Progs\HardwareManager\*.*
del /q .\DeployProduction\Progs\HardwareManager\*.pdb

XCopy /c /s /e .\Motion\Infomedia\Source\AudioRendererApp\bin\Release\*.* .\DeployProduction\Progs\AudioRenderer\*.*
del /q .\DeployProduction\Progs\AudioRenderer\*.pdb

Copy /y .\Motion\Infomedia\Source\ComposerApp\bin\Release\*.* .\DeployProduction\Progs\Composer\*.*
del /q .\DeployProduction\Progs\Composer\*.pdb

XCopy /c /s /e .\Motion\Infomedia\Source\DirectXRendererApp\bin\Release\*.* .\DeployProduction\Progs\DirectXRenderer\*.*
del /q .\DeployProduction\Progs\DirectXRenderer\*.pdb

XCopy /c /s /e .\Motion\Protran\Source\ConsoleApp\bin\Release\*.* .\DeployProduction\Progs\Protran\*.*
del /q .\DeployProduction\Progs\Protran\*.pdb

XCopy /c /s /e .\Motion\SystemManager\Source\ShellApp\bin\Release\*.* .\DeployProduction\Progs\SystemManager\*.*
del /q .\DeployProduction\Progs\SystemManager\*.pdb
XCopy /c /s /e /y .\Motion\SystemManager\Source\ConsoleApp\bin\Release\*.* .\DeployProduction\Progs\SystemManager\*.*
del /q .\DeployProduction\Progs\SystemManager\*.pdb

XCopy /c /s /e .\Motion\Update\Source\ConsoleApp\bin\Release\*.* .\DeployProduction\Progs\Update\*.*
del /q .\DeployProduction\Progs\Update\*.pdb

move /y .\DeployProduction\Progs\HardwareManager\Nlog.config .\DeployProduction\Config\HardwareManager\.
move /y .\DeployProduction\Progs\HardwareManager\Master\HardwareManager.xml .\DeployProduction\Config\HardwareManager\.
move /y .\DeployProduction\Progs\HardwareManager\Master\medi.config .\DeployProduction\Config\HardwareManager\.

move /y .\DeployProduction\Progs\AudioRenderer\Nlog.config .\DeployProduction\Config\AudioRenderer\.
move /y .\DeployProduction\Progs\AudioRenderer\AudioRenderer.xml .\DeployProduction\Config\AudioRenderer\.
move /y .\DeployProduction\Progs\AudioRenderer\medi.config .\DeployProduction\Config\AudioRenderer\.

move /y .\DeployProduction\Progs\Composer\Nlog.config .\DeployProduction\Config\Composer\.
move /y .\DeployProduction\Progs\Composer\Composer.xml .\DeployProduction\Config\Composer\.
move /y .\DeployProduction\Progs\Composer\medi.config .\DeployProduction\Config\Composer\.

move /y .\DeployProduction\Progs\DirectXRenderer\Nlog.config .\DeployProduction\Config\DirectXRenderer\.
move /y .\DeployProduction\Progs\DirectXRenderer\DirectXRenderer.xml .\DeployProduction\Config\DirectXRenderer\.
move /y .\DeployProduction\Progs\DirectXRenderer\medi.config .\DeployProduction\Config\DirectXRenderer\.

move /y .\DeployProduction\Progs\Protran\Nlog.config .\DeployProduction\Config\Protran\.
move /y .\DeployProduction\Progs\Protran\Protran.xml .\DeployProduction\Config\Protran\.
move /y .\DeployProduction\Progs\Protran\medi.config .\DeployProduction\Config\Protran\.
move /y .\DeployProduction\Progs\Protran\dictionary.xml .\DeployProduction\Config\Protran\.
move /y .\DeployProduction\Progs\Protran\gorba.xml .\DeployProduction\Config\Protran\.
move /y .\DeployProduction\Progs\Protran\io.xml .\DeployProduction\Config\Protran\.

move /y .\DeployProduction\Progs\SystemManager\Nlog.config .\DeployProduction\Config\SystemManager\.
move /y .\DeployProduction\Progs\SystemManager\Master\SystemManager.xml .\DeployProduction\Config\SystemManager\.
move /y .\DeployProduction\Progs\SystemManager\Master\medi.config .\DeployProduction\Config\SystemManager\.

move /y .\DeployProduction\Progs\Update\Nlog.config .\DeployProduction\Config\Update\.
move /y .\DeployProduction\Progs\Update\Master\Update.xml .\DeployProduction\Config\Update\.
move /y .\DeployProduction\Progs\Update\medi.config .\DeployProduction\Config\Update\.

copy .\Motion\Infomedia\Source\ComposerApp\main.im2 .\DeployProduction\Presentation\.
copy .\Motion\Infomedia\Source\ComposerApp\Audio\*.* .\DeployProduction\Presentation\Audio\.
copy .\Motion\Infomedia\Source\ComposerApp\Fonts\*.* .\DeployProduction\Presentation\Fonts\.
copy .\Motion\Infomedia\Source\ComposerApp\Images\*.* .\DeployProduction\Presentation\Images\.
xcopy  /c /s /e  .\Motion\Infomedia\Source\ComposerApp\Pools\*.* .\DeployProduction\Presentation\Pools\.
copy .\Motion\Infomedia\Source\ComposerApp\Videos\*.* .\DeployProduction\Presentation\Videos\.

cd DeployProduction

GOTO End

REM --------------------------------------------------------------------------
REM TODO: Refactor the directory creation and file copying
:Slave

del /s /q DeployProduction
mkdir DeployProduction
Cd DeployProduction

mkdir Config
cd Config
mkdir AudioRenderer
mkdir Composer
mkdir DirectXRenderer
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

XCopy /c /s /e .\Tools\Support\OnBoot\*.* .\DeployProduction\Support\OnBoot\*.*
XCopy /c /s /e .\Tools\Sentinel\*.* .\DeployProduction\Support\Sentinel\*.*
XCopy /c /s /e .\Tools\Support\Luminator.Motion.WpfIntegratedTester\bin\Release\*.* .\DeployProduction\Support\AudioSwitchTool\*.*

XCopy /c /s /e .\Motion\HardwareManager\Source\ConsoleApp\bin\Release\*.* .\DeployProduction\Progs\HardwareManager\*.*
del /q .\DeployProduction\Progs\HardwareManager\*.pdb

XCopy /c /s /e .\Motion\Infomedia\Source\DirectXRendererApp\bin\Release\*.* .\DeployProduction\Progs\DirectXRenderer\*.*
del /q .\DeployProduction\Progs\DirectXRenderer\*.pdb

XCopy /c /s /e .\Motion\SystemManager\Source\ShellApp\bin\Release\*.* .\DeployProduction\Progs\SystemManager\*.*
del /q .\DeployProduction\Progs\SystemManager\*.pdb
XCopy /c /s /e /y .\Motion\SystemManager\Source\ConsoleApp\bin\Release\*.* .\DeployProduction\Progs\SystemManager\*.*
del /q .\DeployProduction\Progs\SystemManager\*.pdb

XCopy /c /s /e .\Motion\Update\Source\ConsoleApp\bin\Release\*.* .\DeployProduction\Progs\Update\*.*
del /q .\DeployProduction\Progs\Update\*.pdb

move /y .\DeployProduction\Progs\HardwareManager\Nlog.config .\DeployProduction\Config\HardwareManager\.
move /y .\DeployProduction\Progs\HardwareManager\Slave\HardwareManagerSlave.xml .\DeployProduction\Config\HardwareManager\HardwareManager.xml
move /y .\DeployProduction\Progs\HardwareManager\Slave\medi.config .\DeployProduction\Config\HardwareManager\.

move /y .\DeployProduction\Progs\DirectXRenderer\Nlog.config .\DeployProduction\Config\DirectXRenderer\.
move /y .\DeployProduction\Progs\DirectXRenderer\DirectXRenderer.xml .\DeployProduction\Config\DirectXRenderer\.
move /y .\DeployProduction\Progs\DirectXRenderer\medi.config .\DeployProduction\Config\DirectXRenderer\.

move /y .\DeployProduction\Progs\SystemManager\Nlog.config .\DeployProduction\Config\SystemManager\.
move /y .\DeployProduction\Progs\SystemManager\Slave\SystemManagerSlave.xml .\DeployProduction\Config\SystemManager\SystemManager.xml
move /y .\DeployProduction\Progs\SystemManager\Slave\mediSlave.config .\DeployProduction\Config\SystemManager\medi.config

move /y .\DeployProduction\Progs\Update\Nlog.config .\DeployProduction\Config\Update\.
move /y .\DeployProduction\Progs\Update\Slave\Update.xml .\DeployProduction\Config\Update\.
move /y .\DeployProduction\Progs\Update\medi.config .\DeployProduction\Config\Update\.

copy .\Motion\Infomedia\Source\ComposerApp\main.im2 .\DeployProduction\Presentation\.
copy .\Motion\Infomedia\Source\ComposerApp\Audio\*.* .\DeployProduction\Presentation\Audio\.
copy .\Motion\Infomedia\Source\ComposerApp\Fonts\*.* .\DeployProduction\Presentation\Fonts\.
copy .\Motion\Infomedia\Source\ComposerApp\Images\*.* .\DeployProduction\Presentation\Images\.
xcopy  /c /s /e  .\Motion\Infomedia\Source\ComposerApp\Pools\*.* .\DeployProduction\Presentation\Pools\.
copy .\Motion\Infomedia\Source\ComposerApp\Videos\*.* .\DeployProduction\Presentation\Videos\.

cd DeployProduction

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