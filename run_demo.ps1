# ===============================================================================
#   DISTRIBUTED PRINT QUEUE MANAGEMENT SYSTEM - POWERSHELL DEMO LAUNCHER
# ===============================================================================

Write-Host "===============================================================================" -ForegroundColor Cyan
Write-Host "  DISTRIBUTED PRINT QUEUE MANAGEMENT SYSTEM - LIVE DEMONSTRATION LAUNCHER" -ForegroundColor Cyan
Write-Host "===============================================================================" -ForegroundColor Cyan
Write-Host ""

$baseDir = $PSScriptRoot
if (-not $baseDir) { $baseDir = Get-Location }

$serverExe = Join-Path $baseDir "DistributedPrintQueue.Server\bin\Release\DistributedPrintQueue.Server.exe"
$clientExe = Join-Path $baseDir "DistributedPrintQueue.Client\bin\Release\DistributedPrintQueue.Client.exe"

# If binaries do not exist, build the solution first
if (-not (Test-Path $serverExe) -or -not (Test-Path $clientExe)) {
    Write-Host "[INFO] Binaries not found. Building solution..." -ForegroundColor Yellow
    cmd.exe /c "build.bat"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[ERROR] Build failed. Cannot launch demo." -ForegroundColor Red
        return
    }
}

Write-Host "[0/3] Cleaning up any lingering Server or Client processes..." -ForegroundColor DarkGray
Get-Process -Name "DistributedPrintQueue.Server", "DistributedPrintQueue.Client" -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1

Write-Host "[1/3] Starting Centralized Print Server on TCP Port 9000..." -ForegroundColor Green
Start-Process -FilePath $serverExe

Write-Host "Waiting 2 seconds for server initialization..." -ForegroundColor DarkGray
Start-Sleep -Seconds 2

Write-Host "[2/3] Launching Client Instance 1 (Student-PC-01)..." -ForegroundColor Green
Start-Process -FilePath $clientExe -ArgumentList "Student-PC-01"

Write-Host "Waiting 1 second..." -ForegroundColor DarkGray
Start-Sleep -Seconds 1

Write-Host "[3/3] Launching Client Instance 2 (Student-PC-02)..." -ForegroundColor Green
Start-Process -FilePath $clientExe -ArgumentList "Student-PC-02"

Write-Host ""
Write-Host "===============================================================================" -ForegroundColor Cyan
Write-Host "  DEMO ENVIRONMENT READY!" -ForegroundColor Cyan
Write-Host ""
Write-Host "  1. Server console is listening on tcp://localhost:9000"
Write-Host "  2. Two client Windows Forms instances are active:"
Write-Host "     - Client 1: Student-PC-01"
Write-Host "     - Client 2: Student-PC-02"
Write-Host "  3. Click 'Connect' on each client to connect to the server."
Write-Host "===============================================================================" -ForegroundColor Cyan
