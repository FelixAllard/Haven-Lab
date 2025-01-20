@echo off

REM Set the root directory to the current working directory
SET ROOT_DIR=%cd%

REM Loop through all folders in the root directory
for /d %%F in (%ROOT_DIR%\*) do (
    REM Skip the TestingProject folder
    if /i not "%%F" == "%ROOT_DIR%\TestingProject" (
        REM Check if a .csproj file exists in the folder
        if exist "%%F\*.csproj" (
            echo Building and launching .NET project in folder: %%F
            REM Run dotnet build, publish, and run commands asynchronously
            start cmd /c "pushd %%F && dotnet build && dotnet publish -c Release && dotnet run && popd"
        )
    )
)

REM Now go to the front-end directory and run the React application asynchronously
cd "%ROOT_DIR%\front-end"

echo Running React application in front-end folder...
REM Assuming npm or yarn is installed for React projects
if exist "package.json" (
    REM Using npm to install and run the React app asynchronously
    start cmd /c "npm install && npm run build && serve -s build"
) else (
    echo No React application found in front-end folder.
)

REM Keep the terminal open for this script
pause
