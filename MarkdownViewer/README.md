# Github Flavored Markdown Viewer

This Mac sample is quite useful for authoring and viewing __Markdown__ files,
but also offers a little insight into building a hybrid web and native Mac
application.

* `MonoMac.AppKit.NSDocument` is used for automatic document handling. 
* The `MonoMac.WebKit.WebView` control is used for rendering.
* File system monitoring through `MonoMac.CoreServices.FSEventStream` is used to monitor the loaded document for changes.
* Changes made are reflected by replacing content directly in the `DOM`.
* JavaScript is used to scroll the page, invoked from native code.
* Navigation events are intercepted to open links in the system browser.

## Under the hood

The [Sundown](https://github.com/vmg/sundown) C library is used to actually
convert Markdown to HTML. In addition all the Github extensions are enabled.
A small C# binding to Sundown is included.

## Resources

* [Learn Github Flavored Markdown](https://help.github.com/articles/github-flavored-markdown)
* [Become familiar with classical Markdown](http://daringfireball.net/projects/markdown/syntax)

# A Screenshot

![Markdown Viewer Screenshot](https://raw.github.com/xamarin/mac-samples/master/MarkdownViewer/Screenshots/0.png)
