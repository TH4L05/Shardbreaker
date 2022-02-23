#!/bin/sh
ROOT=$WWISEROOT
ADDITIONAL=
if [ -f ../../WwiseRoot.txt ]
then
    ROOT=`cat ../../WwiseRoot.txt`
    ADDITIONAL=--wwiseroot=$ROOT
fi

if [[ "$OSTYPE" == "linux-gnu" ]]; then
    "$ROOT/Tools/Linux/bin/premake5" --scripts="$ROOT/Scripts/Premake" $ADDITIONAL $@
elif [[ "$OSTYPE" == "darwin"* ]]; then
    "$ROOT/Tools/Mac/bin/premake5" --scripts="$ROOT/Scripts/Premake" $ADDITIONAL $@
fi
