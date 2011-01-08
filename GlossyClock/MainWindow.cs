
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace GlossyClock
{
	public partial class MainWindow : MonoMac.AppKit.NSWindow
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

