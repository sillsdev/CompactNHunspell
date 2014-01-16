#!/bin/bash
usage="Usage: $(basename "$0") [-h] [-s] [-r] -- simple build script for CompactNHunspell

where:
    -h show this help text
    -r Build in Release mode"

DORELEASE=false
while getopts ":rsh" opt; do
   case $opt in
	r)
	DORELEASE=true
	;;
	h)
	echo "$usage"
	exit
	;;
	\?)
	echo "Unknown argument: -$OPTARG"
	;;
    esac
done

if ! hash xbuild 2>/dev/null; then
	echo "xbuild is required to use the build script"	
	exit 1
fi

# Build type
BUILD="Debug"
if $DORELEASE ; then
	BUILD="Release"
fi

# Build
xbuild /property:Configuration=$BUILD CompactNHunspell.sln
if [ $? -eq 1 ]; then
	echo "Build failed"
	exit 1
fi

echo "All done"
