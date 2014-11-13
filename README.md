CompactNHunspell
================
NHunspell compact implementation for spell checking in Windows &amp; Linux from C#

Usage
=====
CompactNHunspell requires input of the affix/dict files. These files are required to use the underlying hunspell functionality.

Requirements
============
* Windows requires the Hunspellx86 or Hunspellx64 libraries (depending on architecture) - pull NHunspell to get these (checkout the powershell script or run it to get dependencies)
* Linux requires hunspell to be installed (mainly libhunspell). The Linux version is currently using "libhunspell-1.3.so.0" as the reference. If a different name/version is preferred see the example CompactNHunspell.dll.config (or the Mac variant: CompactNHunspell.macosx.dll.config) and this link: http://www.mono-project.com/docs/advanced/pinvoke/

Building
========
* Linux - use make. To (successfully) run the quick tests set the affFile=/path/to/a/en_US.aff file and dictFile=/path/to/a/en_US.dic file or libPath=/path/to/spell/dir/

Debugging
=========
* Include an appSetting key in the app.config/web.config with a location for tracing
```text
<add key="CompactNHunspell.TraceFile" value="path/to/a/trace/location" />
```
* To write to the console:
```text
<add key="CompactNHunspell.Verbose" value="true" />
```
* To restrict the logger to the CompactNHunspell module (ignore the LogActions etting)
```text
<add key="CompactNHunspell.Restricted" value="true" />
```
* To force the use of a specific underlying spell checker set the following (using types derived from the BaseHunspell class)
```text
<add key="CompactNHunspell.OverrideType" value="CompactNHunspell.HunspellLinux" />
```
* Mono only: If it seems that the underlying hunspell libraries are not loading, try running the application and looking for library load failures for hunspell itself:
```text
MONO_LOG_LEVEL=debug mono <application>
```
