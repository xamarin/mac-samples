NSTableViewBinding
==================

Original Project file lifted from Apple's sample site of the same name: NSTableViewBinding
Converted to Mono and C# by:  Kenneth J. Pouncey 2010/11/17

NSTableViewBinding is an application that demonstrates how to use Cocoa Bindings with a NSTableView.  

It demonstrates how to setup bindings in both Interface Builder or programatically.  

The compile directive flag "USE_BINDINGS_BY_CODE" defined in the projects's Options will 
instruct the compiler to build code that establishes the very same bindings created in Interface Builder.  

Of course, to set this flag will mean that these bindings setup in Interface Builder will not be necessary and 
become redundant.

Running example with code binding:

Steps for setting the Compiler Directive "USE_BINDINGS_BY_CODE"
--------------------------------------------------------------------
1) From the MonoDevelop Menu select Project -> NSTableViewBinding Options.  
2) The project options panel will be be opened.
3) Under the category Build -> Compiler look for the field labeled Define Symbols half way down the panel page.
4) Add the symbol USE_BINDINGS_BY_CODE 

Running example without code binding where the bindings are established in Interface Builder:			

Steps for un-setting the Compiler Directive "USE_BINDINGS_BY_CODE"
--------------------------------------------------------------------
1) From the MonoDevelop Menu select Project -> NSTableViewBinding Options.  
2) The project options panel will be be opened.
3) Under the category Build -> Compiler look for the field labeled Define Symbols half way down the panel page.
4) Make sure the symbol USE_BINDINGS_BY_CODE is not in this field.

Sample Requirements
The supplied Mono Solution/Project demonstrates demonstrates controlling NSTableView with the following:

NSArrayController drives the content for each table column.
NSArrayController drives the table's current selection.
Provides table row double-click inspection.
Proper enabling/disabling of buttons based on the table's current selection.
NSArrayController drives the content of several NSTextFields based on the controller's selection.
Provides notification of table selection changes through the use of observeValueForKeyPath.


Using the Sample:

Build and run the sample solution provided with MonoDevelop.  

Use the Add and Remove buttons (small square buttons marked with "+" and "-") to add and remove people from the table.  
You can edit each person's info by changing the first, last, and phone text fields.  
By double-clicking a row in the table can also alternately edit a given person through the use of a sheet window.

	
Changes from Previous Versions:
n/a

Feedback and Bug Reports:
