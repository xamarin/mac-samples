# SourceWriter

The SourceWriter Xamarin.Mac app is a simple source code editor that provides support for code completion and simple syntax highlighting. SourceWriter works with text documents in the following languages: C#, HTML, MarkDown and XML.

Multiple documents of different types can be open at the same time in multiple windows and SourceWriter provides the ability to preview the final output in a central Preview Window (for example the output of MarkDown formatted text). Additionally, the user can print both the source and the final preview output.

SourceWriter uses a set of modular, reusable **Language Formatters** and **Language Descriptors** to provide keywords, closures, custom formatting commands and custom format styles for the languages supported. 

When a keyword is selected in the editor, a tooltip is displayed in the status bar at the bottom of the editor window. The user can optionally display an Information Sheet with more details on the selected word.

SourceWriter includes many user defined options to control the function and styling of the editor. These options are implemented via a user preferences system and maintained with a Preferences Dialog Box.

# Implementation

The following sections will briefly discuss the key source code components and how they were used to implement SourceWriter's functionality.

## Generic Classes

The following generic classes are defined:

* **AppPreferences.cs** - Provides the [user's preferences](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Saving_and_Loading_Preferences) that control the features of the app such as syntax highlight colors. Preferences are stored and retrieved using `NSUserPreferences` and [Data Binding and Key/Value Coding](https://developer.xamarin.com/guides/mac/application_fundamentals/databinding/) is used to communicate with UI elements.
* **EditorWindowDelegate.cs** - Handles the UI events for the Editor Window such as [tracking when the window is closed](https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/#Saving_Changes_Before_Closing_a_Window) or becomes the front most window. This code adds and removes Format Menu items based on the current language Descriptor.
* **ManualToolbarItem.cs** - Provides a type of toolbar item that can be manually [enabled or disabled](https://developer.xamarin.com/guides/mac/user-interface/working-with-toolbars/#Disabling_Toolbar_Items).
* **MarkDown.cs** - An open source library used to convert MarkDown formatted text into HTML for display in a Web View preview window.
* **PreferenceWindowDelegate.cs** - Handles the UI events for the [Preference Window](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Creating_a_Preferences_Dialog) and [updates any open windows with preference changes](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Applying_Preference_Changes_to_All_Open_Windows).
* **PreviewWindowDelegate.cs** - Handles the UI events for the Preview Window such as it being closed.

## Language Formatters

The following classes have been defined for the following languages that SourceWriter supports editing:

* **CSharpDescriptor.cs** - Defines the keywords, custom commands and syntax highlighting rules for the C# language.
* **HTMLDescriptor.cs** - Defines the keywords, custom commands and syntax highlighting rules for the HTML language.
* **MarkDownDescriptor.cs** - Defines the keywords, custom commands and syntax highlighting rules for the MarkDown language.
* **XMLDescriptor.cs** - Defines the keywords, custom commands and syntax highlighting rules for the XML language.

See the **Language Formatter** section below for more details.

## Language Formatter

Defines the modular, reusable classes used to define keywords, commands and syntax highlighting rules for a given language as:

* **FormatDescriptor.cs** - Defines a syntax highlight rule for a section of text that starts and/or ends with a given set of characters.
* **FormatDescriptorType.cs** - Defines the type of syntax highlighting rule such as a Prefix or an Enclosure.
* **KeywordDescriptor.cs** Defines a keyword used in a language, its Type, Description and how it should be highlighted.
* **KeywordType.cs** - Defines the types of keywords such as: Reference Type, Modifier or Selection Statement.
* **LanguageClosure.cs** - Defines a type of closure for a language such as: (), [], "" or ''.
* **LanguageDescriptor.cs** - Holds the definition of a given language, its Keywords, Separators, Escape Character, Formats, Closures and Custom Formatting Commands.
* **LanguageFormatCommand.cs** - A custom menu item that will be [added to the Format Menu](https://developer.xamarin.com/guides/mac/user-interface/working-with-menus/#Creating_Menus_from_Code) when the language is selected. The command can insert text into the document at the current cursor or selection location.
* **LanguageFormatter.cs** - Provides routines for retrieving words, lines or paragraphs from `NSTextView` Attributed Text and to highlight syntax in the [Text View](https://developer.xamarin.com/guides/mac/user-interface/standard-controls/#Working_with_Text_Controls) as the user types.
* **SourceTextView.cs** - A customized `NSTextView` used for syntax highlighting based on the current Language Descriptor.
* **SourceTextViewDelegate.cs** - Handles the UI events for the Source Text View and the Auto Completion of text with the custom set of keywords for the current Language Descriptor.

## Resources

Provides the images and icons used in SourceWriter. All of the icons were provided by [Icons8](https://icons8.com). The icons are free for personal use and also free for commercial use, but they require [linking](https://icons8.com/license/) to to their web site. They distribute them under the license called [Creative Commons Attribution-NoDerivs 3.0 Unported](https://creativecommons.org/licenses/by-nd/3.0/). Alternatively, you can [buy a license](https://icons8.com/paid-license-99/) that doesn't require any linking.

## App, Windows and Controllers

The following classes provide app support, Windows, Windows Controllers, View and View Controller Support in the application:

* **AboutViewController** - Handles the View that is displayed inside the custom About Window.
* **AboutViewController** - Controls the custom About Window that is displayed when the user selects the About SourceWriter Menu Item from the SourceWriter Menu.
* **AppDelegate.cs** - Houses global [User Preferences](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Saving_and_Loading_Preferences), handles opening existing files, new files and recent files and pushes updates to preferences to all open windows. It also handles [informing windows that the app is closing](https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/#Saving_Changes_Before_Closing_the_App) and gives the user a change to save unsaved changes.
* **AutoCompletePrefsController.cs** - Handles the Auto Complete Preference Tab of the [Preferences Dialog](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Creating_a_Preferences_Dialog).
* **DefinitionView.cs** - Handles the View for the Keyword Definition [Sheet](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Creating_a_Custom_Sheet).
* **DefinitionViewController.cs** - Controls the Definition View for the Keyword Definition [Sheet](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Creating_a_Custom_Sheet).
* **EditorPrefsController.cs** - Handles the Editor Preference Tab of the [Preferences Dialog](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Creating_a_Preferences_Dialog).
* **EditorWindowController.cs** - Handles the Editor Window and provides [custom Actions](https://developer.xamarin.com/guides/mac/user-interface/working-with-menus/#Working_with_Custom_Window_Actions) that are tied to [Menu Items](https://developer.xamarin.com/guides/mac/user-interface/working-with-menus/) in Interface Builder. Also handles the [Toolbar Items](https://developer.xamarin.com/guides/mac/user-interface/working-with-toolbars/) for the Window.
* **FormatsPrefsController.cs** - Handles the Formats Preference Tab of the [Preferences Dialog](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Creating_a_Preferences_Dialog). This defines the Syntax Highlighting colors for each supported language.
* **GeneralPrefsController.cs** - Handles the General Preference Tab of the [Preferences Dialog](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Creating_a_Preferences_Dialog).
* **Info.plist** - The [`Info.plist`](https://developer.xamarin.com/guides/mac/getting_started/hello,_mac/#Info.plist) file defines information for the app such as its Title, Type and Icon.
* **License.txt** - Provides the license for this source code. SourceWriter is released under the [MIT License](#License).
* **Main.cs** - The [`Main.cs`](https://developer.xamarin.com/guides/mac/getting_started/hello,_mac/#Main.cs) file starts the App.
* **Main.storyboard** - This [Storyboard](https://developer.xamarin.com/guides/mac/platform-features/storyboards/) defines the User Interface for the app. Use Xcode's Interface Builder to edit.
* **Notes.txt** - Provides some simple notes on the app's source code.
* **PreferenceWindowController.cs** - Controls the [Preference Window](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Creating_a_Preferences_Dialog) and selects the General Preference Tab when the window is first opened.
* **PreviewViewController.cs** - Controls the Preview Web View and provides routines for displaying and updating preview contents.
* **PreviewWindowController.cs** - Controls the Preview Window, provides routines for displaying and updating preview contents, defines [custom Actions](https://developer.xamarin.com/guides/mac/user-interface/working-with-menus/#Working_with_Custom_Window_Actions) wired into [Menu Items](https://developer.xamarin.com/guides/mac/user-interface/working-with-menus/) (in Interface Builder) and controls the [Toolbar Items](https://developer.xamarin.com/guides/mac/user-interface/working-with-toolbars/) for the Window.
* **SearchPrefsController.cs** - Handles the Search Preference Tab of the [Preferences Dialog](https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Creating_a_Preferences_Dialog).
* **TemplateViewController.cs** - Allows the user to select the language type when creating new files and creates a new Editor Window of the correct type.
* **TemplateWindowController.cs** - Handles the Template Window.
* **ViewController.cs** - Handles the View for the Editor Window, enables Keyword Definitions, updates the status bar at the bottom of the Editor Window and handles syntax highlighting based on the selected **Language Descriptor**.

For more detailed descriptions, please see the [API Documentation](html/index.html) for the SourceWriter source code.

<a name="License" class="injected"></a>
# Related Apple Documentation

The following Apple Documents (with Apple's descriptions) are related to topics covered by SourceWriter:

* [Introduction to String Programming Guide for Cocoa](https://developer.apple.com/library/prerelease/mac/documentation/Cocoa/Conceptual/Strings/introStrings.html#//apple_ref/doc/uid/10000035-SW1) - String Programming Guide for Cocoa describes how to create, search, concatenate, and draw strings. It also describes character sets, which let you search a string for characters in a group, and scanners, which convert numbers to strings and vice versa.
* [Introduction to Attributed String Programming Guide](https://developer.apple.com/library/prerelease/mac/documentation/Cocoa/Conceptual/AttributedStrings/AttributedStrings.html#//apple_ref/doc/uid/10000036i) - Attributed String Programming Guide describes the attributed string objects, instantiated from the `NSAttributedString` class or the `CFAttributedString` Core Foundation opaque type, which manage sets of text attributes, such as font and kerning, that are associated with character strings or individual characters.
* [About the Cocoa Text System](https://developer.apple.com/library/prerelease/mac/documentation/TextFonts/Conceptual/CocoaTextArchitecture/Introduction/Introduction.html#//apple_ref/doc/uid/TP40009459) - The Cocoa text system is the primary text-handling system in OS X, responsible for the processing and display of all visible text in Cocoa. It provides a complete set of high-quality typographical services through the text-related AppKit classes, which enable applications to create, edit, display, and store text with all the characteristics of fine typesetting, such as kerning, ligatures, line-breaking, and justification.
* [Introduction to Text Layout Programming Guide](https://developer.apple.com/library/prerelease/mac/documentation/Cocoa/Conceptual/TextLayout/TextLayout.html#//apple_ref/doc/uid/10000158i) - Text Layout Programming Guide describes how the Cocoa text system lays out text. Text layout is the process of converting a string of text characters, font information, and page specifications into lines of glyphs placed at specific locations on a page, suitable for display and printing.
* [Introduction to Text System Storage Layer Overview](https://developer.apple.com/library/prerelease/mac/documentation/Cocoa/Conceptual/TextStorageLayer/TextStorageLayer.html#//apple_ref/doc/uid/10000087i) - Text System Storage Layer Overview discusses the facilities that the Cocoa text system uses to store the text and geometric shape information used for text layout.
* [Introduction to Text Attachments](https://developer.apple.com/library/prerelease/mac/documentation/Cocoa/Conceptual/TextAttachments/TextAttachments.html#//apple_ref/doc/uid/10000094i) - Text Attachments explains how to add graphics and other attachments to text.

And the following Apple Class References (with Apple's descriptions):

* [NSTextView](https://developer.apple.com/library/mac/documentation/Cocoa/Reference/ApplicationKit/Classes/NSTextView_Class/) - The `NSTextView` class is the front-end class to the Application Kit’s text system. 
* [NSTextContainer](https://developer.apple.com/library/mac/documentation/Cocoa/Reference/ApplicationKit/Classes/NSTextContainer_Class/index.html#//apple_ref/doc/c_ref/NSTextContainer) - The `NSTextContainer` class defines a region where text is laid out. 
* [NSTextStorage](https://developer.apple.com/library/mac/documentation/Cocoa/Reference/ApplicationKit/Classes/NSTextStorage_Class/) - `NSTextStorage` is a semi-concrete subclass of `NSMutableAttributedString` that manages a set of client `NSLayoutManager` objects, notifying them of any changes to its characters or attributes so that they can relay and redisplay the text as needed.
* [NSLayoutManager](https://developer.apple.com/library/mac/documentation/Cocoa/Reference/ApplicationKit/Classes/NSLayoutManager_Class/index.html#//apple_ref/occ/cl/NSLayoutManager) - An `NSLayoutManager` object coordinates the layout and display of characters held in an `NSTextStorage` object. 
* [NSParagraphStyle](https://developer.apple.com/library/prerelease/mac/documentation/Cocoa/Reference/ApplicationKit/Classes/NSParagraphStyle_Class/index.html) - The `NSParagraphStyle` class and its subclass `NSMutableParagraphStyle` encapsulate the paragraph or ruler attributes used by the `NSAttributedString` classes.


<a name="License" class="injected"></a>
# License

**The MIT License (MIT)<br/>
Copyright (c) 2016 Xamarin, Inc.**

Permission is hereby granted, free of charge, to any person obtaining a 
copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
OTHER DEALINGS IN THE SOFTWARE.