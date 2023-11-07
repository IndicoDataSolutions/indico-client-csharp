#!/usr/bin/env bash

dotnet tool install docfx
dotnet tool run docfx docfx_project/docfx.json 

mv docfx_project/apiV2/* apiV2_docs

for filename in apiV2_docs/*.md; do
    if [ "$(basename $filename)" != "IndicoV2.md" ]; then
        newdir="$(dirname "$(echo $filename | sed -r 's|\.|\/|g; s|/([^/]*)$|.md|g')")" 
        # create the parent directory
        mkdir -p $newdir
        # move file into correct directory
        newfilename="$(basename $filename | sed -r 's|IndicoV2\.(.+)\.md|\1|; s|\.|-|g')"
        newlocation="${newdir}/${newfilename}.md"
        mv $filename $newlocation
    fi
done

mv readme_docs/CommonModels.md apiV2_docs/IndicoV2/CommonModels.md
mv readme_docs/Extensions.md apiV2_docs/IndicoV2/Extensions.md

cd ./apiV2_docs
# fix links
find . -type f -name '*.md' -print0 | xargs -0 sed -i -r 's|IHasCursor\\-1|IHasCursor-1|; s|IndicoV2\.(.+)\.md|\1|; s|\.|-|g'