using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using AppKit;

namespace MacCopyPaste
{
	/// <summary>
	/// https://developer.apple.com/library/prerelease/mac/documentation/Cocoa/Conceptual/UIValidation/Articles/implementingValidation.html#//apple_ref/doc/uid/TP40006268-SW1
	/// </summary>
	public class Validate
	{
		public Validate ()
		{
		}

		// This class includes a work around to fix a binding issue in the current version of Xamarin.Mac.
		// A new version should be released soon that fixes this issue and the sample will be updated.
		[DllImport (ObjCRuntime.Constants.ObjectiveCLibrary, EntryPoint="objc_msgSend")]
		internal extern static IntPtr intptr_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (ObjCRuntime.Constants.ObjectiveCLibrary, EntryPoint="objc_msgSend")]
		internal extern static bool bool_objc_msgSend_intptr_intptr (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		bool ValidateUserInterfaceItem (NSObject anItem)
		{
			var canReadSel = new Selector ("canReadObjectForClasses:options:");
			var pasteSel = new Selector ("paste:");
			var actionSel = new Selector ("action");

			var actionSelPtr = intptr_objc_msgSend (anItem.Handle, actionSel.Handle);
			var objActionSel = new Selector (actionSelPtr);

			if (objActionSel == pasteSel) {
				var pasteboard = NSPasteboard.GeneralPasteboard;
				var classArrayPtrs = new [] { Class.GetHandle (typeof(NSImage)) };
				var classArray = NSArray.FromIntPtrs (classArrayPtrs);
				var options = new NSDictionary ();
				return bool_objc_msgSend_intptr_intptr (pasteboard.Handle, canReadSel.Handle, classArray.Handle, options.Handle);
			}
			//base.ValidateUserInterfaceItem (NSObject anItem);

			return true;
		}

		// This is the version of the code after the binding issue has been fixed
		// and released to the Alpha channel of Xamarin.Mac.
		// static Selector actionSel = new Selector ("action");
		// static Selector pasteSel = new Selector ("paste");
		// 
		// bool ValidateUserInterfaceItem (NSObject anItem)
		// {
		// 	Selector actionSelPtr = new Selector (anItem.PerformSelector (actionSel).Handle);
		// 
		// 	if (actionSelPtr == pasteSel)
		// 		return NSPasteboard.GeneralPasteboard.CanReadObjectForClasses (new Class [] { ( new Class ("NSImage") )} , null);
		// 
		// 	return true;
		// }
	}
}

