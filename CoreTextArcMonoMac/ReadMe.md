CoreTextArcMonoMac
==================

Illustrates how to use CoreText to draw text along an arc in a Cocoa MonoMac application.

Converted to Mono and C# by:  Kenneth J. Pouncey 2011/02/05
Original Converted from Apple's sample "CoreTextArcCocoa"

The main drawing functionality demonstrated in this application is implemented in a custom NSView called CoreTextArcView.  
And, all of the interresting functionality in that view is encapsulated in the  -drawRect: method in the CoreTextArcView.cs file.  
There, CoreText is used to layout and draw glyphs along a curve.

NSFontPanel
===========

This sample also makes use of the NSFontPanel to allow user configuration of the text being displayed in the custom view.  
This functionality can be found in the file MyDocument.cs.  Key points to make note of in that file are:

1. The font panel is synchronized with the current font settings for the custom view in the -windowDidBecomeKey: method.
  The Font Panel is a shared resource that stays on screen and calls methods on the first responder to communicate changes 
  in the font selection.  By placing the synchronization code inside of the -windowDidBecomeKey: method, the application 
  is able to make sure the font panel settings are accurately reflected for the state of the document whenever the it 
  becomes the first responder.

2. The -changeFont: method on the MyDocument class is called by the font panel whenever the user selects and new font setting.
  This method receives the new font settings and changes the font settings for the custom view.

3. The methods -toggleBold: and -toggleItalic: are called in response to user clicks in the italic and bold checkboxes.
  In these methods, the respective font attributes are changed and then the current settings are synchronized to the font pane 
  and to the custom view.


Using the Sample
================

Build and run this sample.  When launched, the application will 
display a string drawn along an curve.  Click in the checkboxes in wthe window to change some font settings.  Choose the 
"Show Fonts" menu item from the "Format" menu to open the font panel so you can change additional font settings.

