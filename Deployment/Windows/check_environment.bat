@echo off
setlocal EnableDelayedExpansion

echo Checking system requirements...

:: Pause for 1 second
ping -n 2 127.0.0.1 >nul

:: Check .NET 8.0+
dotnet --list-sdks | findstr "8." >nul
if %errorlevel% neq 0 (
    echo .NET 8.0 or later is not installed.
    exit /b 1
)
echo .NET 8.0+ is installed.

:: Pause for 1 second
ping -n 2 127.0.0.1 >nul

:: Check NPM
for /f %%v in ('npm --version 2^>nul') do set npm_version=%%v
if not defined npm_version (
    echo NPM is not installed or not in PATH.
    exit /b 1
)
echo NPM version %npm_version% is installed.

:: Pause for 1 second
ping -n 2 127.0.0.1 >nul

echo All requirements are met.
pause
exit /b 0