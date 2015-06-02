using System;

using Foundation;
using AppKit;

namespace MacDatabase
{
	public partial class MainWindowController : NSWindowController
	{

		#region Computed Properties
		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}
		#endregion

		#region Constructors
		public MainWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}

		public MainWindowController () : base ("MainWindow")
		{
		}
		#endregion

	}
}
