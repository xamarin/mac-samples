
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.QuartzComposer;
using MonoMac.CoreGraphics;
using MonoMac.CoreImage;
using MonoMac.CoreAnimation;

namespace CAQuartzComposition
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		public MainWindowController (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base(coder) {}

		public MainWindowController () : base("MainWindow") {}

		//strongly typed window accessor
		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}
		
	}
}

