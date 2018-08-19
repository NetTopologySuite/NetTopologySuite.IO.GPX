@echo off
REM ===========================================================================
REM Regenerates the https://nettopologysuite.github.io/NetTopologySuite.IO.GPX
REM content locally
REM ===========================================================================
set DOCFX_PACKAGE_VERSION=2.38.1
pushd %~dp0
REM incremental / cached builds tweak things about the output, so let's do it
REM all fresh if we can help it...
rd /s /q src\NetTopologySuite.IO.GPX\obj
rd /s /q doc\obj
dotnet restore
pushd tools
rd /s /q docfx.console.%DOCFX_PACKAGE_VERSION%
nuget install docfx.console -Version %DOCFX_PACKAGE_VERSION%
popd
%~dp0\tools\docfx.console.%DOCFX_PACKAGE_VERSION%\tools\docfx doc\docfx.json
popd
