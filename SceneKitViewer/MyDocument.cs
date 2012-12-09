//
// Handles a toplevel window.
//   * Initializes the sceneView that is in the window (defined on the MyDocument.xib file)
//   * Loads the startup scene
//
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace SceneKitViewer
{
	public partial class MyDocument : MonoMac.AppKit.NSDocument
	{
		// Called when created from unmanaged code
		public MyDocument (IntPtr handle) : base (handle)
		{
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MyDocument (NSCoder coder) : base (coder)
		{
		}

		public override void WindowControllerDidLoadNib (NSWindowController windowController)
		{
			base.WindowControllerDidLoadNib (windowController);

			// Enable user to manipulate the view with the built-in behavior
			sceneView.AllowsCameraControl = true;

			// Improves anti-aliasing when scene is still
			sceneView.JitteringEnabled = true;

			// Play the animations
			sceneView.Playing = true;

			// Automatically light scenes without light
			sceneView.AutoenablesDefaultLighting = true;

			// Background color
			sceneView.BackgroundColor = NSColor.Black;

			var url = NSBundle.MainBundle.PathForResource ("scene", "dae");
			sceneView.LoadScene (url);
		}
		

		// If this returns the name of a NIB file instead of null, a NSDocumentController 
		// is automatically created for you.
		public override string WindowNibName { 
			get {
				return "MyDocument";
			}
		}

	}
}

