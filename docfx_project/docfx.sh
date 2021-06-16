#!/bin/sh

if [ $# -eq 0 ]; then
    PARAMS="--serve"
else
    PARAMS="$1 $2 $3"
fi

NUGET="./nuget"
DOCFX="$NUGET/docfx.console/2.58.0/tools/docfx.exe DocFx.json $PARAMS"

cd docfx_project
dotnet restore --packages $NUGET
$DOCFX
