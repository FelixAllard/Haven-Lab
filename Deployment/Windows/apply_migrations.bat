@echo off
setlocal enabledelayedexpansion

:: Set the path to your .env file (Change this if needed)
set "CURRENT_DIR=%~dp0"
set "CURRENT_DIR=%CURRENT_DIR:~0,-1%"
set "ENV_FILE_PATH=%CURRENT_DIR%\..\..\.env"

:: If an argument is provided, use it as the .env file path
if not "%~1"=="" set "ENV_FILE_PATH=%~1"

:: Check if .env file exists
if not exist "%ENV_FILE_PATH%" (
    echo ERROR: .env file not found at %ENV_FILE_PATH%!
    pause
    exit /b 1
)

:: Load .env file
for /f "delims=" %%x in (%ENV_FILE_PATH%) do (
    set "line=%%x"
    set "line=!line:#=!"
    for /f "tokens=1,2 delims==" %%a in ("!line!") do set "%%a=%%b"
)

:: Validate environment variables
if "%GLOBAL_DB_HOST%"=="" (
    echo ERROR: GLOBAL_DB_HOST not found in .env file!
    pause
    exit /b 1
)
if "%GLOBAL_DB_PORT%"=="" (
    echo ERROR: GLOBAL_DB_PORT not found in .env file!
    pause
    exit /b 1
)
if "%GLOBAL_DB_USER%"=="" (
    echo ERROR: GLOBAL_DB_USER not found in .env file!
    pause
    exit /b 1
)
if "%GLOBAL_DB_PASSWORD%"=="" (
    echo ERROR: GLOBAL_DB_PASSWORD not found in .env file!
    pause
    exit /b 1
)

:: Construct MySQL connection string
set "CONNECTION_STRING=Server=%GLOBAL_DB_HOST%;Port=%GLOBAL_DB_PORT%;User Id=%GLOBAL_DB_USER%;Password=%GLOBAL_DB_PASSWORD%;Database=appointment_db"

:: Set paths for the two repositories (point to the actual EF project, not migration files)
set "REPO1_PATH=%CURRENT_DIR%\..\..\AppointmentsService"
set "REPO2_PATH=%CURRENT_DIR%\..\..\Email-Api"

:: Change directory to first repository and apply migration
cd /d "%REPO1_PATH%"
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to navigate to Repo1 directory!
    pause
    exit /b 1
)

if "%~2"=="" (
    echo Applying latest migration in Repo1...
    dotnet ef database update --connection "%CONNECTION_STRING%"
) else (
    echo Applying migration "%~2" in Repo1...
    dotnet ef database update "%~2" --connection "%CONNECTION_STRING%"
)

if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Migration failed in Repo1!
    pause
    exit /b 1
)

:: Change directory to second repository and apply migration
cd /d "%REPO2_PATH%"
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to navigate to Repo2 directory!
    pause
    exit /b 1
)

if "%~3"=="" (
    echo Applying latest migration in Repo2...
    dotnet ef database update --connection "%CONNECTION_STRING%"
) else (
    echo Applying migration "%~3" in Repo2...
    dotnet ef database update "%~3" --connection "%CONNECTION_STRING%"
)

if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Migration failed in Repo2!
    pause
    exit /b 1
)

echo Both migrations applied successfully!
pause
exit /b 0
