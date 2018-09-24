@echo OFF
REM Make a backup of D:\Config, D:\Progs, D:\Resources, D:\Presentation and D:\Data
set host=%COMPUTERNAME%
echo %host%
mkdir %host%BackUp
cd %host%BackUp
mkdir Config
mkdir Progs
mkdir Presentation
mkdir Data
mkdir Resources
XCopy /c /s /e  d:\Config\*.* .\Config\.
XCopy /c /s /e  d:\Progs\*.* .\Progs\.
XCopy /c /s /e  d:\Presentation\*.* .\Presentation\.
XCopy /c /s /e  d:\Data\*.* .\Data\.
XCopy /c /s /e  d:\Resources\*.* .\Resources\.

cd..

7z.exe e TFTUpdate.zip  -y -spf

XCopy /c /s /e /y Config\*.* d:\Config\.
XCopy /c /s /e /y Progs\*.* d:\Progs\.
XCopy /c /s /e /y Presentation\*.* d:\Presentation\.

rd  /s /q .\Config
rd  /s /q .\Progs
rd  /s /q .\Presentation





color 1D
	echo.
	echo  ‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹
	echo  ›                                              ›  
	echo  ›  Please Remove the USB Stick and Hit Enter   ›  
	echo  ›                                              ›    
	echo. ﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂ
	echo.
	echo.
	pause





