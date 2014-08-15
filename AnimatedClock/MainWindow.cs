
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace AnimatedClock
{
	public partial class MainWindow : AppKit.NSWindow
	{
		public MainWindow (IntPtr handle) : base(handle)
		{
		}

		[Export("initWithCoder:")]
		public MainWindow (NSCoder coder) : base(coder)
		{
		}

		public override void AwakeFromNib ()
		{
			IsOpaque = false;
			BackgroundColor = NSColor.Clear;
			MovableByWindowBackground = true;
			Level = NSWindowLevel.PopUpMenu;
			StyleMask = NSWindowStyle.Borderless;
			IsVisible = true;	
		}
	}
}

