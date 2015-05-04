using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacControls
{
	public partial class SubviewChecksRadioController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewChecksRadioController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewChecksRadioController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SubviewChecksRadioController () : base ("SubviewChecksRadio", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new SubviewChecksRadio View {
			get {
				return (SubviewChecksRadio)base.View;
			}
		}
	}
}
