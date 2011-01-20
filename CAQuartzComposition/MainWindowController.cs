
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
		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public MainWindowController () : base("MainWindow")
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}
		
		public override void AwakeFromNib ()
		{
			Window.ContentView.WantsLayer = true;
			var path = NSBundle.MainBundle.PathForResource("VideoCube","qtz");
			//var path = NSBundle.MainBundle.PathForResource("VideoQuad","qtz");
			
			var layer = new QCCompositionLayer(path);
			
			layer.Frame = Window.ContentView.Frame;
			
			var text = new CATextLayer();
			text.String = "Hello MonoMac";
			text.Frame = Window.ContentView.Frame;
			layer.AddSublayer(text);
			
			var blurFilter = CIFilter.FromName("CIGaussianBlur");
			
			blurFilter.SetDefaults();
			blurFilter.SetValueForKey((NSNumber)2.0f,(NSString)"inputRadius");
			blurFilter.Name = "blur";
			
			layer.Filters = new NSObject[] {blurFilter};
			
			Window.ContentView.Layer.AddSublayer(layer);
			
		}
	}
}

