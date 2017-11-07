del /F /Q /S *.CodeAnalysisLog.xml

".nuget\NuGet.exe" pack -sym Spooky.nuspec -BasePath .\
".nuget\NuGet.exe" pack -sym Spooky.Json20.nuspec -BasePath .\
".nuget\NuGet.exe" pack -sym Spooky.Xml.nuspec -BasePath .\
pause

copy *.nupkg C:\Nuget.LocalRepository\
pause
