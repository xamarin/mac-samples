//
// Handles a toplevel window.
//   * Initializes the sceneView that is in the window (defined on the MyDocument.xib file)
//   * Loads the startup scene
//
using System;
using System.Linq;
using System.Collections.Generic;

using AppKit;
using Foundation;

namespace SceneKitViewer
{
	public partial class MyDocument : AppKit.NSDocument
	{
		// Called when created from unmanaged code
		public MyDocument (IntPtr handle) : base (handle)
		{
		}

		public override void WindowControllerDidLoadNib (NSWindowController windowController)
		{
			base.WindowControllerDidLoadNib (windowController);
			Console.WriteLine ("On callback, sleeping");
			System.Threading.Thread.Sleep (1000);

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

			sceneView.LoadScene ("scene");
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

