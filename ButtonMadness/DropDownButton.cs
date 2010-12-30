using System;

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace SamplesButtonMadness
{
	public partial class DropDownButton : NSButton
	{
		NSPopUpButtonCell popUpCell;
		
		public DropDownButton (IntPtr handle) : base(handle)
		{
		}
		
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			
			if (this.Menu != null)
				UsesMenu = true;
		}
		
		public bool UsesMenu
		{
			get
			{
				return popUpCell != null;
			}
			
			set
			{
				if (popUpCell == null && value == true) {
					popUpCell = new NSPopUpButtonCell ();
					
					popUpCell.PullsDown = true;
					popUpCell.PreferredEdge = NSRectEdge.MaxYEdge;
				} else if (popUpCell != null && value == false) {
					popUpCell = null;
				}
			}
		}
		
		public override void MouseDown (NSEvent theEvent)
		{
			if (UsesMenu)
			{
				RunPopUp (theEvent);
			}
			else
			{
				base.MouseDown (theEvent);
			}
		}
		
		public void RunPopUp(NSEvent theEvent)
		{
			// create the menu the popup will use
			NSMenu popUpMenu = this.Menu;
			
			if (popUpCell.Count > 0)
			{
				NSMenuItem item = popUpCell [0];
				
				if (item.Title != "")
					popUpMenu.InsertItem ("", null, "", 0);					
			}
			
			popUpCell.Menu = popUpMenu;
    
			// and show it
			popUpCell.PerformClick (Bounds, this);
    
			NeedsDisplay = true;
		}
	}
}

