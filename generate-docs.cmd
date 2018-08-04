@echo off
REM ===========================================================================
REM Regenerates the https://nettopologysuite.github.io/NetTopologySuite.IO.GPX
REM content locally
REM ===========================================================================
set DOCFX_PACKAGE_VERSION=2.37.2
pushd %~dp0
dotnet restore
pushd tools
rd /s /q docfx.console.%DOCFX_PACKAGE_VERSION%
nuget install docfx.console -Version %DOCFX_PACKAGE_VERSION%
popd
%~dp0\tools\docfx.console.%DOCFX_PACKAGE_VERSION%\tools\docfx doc\docfx.json
popd
