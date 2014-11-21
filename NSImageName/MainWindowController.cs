using System;

using Foundation;
using AppKit;

namespace NSImageNameSample
{
	public partial class MainWindowController : AppKit.NSWindowController
	{
		NSImageName [] imageNameValues = (NSImageName [])Enum.GetValues (typeof (NSImageName));
		int currentImageNameIndex;

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
		
		public override void AwakeFromNib ()
		{
			UpdateImage (0);
			previousButton.Activated += (sender, e) => UpdateImage (-1);
			nextButton.Activated += (sender, e) => UpdateImage (1);
		}
		
		void UpdateImage (int increment)
		{
			currentImageNameIndex += increment;
			
			if (currentImageNameIndex < 0) {
				currentImageNameIndex = imageNameValues.Length - 1;
			} else if (currentImageNameIndex >= imageNameValues.Length) {
				currentImageNameIndex = 0;
			}
			
			imageLabel.StringValue = imageNameValues[currentImageNameIndex].ToString ();
			image.Image = NSImage.ImageNamed (imageNameValues[currentImageNameIndex]);
		}
	}
}