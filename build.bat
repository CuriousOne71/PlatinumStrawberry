@echo off

set fwdir="%WINDIR%\Microsoft.NET\Framework\v4.0.30319"
set msbuilddir="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin"

set project="PlatinumStrawberry.sln"

set celestedir="C:\Program Files (x86)\Steam\steamapps\common\Celeste\Mods\PlatinumStrawberry"
set outputdir=".\bin\Debug\net46"

IF "%1" == "" GOTO start
set propfile=%1

:start
if not exist "%propfile%" echo. > "%propfile%"

%msbuilddir%\msbuild.exe %project%

xcopy %outputdir%\*.* %celestedir% /E/H/C/I/Y