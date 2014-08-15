
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace Test_Hello_Mac
{
	public partial class MainWindow : AppKit.NSWindow
	{
		protected int numberOfTimesClicked = 0;

		#region Constructors
		
		// Called when created from unmanaged code
		public MainWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			ClickMeButton.Activated += (object sender, EventArgs e) => {
				numberOfTimesClicked++;
				OutputLabel.StringValue = "Clicked " + numberOfTimesClicked + " times.";
			};
		}
		#endregion
	}
}

