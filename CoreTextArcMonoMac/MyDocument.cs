using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace CoreTextArcMonoMac
{
        public partial class MyDocument : MonoMac.AppKit.NSDocument
        {
                // Called when created from unmanaged code
                public MyDocument (IntPtr handle) : base(handle)
                {
                }

                // Called when created directly from a XIB file
                [Export("initWithCoder:")]
                public MyDocument (NSCoder coder) : base(coder)
                {
                }

                public override void WindowControllerDidLoadNib (NSWindowController windowController)
                {
                        base.WindowControllerDidLoadNib (windowController);
                        
                        // Add code to here after the controller has loaded the document window
                        windowController.Window.DidBecomeKey += delegate {
                                NSFontManager.SharedFontManager.Target = this;
                                NSFontPanel.SharedFontPanel.SetPanelFont (arcView.Font, false);
                        };
                        
                        txtField.Changed += delegate(object sender, EventArgs e) {
                                
                                arcView.Title = txtField.StringValue;
                                
                                // you can also use the object that is sent from the NSNotification
                                //NSNotification notification = (NSNotification)sender;
                                //NSTextField field = (NSTextField)notification.Object;
                                //arcView.Title = field.StringValue;
                                
                                updateDisplay ();
                        };
                }

                // 
                // Save support:
                //    Override one of GetAsData, GetAsFileWrapper, or WriteToUrl.
                //

                // This method should store the contents of the document using the given typeName
                // on the return NSData value.
                public override NSData GetAsData (string documentType, out NSError outError)
                {
                        outError = NSError.FromDomain (NSError.OsStatusErrorDomain, -4);
                        return null;
                }

                // 
                // Load support:
                //    Override one of ReadFromData, ReadFromFileWrapper or ReadFromUrl
                //
                public override bool ReadFromData (NSData data, string typeName, out NSError outError)
                {
                        outError = NSError.FromDomain (NSError.OsStatusErrorDomain, -4);
                        return false;
                }

                // If this returns the name of a NIB file instead of null, a NSDocumentController 
                // is automatically created for you.
                public override string WindowNibName {
                        get { return "MyDocument"; }
                }

                public override string DisplayName {
                        get { return arcView.Title; }
                }

                void updateDisplay ()
                {
                        arcView.NeedsDisplay = true;
                        
                        //Update the bold button
                        boldButton.State = isFontBold (arcView.Font) ? NSCellStateValue.On : NSCellStateValue.Off;
                        boldButton.Enabled = canToggleTrait (arcView.Font, NSFontTraitMask.Bold);
                        
                        // Update the italic button
                        italicButton.State = isFontItalic (arcView.Font) ? NSCellStateValue.On : NSCellStateValue.Off;
                        italicButton.Enabled = canToggleTrait (arcView.Font, NSFontTraitMask.Italic);
                        
                        // Update the window title
                        foreach (var controller in WindowControllers)
                                controller.Window.Title = DisplayName;
				}

                private bool isFontBold (NSFont font)
                {
                        return font.FontDescriptor.SymbolicTraits.HasFlag (NSFontSymbolicTraits.BoldTrait);
                }

                private bool isFontItalic (NSFont font)
                {
                        return font.FontDescriptor.SymbolicTraits.HasFlag (NSFontSymbolicTraits.ItalicTrait);
                }

                private bool canToggleTrait (NSFont font, NSFontTraitMask trait)
                {
                        NSFont testFont = null;
                        
                        if ((NSFontTraitMask)((int)font.FontDescriptor.SymbolicTraits & (int)trait) == trait) {
                                testFont = NSFontManager.SharedFontManager.ConvertFontToNotHaveTrait (arcView.Font, trait);
                        } else {
                                testFont = NSFontManager.SharedFontManager.ConvertFont (arcView.Font, trait);
                        }
                        
                        if (testFont != null) {
                                if ((NSFontTraitMask)(testFont.FontDescriptor.SymbolicTraits ^ font.FontDescriptor.SymbolicTraits) == trait) {
                                        return true;
                                }
                        }
                        return false;
                }

                [Export("changeFont:")]
                public void ChangeFont (NSFontManager sender)
                {
                        
                        arcView.Font = sender.ConvertFont (arcView.Font);
                        updateDisplay ();
                }

                [Export("setShowsGlyphOutlines:")]
                public void SetShowsGlyphOutlines (NSButton sender)
                {
                        arcView.ShowsGlyphBounds = sender.State == NSCellStateValue.On;
                        updateDisplay ();
                }

                [Export("setShowsLineMetrics:")]
                public void SetShowsLineMetrics (NSButton sender)
                {
                        arcView.ShowsLineMetrics = sender.State == NSCellStateValue.On;
                        updateDisplay ();
                }

                [Export("setDimsSubstitutedGlyphs:")]
                public void SetDimsSubstitutedGlyphs (NSButton sender)
                {
                        arcView.DimsSubstitutedGlyphs = sender.State == NSCellStateValue.On;
                        updateDisplay ();
                }

                [Export("toggleBold:")]
                public void ToggleBold (NSButton sender)
                {
                        NSFont newFont = null;
                        
                        if (sender.State == NSCellStateValue.On) {
                                newFont = NSFontManager.SharedFontManager.ConvertFont (arcView.Font, NSFontTraitMask.Bold);
                        } else {
                                newFont = NSFontManager.SharedFontManager.ConvertFontToNotHaveTrait (arcView.Font, NSFontTraitMask.Bold);
                        }
                        
                        if (newFont != null) {
                                arcView.Font = newFont;
                                updateDisplay ();
                                NSFontPanel.SharedFontPanel.SetPanelFont (arcView.Font, false);
                        }
                }

                [Export("toggleItalic:")]
                public void ToggleItalic (NSButton sender)
                {
                        NSFont newFont = null;
                        
                        if (sender.State == NSCellStateValue.On) {
                                newFont = NSFontManager.SharedFontManager.ConvertFont (arcView.Font, NSFontTraitMask.Italic);
                        } else {
                                newFont = NSFontManager.SharedFontManager.ConvertFontToNotHaveTrait (arcView.Font, NSFontTraitMask.Italic);
                        }
                        
                        if (newFont != null) {
                                arcView.Font = newFont;
                                updateDisplay ();
                                NSFontPanel.SharedFontPanel.SetPanelFont (arcView.Font, false);
                        }
                }
                
        }
}

