@echo off
setlocal

:: Get the current directory of the script (without the trailing backslash)
set "CURRENT_DIR=%~dp0"
set "CURRENT_DIR=%CURRENT_DIR:~0,-1%"

:: Resolve relative paths
set "SOURCE=%CURRENT_DIR%\..\TEMP\.env"
set "DESTINATION=%CURRENT_DIR%\..\.."

:: Normalize paths (convert forward slashes for Windows)
set "SOURCE=%SOURCE:/=\%"
set "DESTINATION=%DESTINATION:/=\%"

:: Ensure destination folder exists
if not exist "%DESTINATION%" mkdir "%DESTINATION%"

:: Copy the file
copy "%SOURCE%" "%DESTINATION%" /Y

:: Check if the copy was successful
if %ERRORLEVEL%==0 (
    echo ✅ File copied successfully!
) else (
    echo ❌ Failed to copy the file.
)

pause
