# NSTableView / NSOutlineView

This quick start guide will walk you through the programmatic creation of and 
use of `NSTableView`/`NSOutlineView` with Xamarin.Mac.

These views often give beginners trouble, as they require knowledge of the 
delegate/data source pattern and their APIs have both 
`Cell` and `View` based variants.

This example assumes familiarity with the following documentation:

*  [Mac Patterns](http://developer.xamarin.com/guides/mac/application_fundamentals/patterns/)

*  [Mac APIs](http://developer.xamarin.com/guides/mac/application_fundamentals/mac-apis/)

The two examples are for:

- `NSTableView` is a control for displaying tabular data with multiple rows and 
one or more columns. 
[Apple's NSTableView doc](https://developer.apple.com/documentation/Cocoa/Reference/ApplicationKit/Classes/NSTableView_Class/)

- `NSOutlineView` is a control for displaying hierarchal data in a tree 
like format. 
[Apple's NSOutlineView doc](https://developer.apple.com/library/mac///documentation/Cocoa/Reference/ApplicationKit/Classes/NSOutlineView_Class/index.html)

This example uses the `NSView` APIs, as they are preferred in new applications.

Start by looking at **AppDelegate.cs**