@echo off
echo =====================================
echo         System Specifications
echo =====================================

:: Get CPU Information
echo CPU: 
wmic cpu get Name
echo -------------------------------------

:: Get RAM Information
echo RAM Installed: 
wmic memorychip get Capacity
wmic computersystem get TotalPhysicalMemory
echo -------------------------------------

:: Get OS Information
echo Operating System: 
wmic os get Caption, Version, OSArchitecture
echo -------------------------------------

:: Get GPU Information
echo GPU: 
wmic path win32_videocontroller get name
echo -------------------------------------

:: Get Disk Space
echo Disk Space (C: Drive):
wmic logicaldisk where DeviceID="C:" get Size, FreeSpace
echo -------------------------------------

:: Pause so user can read output
pause
