@echo off
REM ===========================================================================
REM Updates https://nettopologysuite.github.io/NetTopologySuite.IO.GPX to have
REM the documentation that corresponds to whatever's in our working tree.
REM ===========================================================================
set DOCFX_PACKAGE_VERSION=2.37.2
pushd %~dp0
dotnet restore
pushd tools
rd /s /q docfx.console.%DOCFX_PACKAGE_VERSION%
nuget install docfx.console -Version %DOCFX_PACKAGE_VERSION%
popd
pushd doc
rd /s /q artifacts obj
%~dp0\tools\docfx.console.%DOCFX_PACKAGE_VERSION%\tools\docfx
pushd artifacts
git clone --branch gh-pages %~dp0 gh-pages
xcopy /Q /E /R /Y _site gh-pages
pushd gh-pages
git add .
git commit -m "Update docs.  This was performed automatically."
git push origin gh-pages
popd
popd
popd
git push origin gh-pages
popd
