buildType=Release
nunit=nunit-console2
affFile=en_US.aff
dictFile=en_US.dic
stylecop=StyleCopCmd.Console.exe
libPath=""
version:= `cat CompactNHunspell/Properties/AssemblyInfo.cs | grep "AssemblyFileVersion"| cut -f2 -d '"' | cut -f1,2,3 -d '.' | awk '{print $0}'`
all: build

release: analyze package

build:
	xbuild /property:Configuration="$(buildType)" CompactNHunspell.sln

test: build
	"$(nunit)" CompactNHunspell.Test/bin/$(buildType)/CompactNHunspell.Test.dll -noshadow

analyze: test
	mono "$(stylecop)" -s CompactNHunspell.sln -t

integrate: analyze
	mono CompactNHunspell.Harness/bin/$(buildType)/CompactNHunspell.Harness.exe "$(libPath)$(affFile)" "$(libPath)$(dictFile)"

package:
	zip -j CompactNHunspell-$(version).zip CompactNHunspell/bin/$(buildType)/CompactNHunspell.dll
