using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MonoMacGameView
{
	public partial class MonoMacGameWindow : AppKit.NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public MonoMacGameWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MonoMacGameWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion
		
		

	}
}

