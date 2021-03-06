---
name: Xamarin.Mac - ImageKit Demo
description: Simple demo to exercise ImageKit's ImageBrowserView. While it is largely unrecognizable from the source, this code was inspired by and modeled...
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: imagekitdemostep1
---
# ImageKit Demo

Simple demo to exercise ImageKit's `ImageBrowserView`.

While it is largely unrecognizable from the source, this code was inspired by and modeled after step 1 of Apple's ImageKit Demo.

This demo illustrates:

- IKImageBrowserView.
- IKImageBrowserDataSource.
- IKImageBrowserItem.
- Drag and drop support.

![ImageKit Demo application screenshot](Screenshots/0.png "ImageKit Demo application screenshot")

## Instructions

- Drag images (or PDFs, etc) around the browser view to re-organize.
- Enter search text to filter the images displayed.
- Add images with a file browser.
- Drag and drop images from Finder, iPhoto, etc.
- Load images from files, folders, or urls. 
- Dragging a folder will add all files in that folder.
- Events for double click, right click etc. are wired up, but only print notices to the console.
