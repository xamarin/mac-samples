---
name: Xamarin.Mac - ExtensionSamples
description: This sample demonstrates how to use app extensions in Xamarin.Mac. To register an extension on your machine you'll need run the host application,...
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: extensionsamples
---
# ExtensionSamples

This sample demonstrates how to use app extensions in Xamarin.Mac. To register an extension on your machine you'll need run the host application, **ExtensionSamples** one time and then enable each plugin from the **System Preference** Extension panel.

Open "Console" application to view the system log to view NSLog / Errors / Crashes of extension. Cleaning this project will unregister this plugin from the system.

Sample extensions:

* Share extension
* Finder extension
* Today extension

![ExtensionSamples application screenshot](Screenshots/0.png "ExtensionSamples application screenshot")

## Build Requirements

OS X 10.11, Xcode 7.0 or later

## Prerequisites

* Mac computer with the latest version of macOS.
* [Visual Studio for Mac](https://visualstudio.microsoft.com/vs/mac/).
* Latest version of [Xcode](https://developer.apple.com/xcode/) from Apple.

## Running the sample

1. Open the solution file (**.sln**) in Visual Studio for Mac.
1. Use the **Run** button or menu to start the app.

## Runtime

OS X 10.11

## License

Xamarin samples released under the MIT license
