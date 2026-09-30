@echo off
setlocal

echo ===============================================================================
echo   DISTRIBUTED PRINT QUEUE MANAGEMENT SYSTEM - LIVE DEMONSTRATION LAUNCHER
echo ===============================================================================
echo.

set "SERVER_EXE=DistributedPrintQueue.Server\bin\Release\DistributedPrintQueue.Server.exe"
set "CLIENT_EXE=DistributedPrintQueue.Client\bin\Release\DistributedPrintQueue.Client.exe"

if not exist "%SERVER_EXE%" (
    echo [INFO] Release binaries not found. Invoking build.bat first...
    call build.bat
    if %ERRORLEVEL% NEQ 0 (
        echo [ERROR] Build failed. Cannot launch demo.
        pause
        exit /b %ERRORLEVEL%
    )
)

echo [0/3] Terminating any lingering Print Server or Client instances...
taskkill /f /im DistributedPrintQueue.Server.exe >nul 2>&1
taskkill /f /im DistributedPrintQueue.Client.exe >nul 2>&1
timeout /t 1 /nobreak >nul

echo [1/3] Starting Centralized Print Server on TCP Port 9000...
start "Distributed Print Server (.NET Remoting TCP:9000)" "%SERVER_EXE%"

echo Waiting 2 seconds for server remoting initialization...
timeout /t 2 /nobreak >nul

echo [2/3] Launching Client Instance 1 (Student-PC-01)...
start "Print Client 1 - Student-PC-01" "%CLIENT_EXE%" Student-PC-01

echo Waiting 1 second...
timeout /t 1 /nobreak >nul

echo [3/3] Launching Client Instance 2 (Student-PC-02)...
start "Print Client 2 - Student-PC-02" "%CLIENT_EXE%" Student-PC-02

echo.
echo ===============================================================================
echo   DEMO ENVIRONMENT READY!
echo.
echo   1. The Server console window is running and listening on port 9000.
echo   2. Two separate Windows Forms client windows are open:
echo      - Client 1: Pre-configured as 'Student-PC-01'
echo      - Client 2: Pre-configured as 'Student-PC-02'
echo   3. Click 'Connect' on each client to establish the .NET Remoting TCP connection.
echo   4. Submit jobs from both clients and observe priority scheduling on the server!
echo ===============================================================================
echo.
pause
