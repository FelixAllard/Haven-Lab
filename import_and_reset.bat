@echo off
REM Set the root directory to the current working directory
SET ROOT_DIR=%cd%

REM Fetch the latest changes from GitHub
echo Fetching latest changes from GitHub...
git fetch origin

REM Reset local changes and match the main branch
echo Resetting local branch to match GitHub main branch...
git reset --hard origin/main

REM Optionally, check the status of the repository
echo Checking repository status...
git status

echo Import and reset complete. Second script executed.
pause