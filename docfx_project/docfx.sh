#!/bin/bash

if [[ $# -eq 0 ]]; then
    PARAMS="--serve"
else
    PARAMS="$1 $2 $3"
fi

DOCFX="$HOME/.nuget/packages/docfx.console/2.56.6/tools/docfx.exe DocFx.json $PARAMS"

dotnet restore
$DOCFX
