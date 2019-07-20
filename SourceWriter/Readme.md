---
name: Xamarin.Mac - SourceWriter
description: The SourceWriter Xamarin.Mac app is a simple source code editor that provides support for code completion and simple syntax highlighting. It is...
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: sourcewriter
---
# SourceWriter

The SourceWriter Xamarin.Mac app is a simple source code editor that provides support for code completion and simple syntax highlighting. It is presented as an example of a complete Xamarin.Mac app that includes many of the features a user would expect to find in a typical Mac application.  

SourceWriter includes the following features:

* **App Icons** - Provides a unique [icon](https://developer.xamarin.com/guides/mac/deployment,_testing,_and_metrics/app-icon/) for the app and each of its associated File Types.
* **File Types** - Provides an example of assigning File Types to a Xamarin.Mac app.
* **Storyboards** - Shows how to use a [Storyboard](https://developer.xamarin.com/guides/mac/platform-features/storyboards/) and Xcode's Interface Builder to create a complex user interface.
* **Multiple Windows** - Provides an example of [working with multiple windows](https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/#Working_with_Multiple_Windows) in a Xamarin.Mac app.
* **Template Window** - Provides an example of a Template Window that allows the user to select the type of new document to create.
* **About Window** - Shows how to create a custom **About Window** and wire it into the **About App...** menu item.
* **Cross Communication** - Provides an example of talking across the multiple parts of the app: App Delegate, Windows, Controllers, Views, Menus and Toolbars.
* **Tracking Modified Windows** - Tracking when the document in a [Window has been modified](https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/#Modified_Windows_Content), notifying the user of the modification and prompting the user to save changes before closing a Window or quitting the app.
* **Open and Save Files** - Provides an example of opening and saving files and [marking a window as belonging to a given file](https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/#Setting_a_Windows_Title_and_Represented_File). 
* **Open and Save Dialogs** - Provides and example of working with the [Open](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#The_Open_Dialog) and [Save](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#The_Save_Dialog) Dialog boxes and presenting them as Sheets.
* **Open Recent Menu** - Shows how to implement the [**Open Recent...** menu item](https://developer.xamarin.com/guides/mac/user-interface/working-with-menus/#Working_with_the_Open_Recent_Menu) and track recently used files.
* **User Preferences** - Provides an example of working with [User Preferences](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Saving_and_Loading_Preferences) to control the behavior of the app.
* **Preferences Dialog** - Show how to present a [Preferences Dialog](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Creating_a_Preferences_Dialog) to the user to allow them to modify preferences and how to apply those changes to all open Windows.
* **Data Binding** - Provides an example of using [Data Binding and Key/Value](https://developer.xamarin.com/guides/mac/application_fundamentals/databinding/) coding with C# objects and UI items in Xcode's Interface Builder.
* **Menus** - Shows how to work with [Menus and Menu Items](https://developer.xamarin.com/guides/mac/user-interface/working-with-menus/) in a typical Mac app.
* **Manually Enabling/Disabling Menus** - Provides an example of manually [enabling and disabling menu items](https://developer.xamarin.com/guides/mac/user-interface/working-with-menus/#Enabling_and_Disabling_Menus_and_Items).
* **Modifying Menus On-The-Fly** - Provides an example of [adding and removing Menu Items](https://developer.xamarin.com/guides/mac/user-interface/working-with-menus/#Creating_Menus_from_Code) on-the-fly using C# code.
* **Custom Actions** - Shows how to create [custom Actions on a Window](https://developer.xamarin.com/guides/mac/user-interface/working-with-menus/#Working_with_Custom_Window_Actions), ties those actions into the First Responder and attach them to Menu Items to automatically enable/display the items and activate the actions when clicked.
* **Toolbars** - Shows how to work with [toolbars](https://developer.xamarin.com/guides/mac/user-interface/working-with-toolbars/) in a Xamarin.Mac app.
* **Manually Enabling/Disabling Toolbar Items** - Provides an example of manually [enabling and disabling Toolbar items](https://developer.xamarin.com/guides/mac/user-interface/working-with-toolbars/#Disabling_Toolbar_Items) in code.
* **Custom Sheet** - Shows how to [create and present a custom sheet](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Creating_a_Custom_Sheet).
* **Attributed Strings** - Provides an example of working with Attributed Strings and setting temporary attributes to highlight specific syntax in a string using a modular, reusable set of **Language Formatters** and **Language Descriptors**.
* **String Manipulation** - Provides an example of complex string manipulation.
* **Text Views** - Provides a complex example of working with text in a [`NSTextView`](https://developer.xamarin.com/guides/mac/user-interface/standard-controls/#Working_with_Text_Controls) and using the View as a Text Editor including maintaining tab level indent, conditional formatting and text insertion.
* **Auto Complete** - Shows how to work with the built in Auto Complete feature of the `NSTextView` and provide custom word suggestions.
* **Drag & Drop** - Provides an example of dragging text files from Finder and dropping them on the Text View to copy the contents into the text file at the current cursor location.
* **Web Views** - Provides a simple example of using a WebKit Web View to present formatted text to the user as a document preview.
* **Scroll Views** - Provides an example of working with a `NSScrollView` such as getting or setting the current scroll position.
* **Printing** - Provides a simple example of printing a document using built-in features of the `NSTextView` and `WebView`. 

The code has been fully commented and, where available, links have be provided from key technologies or methods to relevant information in the [Xamarin.Mac Guides Documentation](https://developer.xamarin.com/guides/#mac).

A [Read Me](https://github.com/xamarin/mac-samples/tree/master/SourceWriter/Documentation) document has been provided with a brief description of how the app was designed and works along with complete **API Documentation** (available in `Documentation/html/index.html` when downloaded) for the app.