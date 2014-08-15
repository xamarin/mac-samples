
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace LayerBackedControls
{
	public partial class MainWindowController : AppKit.NSWindowController
	{
		public MainWindowController (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base(coder) {}

		public MainWindowController () : base("MainWindow") {}

		public override void AwakeFromNib ()
		{
			rotatingButton.Superview.WantsLayer = true;
			// comment this line out for no shadow
			ApplyShadow();
		}
		
		partial void RotateButton (NSButton sender)
		{
			var rotation = rotatingButton.FrameCenterRotation;
			rotatingButton.FrameCenterRotation = rotation + 15.0f;
		}
		
		partial void Beep (NSButton sender)
		{
			AppKitFramework.NSBeep ();		
		}
		
		void ApplyShadow ()
		{
			rotatingButton.Shadow = new NSShadow() {
				ShadowOffset = new System.Drawing.SizeF (0, 0),
				ShadowBlurRadius = 3,
				ShadowColor = NSColor.Red
			};
		}
	}
}

