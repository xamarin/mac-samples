ExtensionSamples
==============

This sample demonstrates how to use app extensions in Xamarin.Mac. To install an extension on your machine you'll need to build and run the respective project from IDE to register this extension with PluginKit. However, sometimes it will not be enabled by default. 

Open System Preferences -> Extensions and enable the plugin.
Open "Console" application to view the system log to view NSLog / Errors / Crashes of extension. Cleaning this project will unregister this plugin from PluginKit.
PluginKit register/unregister can be done manually through the Apple PluginKit command line tool.

Sample extensions:
* Share extension
* Finder extension
* Today extension

Build Requirements
------------------
OS X 10.11, Xcode 7.0 or later

Runtime
------------------
OS X 10.11

Copyright
---------

Xamarin samples released under the MIT license

Author
------

Chris Hammons