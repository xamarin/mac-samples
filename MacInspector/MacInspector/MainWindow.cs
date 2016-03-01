using System;
using AppKit;
using Foundation;


namespace MacInspector
{
	/// <summary>
	/// The main window displayed by the app.
	/// </summary>
	[Register("MainWindow")]
	public partial class MainWindow : NSWindow
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.MainWindow"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public MainWindow (IntPtr handle) : base (handle)
		{
		}
		#endregion
	}
}

