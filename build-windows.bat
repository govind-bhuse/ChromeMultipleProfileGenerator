@echo off
setlocal
echo Building Chrome Profile Generator for Windows 11...
where dotnet >nul 2>nul
if errorlevel 1 (
  echo .NET 8 SDK was not found.
  echo Install the .NET 8 SDK from https://dotnet.microsoft.com/download/dotnet/8.0
  pause
  exit /b 1
)
dotnet publish "%~dp0ChromeProfileGenerator.csproj" -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o "%~dp0publish"
if errorlevel 1 (
  echo Build failed. Please review the errors above.
  pause
  exit /b 1
)
echo.
echo Finished. Your EXE is in the publish folder.
pause
