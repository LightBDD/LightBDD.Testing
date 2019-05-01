@echo off
rd /S /Q output 2> nul

dotnet test --configuration Release || exit /b
set /p version=<current_version
call dotnet pack --configuration Release /p:Version=%version% --output "%~dp0\output" || exit /b