
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace LayerBackedControls
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
			rotatingButton.Superview.WantsLayer = true;
			// comment this line out for no shadow
			applyShadow();
		}
		
		partial void rotateButton (NSButton sender)
		{
			float rotation = rotatingButton.FrameCenterRotation;
			rotatingButton.FrameCenterRotation = rotation + 15.0f;
		}
		
		partial void beep (NSButton sender)
		{
			AppKitFramework.NSBeep();		
		}
		
		void applyShadow()
		{
			NSShadow shadoe = new NSShadow();
			shadoe.ShadowOffset = new System.Drawing.SizeF(0.0f, -0.0f);
			shadoe.ShadowBlurRadius = 3.0f;
			shadoe.ShadowColor = NSColor.Red;
			rotatingButton.Shadow = shadoe;
		}
	}
}

