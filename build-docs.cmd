(goto) 2>nul & (
    cd %~dp0
    dotnet restore
    md tools
    cd tools\
    rd /s /q docfx.console.2.37.2
    nuget install docfx.console -Version 2.37.2
    cd ..
    tools\docfx.console.2.37.2\tools\docfx doc\docfx.json
    rd /s /q %TMP%\_site
    move doc\artifacts\_site %TMP%
    git branch -D gh-pages-staging
    git checkout --orphan gh-pages-staging
    git reset --hard
    git clean -xdfq --exclude=%~nx0
    xcopy /E /Q %TMP%\_site\* .
    git add .
    git commit -m "Regenerating gh-pages branch.  This was performed automatically."
    git checkout gh-pages
    git reset --hard gh-pages-staging
    git branch -D gh-pages-staging
    rem git push --force
)