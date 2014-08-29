$enUSFile = "http://downloads.sourceforge.net/project/hunspell/Spelling%20dictionaries/en_US/en_US.zip?r=&ts=1409247746&use_mirror=colocrossing"
$nhunspell = "https://nuget.org/api/v2/package/NHunspell"
$location = "$pwd\hunspell\dl\"
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
mv "$location\native\*.dll" "$location\.."
