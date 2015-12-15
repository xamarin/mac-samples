using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using AppKit;

namespace MacCopyPaste
{
	public class Validate
	{
		#region Static Constants
		static Selector actionSel = new Selector ("action");
		static Selector pasteSel = new Selector ("paste");
		#endregion

		#region Constructors
		public Validate ()
		{
		}
		#endregion

		#region Public Methods
		public bool ValidateUserInterfaceItem (NSObject anItem)
		{
		 	Selector actionSelPtr = new Selector (anItem.PerformSelector (actionSel).Handle);
		 
		 	if (actionSelPtr == pasteSel)
		 		return NSPasteboard.GeneralPasteboard.CanReadObjectForClasses (new Class [] { ( new Class ("NSImage") )} , null);
		 
		 	return true;
		}
		#endregion
	}
}

