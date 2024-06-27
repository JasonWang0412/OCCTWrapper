@echo off

:: Created: 2011-04-27

:: Copyright (c) 2011-2021 OPEN CASCADE SAS

:: This file is part of commercial software by OPEN CASCADE SAS.

:: This software is furnished in accordance with the terms and conditions
:: of the contract and with the inclusion of this copyright notice.
:: This software or any other copy thereof may not be provided or otherwise
:: be made available to any third party.

:: No ownership title to the software is transferred hereby.

:: OPEN CASCADE SAS makes no representation or warranties with respect to the
:: performance of this software, and specifically disclaims any responsibility
:: for any damages, special or consequential, connected with its use.

:: Prepare delivery archives of CSharp sample (sources and binaries);
:: note that content of subfolder ../swig is archived to OCCwrapCSharp!
:: The script requires zip accessible through the PATH.

call "%~dp0/env.bat"

:: Get current date in format YYYY-MM-DD
setlocal enableDelayedExpansion
if %1. == . (
  for /F "usebackq tokens=1,2 delims==" %%i in (`wmic os get LocalDateTime /VALUE 2^>NUL`) do if '.%%i.'=='.LocalDateTime.' set ldt=%%j
  set da=!ldt:~0,4!-!ldt:~4,2!-!ldt:~6,2!
) else set da=%1
endlocal & set da=%da%

:: silence Cygwin warning on DOS paths
set CYGWIN=nodosfilewarning

mkdir "%~dp0\delivery" >NUL 2>&1

:: Clean delivery folder
if exist %~dp0delivery\OCCwrapCSharp ( rd /s /q %~dp0delivery\OCCwrapCSharp >NUL 2>&1 )

set "binarchive=%~dp0delivery\OCCCSharp-win%ARCH%-%VCVER%-%da%.zip"
set "srcarchive=%~dp0delivery\OCCCSharp-src-%da%.zip"
if exist %binarchive% del /f /q %binarchive%
if exist %srcarchive% del /f /q %srcarchive%

cd %~dp0..

rem echo Binaries are packed to %binarchive%...
rem zip -rq -D -x mk*.bat msvc.bat @ %binarchive% win%ARCH%/%VCVER%/bin csharp/*.bat

echo Sources are packed to %srcarchive%...
zip -rq -D -x */mk*.bat *.log */*.vcproj.*.user *.old *.sdf */obj/* */obj/*/* */obj/*/*/* */obj/*/*/*/* */obj/*/*/*/*/* */bin/* */bin/*/* */bin/*/*/* */bin/*/*/*/* */bin/*/*/*/*/* @ %srcarchive% csharp/CSharpTest csharp/ImportExport* csharp/OCC* csharp/*.sln csharp/*.txt csharp/*.bat swig

cd "%~dp0"
