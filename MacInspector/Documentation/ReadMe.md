# MacInspector

Most modern OS X applications present auxiliary controls and options that affect the active document or selection as **Inspectors** that are part of the Main Window (like Apple's Pages or Numbers apps), instead of using [Panel Windows](https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/#Panels).

While **Panel Windows** have been deprecated in Storyboards, Apple does not provide a specific User Interface Widget to build **Inspectors**. Instead the developer must use `NSSplitViewControllers` and standard UI Widgets (such as Checkboxes and Text Fields) to create the [Inspector Interface](/guides/mac/user-interface/working-with-windows/#Inspectors).

This app shows an example of creating two different types of Inspector Panels and running those panels with a Split View. It also uses a [Segment Control](/guides/mac/user-interface/standard-controls/#Working_with_Selection_Controls) in the [Toolbar](/guides/mac/user-interface/working-with-toolbars/) to control the **Inspector** similar to Apple's Pages word processing app.

# Implementation

The following sections will briefly discuss the key source code components and how they were used to implement MacInspector's functionality.

## Generic Classes

The following generic classes are defined:

* **CustomBox** - Provides a customized `NSBox` that responds to the user clicking on it and exposes its properties via [Key-Value Coding and Data Binding](https://developer.xamarin.com/guides/mac/application_fundamentals/databinding/).
* **DocumentProperties.cs** - Provides a set of adjustable properties for the document being edited and exposes those properties via [Key-Value Coding and Data Binding](https://developer.xamarin.com/guides/mac/application_fundamentals/databinding/).
* **ReplaceInspectorPanelSegue.cs** - Creates a custom Segue type that is used to show new **Inspector Panels** in the **Inspector**.

## Resources

Provides the custom Toolbar images used in MacInspector.

## App, Windows and Controllers

The following classes provide app support, Windows, Windows Controllers, View and View Controller Support in the application:

* **AppDelegate.cs** - This file contains our [AppDelegate](https://developer.xamarin.com/guides/mac/getting_started/hello,_mac/#AppDelegate.cs) class, which is responsible for creating our window and listening to OS events.
* **BoxPrefsViewController.cs** - Handles the **Inspector Panel** for the `CustomBox` object currently selected. It uses [Key-Value Coding and Data Binding](https://developer.xamarin.com/guides/mac/application_fundamentals/databinding/) to maintain the values.
* **ContentViewController.cs** - Handles the example window content on the left side of the Split View. Selecting `CustomBox` objects in this view will present their **Inspector Panels** in the **Inspector**.
* **DocPrefsViewController.cs** - Handles the **Inspector Panel** for the `DocumentProperties` for the window. It uses [Key-Value Coding and Data Binding](https://developer.xamarin.com/guides/mac/application_fundamentals/databinding/) to maintain the values.
* **Entitlements.plist** - Controls any Entitlements for the app such as Sandboxing or iCloud support.
* **Info.plist** - The [`Info.plist`](https://developer.xamarin.com/guides/mac/getting_started/hello,_mac/#Info.plist) file defines information for the app such as its Title, Type and Icon.
* **InspectorViewController.cs** - Handles the [Inspector Interface](/guides/mac/user-interface/working-with-windows/#Inspectors) and controls the hiding and displaying of **Inspector Panels**.
* **Main.cs** - The [`Main.cs`](https://developer.xamarin.com/guides/mac/getting_started/hello,_mac/#Main.cs) file starts the App.
* **Main.storyboard** - This [Storyboard](https://developer.xamarin.com/guides/mac/platform-features/storyboards/) defines the User Interface for the app. Use Xcode's Interface Builder to edit.
* **MainSplitViewController.cs** - Handles the Split View used to contain the document content on the left and the **Inspector** on the right. Contains routines to help the Content communicate with the **Inspector**.
* **MainWindow.cs** - Defines the main window for the app.
* **MainWindowController.cs** - Handles the `MainWindow` for the app and controls the [Toolbar](/guides/mac/user-interface/working-with-toolbars/) for the window.

For more detailed descriptions, please see the [API Documentation](html/index.html) for the MacInspector source code.

