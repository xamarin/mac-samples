using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace NSTableViewBinding
{
	public partial class EditController : MonoMac.AppKit.NSWindowController
	{
		
		bool cancelled;
		NSMutableDictionary savedFields;
		
		const uint FIRST_NAME = 0;
		const uint LAST_NAME = 1;
		const uint PHONE = 2;
		
		NSApplication NSApp = NSApplication.SharedApplication;	
		
		#region Constructors

		// Called when created from unmanaged code
		public EditController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public EditController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public EditController () : base("TableEdit")
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		//public new TableEdit Window {
		//	get { return (TableEdit)base.Window; }
		//}
		
		public NSMutableDictionary edit(NSDictionary startingValues, TestWindowController sender)
		{
			
			NSWindow window = this.Window;
			
			cancelled = false;
			
			var editFields = editForm.Cells;
			
			if (startingValues != null)
			{
				savedFields = NSMutableDictionary.FromDictionary(startingValues);
				
				editFields[FIRST_NAME].StringValue = startingValues[TestWindowController.FIRST_NAME].ToString();
				editFields[LAST_NAME].StringValue = startingValues[TestWindowController.LAST_NAME].ToString();
				editFields[PHONE].StringValue = startingValues[TestWindowController.PHONE].ToString();
			}
			else
			{
				// we are adding a new entry,
				// make sure the form fields are empty due to the fact that this controller is recycled
				// each time the user opens the sheet -				
				editFields[FIRST_NAME].StringValue = string.Empty;
				editFields[LAST_NAME].StringValue = string.Empty;
				editFields[PHONE].StringValue = string.Empty;
			}
			
			NSApp.BeginSheet(window,sender.Window);  //,null,null,IntPtr.Zero);
			NSApp.RunModalForWindow(window);
			// sheet is up here.....
			
			// when StopModal is called will continue here ....
			NSApp.EndSheet(window);
			window.OrderOut(this);
			
			return savedFields;
		}
		
		partial void done (NSButton sender)
		{
			// save the values for later
			var editFields = editForm.Cells;
			List<NSString> objects = new List<NSString> {new NSString(editFields[FIRST_NAME].StringValue),
														new NSString(editFields[LAST_NAME].StringValue),
														new NSString(editFields[PHONE].StringValue)};

			savedFields = NSMutableDictionary.FromObjectsAndKeys(objects.ToArray(),
			                                                     TestWindowController.Keys.ToArray());
			
			NSApp.StopModal();
		}
		
		/// <summary>
		/// Cancel Action for cancel button
		/// </summary>
		/// <param name="sender">
		/// A <see cref="NSButton"/>
		/// </param>
		partial void cancel (NSButton sender)
		{
			NSApp.StopModal();
			cancelled = true;
		}
		
		/// <summary>
		/// Property for whether the panel was canceled or not.
		/// </summary>
		internal bool Cancelled
		{
			get
			{
				return cancelled;	
			}
		}

	}
}
