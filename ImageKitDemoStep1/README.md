Simple demo to exercise ImageKit's ImageBrowserView.

While it is largely unrecognizable from the source, this code was inspired by
and modeled after step 1 of Apple''s ImageKit Demo.

This demo illustrates
	1) IKImageBrowserView
	2) IKImageBrowserDataSource
	3) IKImageBrowserItem
	4) drag and drop support
	
You can drag images (or PDFs, etc) around the browser view to re-organize
You can enter search text to filter the images displayed.
You can add images with a file browser
You can drag and drop images from Finder, iPhoto, etc.
You can load images from files, folders, or urls. Dragging a folder will add all files in that folder.
Events for double click, right click etc. are wired up, but only print notices to the console.

Regan Sarwas (rsarwas@gmail.com)
2010-01-31
