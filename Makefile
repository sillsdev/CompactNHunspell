buildType=Release
all: build

build:
	xbuild /property:Configuration="$(buildType)" CompactNHunspell.sln
