#!/bin/bash
dotnet build -c Release
dotnet test NetTopologySuite.IO.GPX.Tests --no-build --no-restore -c Release
dotnet pack --no-build --no-dependencies --version-suffix=-pre$(printf \"%05d\" $TRAVIS_BUILD_NUMBER) --output $(TRAVIS_BUILD_DIR) -c Release
if [ "$TRAVIS_PULL_REQUEST" = "false" ]
then
    dotnet nuget push *.nupkg -k $MYGET_API_KEY -s https://www.myget.org/F/airbreather/api/v2/package
fi
