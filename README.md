CompactNHunspell
================
NHunspell compact implementation for spell checking in Windows &amp; Linux from C#

Usage
=====
CompactNHunspell requires input of the affix/dict files. These files are required to use the underlying hunspell functionality.

Requirements
============
* Linux requires hunspell to be installed (mainly libhunspell)
* Windows requires the Hunspellx86 or Hunspellx64 libraries (depending on architecture)

Debugging
=========
* Include an appSetting key in the app.config/web.config with a location for tracing
```text
<add key="CompactNHunspell.TraceFile" value="path/to/a/trace/location" />
```
* DEBUGVERBOSE builds will write to Console
* Mono only: If it seems that the underlying hunspell libraries are not loading, try running the application and looking for library load failures for hunspell itself:
```text
MONO_LOG_LEVEL=debug mono <application>
```
