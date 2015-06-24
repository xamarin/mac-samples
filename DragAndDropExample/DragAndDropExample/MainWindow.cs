using System;

using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;

namespace DragAndDropExample
{
	public partial class MainWindow : NSWindow
	{
		#region Private Variables
		private SourceView source;
		private DestView dest;
		#endregion

		#region Constructors
		public MainWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
		}
		#endregion
			
		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Calculate the center of the window
			const int padding = 25;
			nfloat viewWidth = (this.ContentView.Frame.Width / 2) - padding;

			// Add a drag source view in the first half
			source = new SourceView (new CGRect (0, 0, viewWidth, this.ContentView.Frame.Height));
			ContentView.AddSubview (source);

			// Add a drag destination in the second half
			dest = new DestView (new CGRect (viewWidth + 2 * padding, 0, viewWidth, this.ContentView.Frame.Height));
			ContentView.AddSubview (dest);
		}
		#endregion
	}
}
