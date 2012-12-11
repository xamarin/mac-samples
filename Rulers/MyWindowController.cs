
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Rulers
{
	public partial class MyWindowController : MonoMac.AppKit.NSWindowController
	{
		#region Constructors

		// Called when created from unmanaged code
		public MyWindowController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MyWindowController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public MyWindowController () : base("MyWindow")
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new MyWindow Window {
			get { return (MyWindow)base.Window; }
		}
		

	}
}

