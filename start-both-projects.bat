@echo off
echo Starting SchoolPortal.API and SchoolPortal.Web...

REM Start the API project in a new command window
start "SchoolPortal.API" cmd /k "cd /d %~dp0SchoolPortal.API && dotnet run"

REM Wait a moment for the API to start
timeout /t 3 /nobreak >nul

REM Start the Web project in a new command window
start "SchoolPortal.Web" cmd /k "cd /d %~dp0SchoolPortal.Web && dotnet run"

echo.
echo Both projects are starting...
echo API will be available at: https://localhost:7185
echo Web will be available at: https://localhost:7029
echo API Swagger UI: https://localhost:7185/swagger
echo.
pause
