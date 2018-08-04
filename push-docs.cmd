@echo off
REM ===========================================================================
REM Pushes the https://nettopologysuite.github.io/NetTopologySuite.IO.GPX
REM content to the server (run generate-docs.cmd first)
REM ===========================================================================
pushd %~dp0
pushd doc\obj
rd /s /q gh-pages
git clone --branch gh-pages %~dp0 gh-pages
xcopy /Q /E /R /Y generated-site-content gh-pages
pushd gh-pages
git add .
git commit -m "Update docs.  This was performed automatically."
git push origin gh-pages
popd
rd /s /q gh-pages
popd
git push origin gh-pages
popd
