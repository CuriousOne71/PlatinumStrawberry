@echo off

rem set propfile=%USERPROFILE%\web.build.rsp

set fwdir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319
set msbuilddir="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin"

rem set project=PlatinumStrawberry.csproj
set project=PlatinumStrawberry.sln

set celestedir="C:\Program Files (x86)\Steam\steamapps\common\Celeste\Mods\PlatinumStrawberry"
set outputdir=.\bin\Debug\net46

IF "%1" == "" GOTO start
set propfile=%1

:start
if not exist "%propfile%" echo. > "%propfile%"

rem %WINDIR%\Microsoft.NET\Framework\v3.5\msbuild.exe build.proj @"%propfile%" /t:Package /v:d /fl

%msbuilddir%\msbuild.exe %project%

rem and now lets zip the output

xcopy %outputdir%\*.* %celestedir% /E/H/C/I

rem Paul was here