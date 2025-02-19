@echo off
setlocal enabledelayedexpansion

set "CURRENT_DIR=%~dp0"
set "CURRENT_DIR=%CURRENT_DIR:~0,-1%"

set "ENV_FILE=%CURRENT_DIR%\..\..\.env"


:: Read .env file and extract variables
for /f "tokens=1,2 delims==" %%A in (%ENV_FILE%) do (
    set "VAR_NAME=%%A"
    set "VAR_VALUE=%%B"

    :: Trim spaces
    set "VAR_NAME=!VAR_NAME: =!"
    set "VAR_VALUE=!VAR_VALUE: =!"

    if /I "!VAR_NAME!"=="GLOBAL_DB_HOST" set "GLOBAL_DB_HOST=!VAR_VALUE!"
    if /I "!VAR_NAME!"=="GLOBAL_DB_PORT" set "GLOBAL_DB_PORT=!VAR_VALUE!"
    if /I "!VAR_NAME!"=="GLOBAL_DB_USER" set "GLOBAL_DB_USER=!VAR_VALUE!"
    if /I "!VAR_NAME!"=="GLOBAL_DB_PASSWORD" set "GLOBAL_DB_PASSWORD=!VAR_VALUE!"
)

:: Check if values are set
if not defined GLOBAL_DB_HOST echo ❌ Missing GLOBAL_DB_HOST in .env & exit /b 1
if not defined GLOBAL_DB_PORT echo ❌ Missing GLOBAL_DB_PORT in .env & exit /b 1
if not defined GLOBAL_DB_USER echo ❌ Missing GLOBAL_DB_USER in .env & exit /b 1
if not defined GLOBAL_DB_PASSWORD echo ❌ Missing GLOBAL_DB_PASSWORD in .env & exit /b 1

:: Check MySQL Connection
echo Checking connection to MySQL...
mysql -h %GLOBAL_DB_HOST% -P %GLOBAL_DB_PORT% -u %GLOBAL_DB_USER% -p%GLOBAL_DB_PASSWORD% -e "SELECT 1;" >nul 2>&1

:: Check if connection was successful
if %ERRORLEVEL%==0 (
    echo ✅ Connection successful!
) else (
    echo ❌ Connection failed!
)

pause
