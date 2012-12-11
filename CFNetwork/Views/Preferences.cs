
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MonoMac.CFNetwork.Test.Views {

	public partial class Preferences : MonoMac.AppKit.NSWindow {
		#region Constructors
		
		// Called when created from unmanaged code
		public Preferences (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public Preferences (NSCoder coder) : base (coder)
		{
			Console.WriteLine ("PREFERENCES CTOR: {0}", Environment.StackTrace);
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion
	}
}

