
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace SamplesButtonMadness
{
	public partial class TestWindowController : MonoMac.AppKit.NSWindowController
	{
		#region members
		
		NSPopUpButton codeBasedPopUpDown;
		NSPopUpButton codeBasedPopUpRight;
		
		#endregion
		
		#region Constructors

		// Called when created from unmanaged code
		public TestWindowController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public TestWindowController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public TestWindowController () : base("TestWindow")
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new TestWindow Window {
			get { return (TestWindow)base.Window; }
		}
		
		#region implementation
		
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			
			// add the image menu item back to the first menu item
			NSMenuItem menuItem = new NSMenuItem (@"", new Selector (@""), @"");
			
			menuItem.Image = NSImage.ImageNamed (@"moof.png");
			buttonMenu.InsertItematIndex (menuItem, 0);
			
			nibBasedPopUpDown.Menu = buttonMenu;
			nibBasedPopUpRight.Menu = buttonMenu;
		
			// create the pull down button pointing DOWN
			RectangleF buttonFrame = placeHolder1.Frame;
			codeBasedPopUpDown = new NSPopUpButton (buttonFrame, true);
			
			((NSPopUpButtonCell)codeBasedPopUpDown.Cell).ArrowPosition = NSPopUpArrowPosition.AtBottom;
			((NSPopUpButtonCell)codeBasedPopUpDown.Cell).BezelStyle = NSBezelStyle.ThickSquare;
			codeBasedPopUpDown.Menu = buttonMenu;
			popupBox.AddSubview (codeBasedPopUpDown);
			placeHolder1.RemoveFromSuperview ();
		
			// create the pull down button pointing RIGHT
			buttonFrame = placeHolder2.Frame;
			codeBasedPopUpRight = new NSPopUpButton (buttonFrame, true);
			
			((NSPopUpButtonCell)codeBasedPopUpRight.Cell).ArrowPosition = NSPopUpArrowPosition.AtBottom;
			((NSPopUpButtonCell)codeBasedPopUpRight.Cell).PreferredEdge = NSRectEdge.MaxXEdge;
			((NSPopUpButtonCell)codeBasedPopUpRight.Cell).BezelStyle = NSBezelStyle.Circular;
			codeBasedPopUpRight.Menu = buttonMenu;
			((NSPopUpButtonCell)codeBasedPopUpRight.Cell).HighlightsBy = (int)NSCellMask.ChangeGrayCell;
			popupBox.AddSubview (codeBasedPopUpRight);
			placeHolder2.RemoveFromSuperview ();
			
			
		}
		
		#endregion
		
		#region event handlers
		
		partial void dropDownAction (NSObject sender)
		{
			Console.WriteLine (@"Drop down button clicked");
		}
		
		#endregion
	}
}

