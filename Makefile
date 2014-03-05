buildType=Release
affFile=en_US.aff
dictFile=en_US.dic
libPath=""
version:= `cat CompactNHunspell/Properties/AssemblyInfo.cs | grep "AssemblyFileVersion"| cut -f2 -d '"' | cut -f1,2,3 -d '.' | awk '{print $0}'`
all: build

release: full package

full: build test

build:
	xbuild /property:Configuration="$(buildType)" CompactNHunspell.sln

test:
	mcs Testing/Program.cs -r:CompactNHunspell/bin/$(buildType)/CompactNHunspell.dll
	cp CompactNHunspell/bin/$(buildType)/CompactNHunspell.dll Testing/
	mono Testing/Program.exe "$(libPath)$(affFile)" "$(libPath)$(dictFile)"


package:
	zip -j CompactNHunspell-$(version).zip CompactNHunspell/bin/$(buildType)/CompactNHunspell.dll
