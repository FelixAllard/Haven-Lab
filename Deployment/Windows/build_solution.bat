@echo off


set "CURRENT_DIR=%~dp0"
set "CURRENT_DIR=%CURRENT_DIR:~0,-1%"

set "ROOT_DIR=%CURRENT_DIR%\..\.."



REM Loop through all folders in the root directory
for /d %%F in (%ROOT_DIR%\*) do (
    REM Skip the TestingProject folder
    if /i not "%%F" == "%ROOT_DIR%\TestingProject" (
        REM Check if a .csproj file exists in the folder
        if exist "%%F\*.csproj" (
            echo Building %%F
            REM Run dotnet build, publish, and run commands asynchronously
            pushd %%F && dotnet build
        )
    )
)
echo Finished building solution

REM Keep the terminal open for this script
pause
