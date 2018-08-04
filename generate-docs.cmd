@echo off
pushd %~dp0
dotnet restore
pushd tools
rd /s /q docfx.console.2.37.2
nuget install docfx.console -Version 2.37.2
popd
pushd doc
rd /s /q artifacts obj
%~dp0\tools\docfx.console.2.37.2\tools\docfx
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
popd
