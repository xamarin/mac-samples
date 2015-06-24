Drag and Drop Example
=====================

This sample shows how to do a simple drag & drop between two views in a Xamarin.Mac application. When the application is run, the user can drag from the **Red View** and drop onto the **Green View**.

The drag will contain two items:

* `NSString` - A `NSString` value containing the value _"Hello World"_.
* `NSImage` - A `NSImage` containing a House icon.

When the drop happens on the **Green View**, the drop counter will be incremented and the contents of the drag package will be read and displayed in the console.