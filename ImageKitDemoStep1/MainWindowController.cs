
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ImageKit;

namespace ImageKitDemo
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
			browseData.AddImages (NSUrl.FromString("http://danlynch.org/blog/wp-content/uploads/2009/02/mono_logo.png"));
			browseData.AddImages (NSUrl.FromFilename ("/Library/Desktop Pictures/Nature/"));
			browseData.AddImages (NSUrl.FromFilename ("/Library/Desktop Pictures/"));
			browserView.DataSource = browseData;
			browserView.ReloadData();
			//setup delegate for drag and drop
			browserView.DraggingDestinationDelegate = new DragDelegate (browserView);
			// set up event handlers just for testing
			browserView.BackgroundWasRightClicked += delegate { Console.WriteLine ("BackgroundWasRightClicked"); };
			browserView.SelectionDidChange += delegate { Console.WriteLine ("SelectionDidChange"); };
			browserView.CellWasDoubleClicked += delegate { Console.WriteLine ("CellWasDoubleClicked"); };
			browserView.CellWasRightClicked += delegate { Console.WriteLine ("CellWasRightClicked"); };
		}
		private BrowseData browseData = new BrowseData();

		#region interface actions
		partial void SliderChanged (NSSlider sender)
		{
			browserView.ZoomValue = sender.FloatValue;
		}

		partial void SearchTextChanged (NSSearchField sender)
		{
			browseData.SetFilter(sender.StringValue);
			browserView.ReloadData ();
		}

		partial void AddButtonClicked (NSButton sender)
		{
			var panel = NSOpenPanel.OpenPanel;
			panel.FloatingPanel = true;
			panel.CanChooseDirectories = true;
			panel.CanChooseFiles = true;
			//FIXME - create enum for open/save panel return code
			int i = panel.RunModal ();
			if (i == 1 && panel.Urls != null) {
				foreach (NSUrl url in panel.Urls) {
					browseData.AddImages (url);
				}
				browserView.ReloadData ();
			}
		}
		#endregion

	}
}

