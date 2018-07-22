#!/bin/bash
dotnet build -c Release
dotnet test NetTopologySuite.IO.GPX.Tests --no-build --no-restore -c Release
if [ "$TRAVIS_EVENT_TYPE" = "push" && "$TRAVIS_BRANCH" == "develop" ]
then
    dotnet pack --no-build --no-dependencies --version-suffix=travis$(printf "%05d" $TRAVIS_BUILD_NUMBER) --output $TRAVIS_BUILD_DIR -c Release
    dotnet nuget push *.nupkg -k $MYGET_API_KEY -s https://www.myget.org/F/airbreather/api/v2/package
fi
