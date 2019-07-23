---
name: Xamarin.Mac - Drag and Drop Example
description: This sample shows how to do a simple drag & drop between two views in a Xamarin.Mac application. When the application is run, the user can drag...
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: draganddropexample
---
# Drag and Drop Example

This sample shows how to do a simple drag & drop between two views in a Xamarin.Mac application. When the application is run, the user can drag from the **Red View** and drop onto the **Green View**.

The drag will contain two items:

* `NSString` - A `NSString` value containing the value _"Hello World"_.
* `NSImage` - A `NSImage` containing a House icon.

When the drop happens on the **Green View**, the drop counter will be incremented and the contents of the drag package will be read and displayed in the console.

![Sample drag and drop sample](Screenshots/0.png)

## Prerequisites

* Mac computer with the latest version of macOS.
* [Visual Studio for Mac](https://visualstudio.microsoft.com/vs/mac/).
* Latest version of [Xcode](https://developer.apple.com/xcode/) from Apple.

## Running the sample

1. Open the solution file (**.sln**) in Visual Studio for Mac.
1. Use the **Run** button or menu to start the app.
