#!/bin/sh

if [ $# -eq 0 ]; then
    PARAMS="--serve"
else
    PARAMS="$1 $2 $3"
fi

DOCFX="docfx docfx.json $PARAMS"

cd docfx_project
$DOCFX
