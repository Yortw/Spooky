@echo off
echo Press any key to publish
pause
".nuget\NuGet.exe" push Spooky.1.0.0.3.nupkg -Source https://api.nuget.org/v3/index.json
".nuget\NuGet.exe" push Spooky.Json20.1.0.0.3.nupkg -Source https://api.nuget.org/v3/index.json
pause