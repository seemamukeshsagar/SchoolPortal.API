# PowerShell script to start both SchoolPortal.API and SchoolPortal.Web simultaneously
# This script will start both projects in separate terminal windows

Write-Host "Starting SchoolPortal.API and SchoolPortal.Web..." -ForegroundColor Green

# Start the API project in a new PowerShell window
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\SchoolPortal.API'; dotnet run"

# Wait a moment for the API to start
Start-Sleep -Seconds 3

# Start the Web project in a new PowerShell window
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\SchoolPortal.Web'; dotnet run"

Write-Host "Both projects are starting..." -ForegroundColor Yellow
Write-Host "API will be available at: https://localhost:7185" -ForegroundColor Cyan
Write-Host "Web will be available at: https://localhost:7029" -ForegroundColor Cyan
Write-Host "API Swagger UI: https://localhost:7185/swagger" -ForegroundColor Magenta

# Keep the script window open
Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
