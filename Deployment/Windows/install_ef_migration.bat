@echo off
setlocal

echo Installing Entity Framework Core Tools...
dotnet tool install --global dotnet-ef

if %errorlevel% neq 0 (
    echo Failed to install dotnet-ef.
    exit /b 1
)

echo dotnet-ef installed successfully.
exit /b 0
