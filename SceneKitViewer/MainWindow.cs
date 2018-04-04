
namespace SceneKitViewer
{
	using System;
	using AppKit;
	using Foundation;

	public partial class MainWindow : NSWindow
	{
		// Called when created from unmanaged code
		public MainWindow(IntPtr handle) : base(handle)
		{
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindow(NSCoder coder) : base(coder)
		{
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

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

			var url = NSBundle.MainBundle.PathForResource("scene", "dae");
			sceneView.LoadScene(url);
		}
	}
}