#!/usr/bin/env bash

dotnet tool install docfx
dotnet tool run docfx docfx_project/docfx.json 
cd docfx_project
rm apiV2/toc.yml

for filename in apiV2/*.md; do
    newfilename="$(basename $filename | sed -r 's|(.+)\.md|\1|; s|\.|-|g')"
    newdir="$(basename $newfilename | sed -r 's|(IndicoV2\-\w+)\-.*|\1|')"
    if [ $newfilename == $newdir ]; then
        mv $filename "apiV2/$newfilename.md"
    else
        mkdir -p "apiV2/$newdir"
        mv $filename "apiV2/$newdir/$newfilename.md"
    fi
done

mv ../readme_docs/CommonModels.md ./apiV2/IndicoV2-CommonModels.md
mv ../readme_docs/Extensions.md ./apiV2/IndicoV2-Extensions.md

# fix links
find apiV2 -type f -name '*.md' -print0 | xargs -0 sed -i -r 's|IHasCursor\\-1|IHasCursor-1|; s|(\(.+)\.md|\L\1|; s|\.|-|g'
cd ..