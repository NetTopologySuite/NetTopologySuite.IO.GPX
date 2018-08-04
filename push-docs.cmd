@echo off
pushd %~dp0\doc
git clone --branch gh-pages %~dp0 gh-pages
xcopy /Q /E /R /Y generated-site-content gh-pages
pushd gh-pages
git add .
git commit -m "Update docs.  This was performed automatically."
git push origin gh-pages
popd
popd
pushd %~dp0
git push origin gh-pages
popd
