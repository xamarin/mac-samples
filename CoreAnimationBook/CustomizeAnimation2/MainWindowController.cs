
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;

namespace CustomizeAnimation
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
		
		partial void makeSlow (NSButton sender)
		{
			CABasicAnimation frameOriginAnimation = new CABasicAnimation();
			frameOriginAnimation.Duration = 2.0f;
			NSDictionary animations = NSDictionary.FromObjectAndKey(frameOriginAnimation,
			                                                        (NSString)"frameOrigin");
			myView.Mover.Animations = animations;
		}
		
		partial void makeFast (NSButton sender)
		{
			CABasicAnimation frameOriginAnimation = new CABasicAnimation();
			frameOriginAnimation.Duration = 0.1f;
			NSDictionary animations = NSDictionary.FromObjectAndKey(frameOriginAnimation,
			                                                        (NSString)"frameOrigin");
			myView.Mover.Animations = animations;
		}
		
		partial void makeDefault (NSButton sender)
		{
			myView.Mover.Animations = new NSDictionary();
		}
	}
}

