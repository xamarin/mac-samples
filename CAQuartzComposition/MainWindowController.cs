
using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using AppKit;
using QuartzComposer;
using CoreImage;
using CoreAnimation;

namespace CAQuartzComposition
{
	public partial class MainWindowController : AppKit.NSWindowController
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

