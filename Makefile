buildType=Release
affFile=en_US.aff
dictFile=en_US.dict
all: build

full: build test

build:
	xbuild /property:Configuration="$(buildType)" CompactNHunspell.sln

test:
	mcs Testing/Program.cs -r:CompactNHunspell/bin/$(buildType)/CompactNHunspell.dll
	cp CompactNHunspell/bin/$(buildType)/CompactNHunspell.dll Testing/
	mono Testing/Program.exe "$(affFile)" "$(dictFile)"
