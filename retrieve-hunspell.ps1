$enUSFile = "http://downloads.sourceforge.net/project/hunspell/Spelling%20dictionaries/en_US/en_US.zip?r=&ts=1409247746&use_mirror=colocrossing"
$nhunspell = "https://nuget.org/api/v2/package/NHunspell"
$nunit = "http://launchpad.net/nunitv2/trunk/2.6.3/+download/NUnit-2.6.3.zip"
$location = "$pwd\hunspell\dl\"
$binFolder = "bin"
if (Test-Path $location)
{
	rm "$location\..\*" -Recurse | Out-Null
}

mkdir $location | Out-Null
function GetAndUnzip($url, $file)
{
	$client = new-object System.Net.WebClient
	$zipFile = "$location$file.zip"
	$client.DownloadFile($url, $zipFile)
	$shell = new-object -com shell.application
	$zip = $shell.NameSpace($zipFile)
	foreach($item in $zip.items())
	{
		$shell.Namespace($location).copyhere($item)
	}
}

GetAndUnzip $enUSFile "en-us"
GetAndUnzip $nhunspell "nhunspell"
mv "$location\en_US*" "$location\.."
mv "$location\content\*.dll" "$location\.."

GetAndUnzip $nunit "nunit"
if (!(Test-Path -Path "$binFolder"))
{
	mkdir "$binFolder" | Out-Null
}

mv "$location\NUnit-2.6.3\bin\*" "$binFolder" -Force
