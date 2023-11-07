#!/usr/bin/env bash

dotnet tool install docfx
dotnet tool run docfx docfx_project/docfx.json 

mv docfx_project/apiV2 apiV2_docs

for filename in apiV2_docs/*.md; do
    newlocation="$(echo $filename | sed -r 's|\.|\/|g; s|/([^/]*)$|.md|g')"
    # create the parent directory
    mkdir -p "$(dirname "$newlocation")"
    # move file into correct directory
    mv $filename $newlocation
done