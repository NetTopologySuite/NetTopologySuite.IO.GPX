@echo off
pushd %~dp0
dotnet restore
pushd tools
rd /s /q docfx.console.2.37.2
nuget install docfx.console -Version 2.37.2
rd /s /q 7-Zip.CommandLine.18.1.0
nuget install 7-Zip.CommandLine -Version 18.1.0
popd
pushd doc
rd /s /q artifacts obj
%~dp0\tools\docfx.console.2.37.2\tools\docfx
del gh-pages.7z
pushd artifacts\_site
%~dp0\tools\7-Zip.CommandLine.18.1.0\tools\x64\7za a -r ..\..\gh-pages.7z .
popd
popd
popd
