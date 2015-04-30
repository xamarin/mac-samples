using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace MacXibless
{
	public partial class MainWindow : NSWindow
	{
		#region Private Variables
		private int NumberOfTimesClicked = 0;
		#endregion

		#region Computed Properties
		public NSButton ClickMeButton { get; set;}
		public NSTextField ClickMeLabel { get ; set;}
		#endregion

		#region Constructors
		public MainWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
		}

		public MainWindow(CGRect contentRect, NSWindowStyle aStyle, NSBackingStore bufferingType, bool deferCreation): base (contentRect, aStyle,bufferingType,deferCreation) {
			// Define the User Interface of the Window here
			Title = "Window From Code";

			// Create the content view for the window and make it fill the window
			ContentView = new NSView (Frame);

			// Add UI Elements to window
			ClickMeButton = new NSButton (new CGRect (10, Frame.Height-70, 100, 30)){
				AutoresizingMask = NSViewResizingMask.MinYMargin
			};
			ContentView.AddSubview (ClickMeButton);

			ClickMeLabel = new NSTextField (new CGRect (120, Frame.Height - 65, Frame.Width - 130, 20)) {
				BackgroundColor = NSColor.Clear,
				TextColor = NSColor.Black,
				Editable = false,
				Bezeled = false,
				AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.MinYMargin,
				StringValue = "Button has not been clicked yet."
			};
			ContentView.AddSubview (ClickMeLabel);
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Wireup events
			ClickMeButton.Activated += (sender, e) => {
				// Update count
				ClickMeLabel.StringValue = (++NumberOfTimesClicked == 1) ? "Button clicked one time." : string.Format("Button clicked {0} times.",NumberOfTimesClicked);
			};
		}
		#endregion

	}
}
