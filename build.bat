@echo off
REM Build script for Deadpool's Healing Factor mod
REM Edit the RimWorldDir path below to match your Rimworld installation

REM Set your Rimworld installation path here
set RimWorldDir=C:\Program Files\Steam\steamapps\common\RimWorld

REM Alternative common paths - uncomment and modify as needed:
REM set RimWorldDir=C:\Program Files (x86)\Steam\steamapps\common\RimWorld
REM set RimWorldDir=D:\Steam\steamapps\common\RimWorld

echo Building Deadpool's Healing Factor mod...
echo Using Rimworld path: %RimWorldDir%

REM Check if Rimworld path exists
if not exist "%RimWorldDir%" (
    echo ERROR: Rimworld path not found: %RimWorldDir%
    echo Please edit this file and set the correct Rimworld installation path
    pause
    exit /b 1
)

REM Build the project
"C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe" "Source\DeadpoolsHealingFactor\DeadpoolsHealingFactor.csproj" /p:Configuration=Release /p:Platform="Any CPU" /p:RimWorldDir="%RimWorldDir%"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Build successful! DLL should be in: 1.6\Assemblies\
) else (
    echo.
    echo Build failed with error code: %ERRORLEVEL%
)

pause
