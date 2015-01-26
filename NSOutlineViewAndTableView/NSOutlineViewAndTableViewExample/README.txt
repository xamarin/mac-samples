This quick start guide will walk you through the programmatic creation of and 
use of NSTableView/NSOutlineView with the Xamarin.Mac classic API. 

These views often give beginners trouble, as they require knowledge of the 
delegate/data source pattern and their APIs have both 
Cell and View based variants.

This example assumes familiarity with the following documentation:
   http://developer.xamarin.com/guides/mac/application_fundamentals/patterns/
   http://developer.xamarin.com/guides/mac/application_fundamentals/mac-apis/

- NSTableView is a control for displaying tabular data with multiple rows and 
one or more columns. 
(https://developer.apple.com/documentation/Cocoa/Reference/ApplicationKit/Classes/NSTableView_Class/)

- NSOutlineView is a control for displaying hierarchal data in a tree 
like format. 
(https://developer.apple.com/library/mac///documentation/Cocoa/Reference/ApplicationKit/Classes/NSOutlineView_Class/index.html)

This example is written against the Xamarin.Mac Classic API, but migration to
Unified should be straight forward.

This example uses the NSView APIs, as they are preferred in new applications.

Start by looking at with AppDelegate.cs