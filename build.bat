@echo off
setlocal enabledelayedexpansion

echo ===============================================================================
echo   Building Distributed Print Queue Management System (Release Configuration)
echo ===============================================================================
echo.

set MSBUILD_PATH=

:: 1. Check if MSBuild is already in PATH
where msbuild.exe >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    set MSBUILD_PATH=msbuild.exe
    goto :Found
)

:: 2. Check Visual Studio 2022 / 2019 installations
if exist "%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    goto :Found
)
if exist "%ProgramFiles%\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=%ProgramFiles%\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
    goto :Found
)
if exist "%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
    goto :Found
)
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
    goto :Found
)

:: 3. Check .NET Framework 4.8 Built-in MSBuild (guaranteed on Windows 10/11)
if exist "%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe" (
    set "MSBUILD_PATH=%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
    goto :Found
)
if exist "%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" (
    set "MSBUILD_PATH=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
    goto :Found
)

echo [ERROR] MSBuild.exe could not be found automatically.
echo Please run this script from a 'Developer Command Prompt for VS' or ensure .NET Framework 4.8 is installed.
exit /b 1

:Found
echo [OK] Using MSBuild at: "!MSBUILD_PATH!"
echo.

"!MSBUILD_PATH!" DistributedPrintQueueSystem.sln /p:Configuration=Release /v:minimal
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [BUILD FAILED] An error occurred during compilation.
    exit /b %ERRORLEVEL%
)

echo.
echo ===============================================================================
echo   BUILD SUCCEEDED! Binaries are located in:
echo   - Server: DistributedPrintQueue.Server\bin\Release\DistributedPrintQueue.Server.exe
echo   - Client: DistributedPrintQueue.Client\bin\Release\DistributedPrintQueue.Client.exe
echo   - Shared: DistributedPrintQueue.Shared\bin\Release\DistributedPrintQueue.Shared.dll
echo ===============================================================================
echo.
exit /b 0
