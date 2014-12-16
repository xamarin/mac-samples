//
// SceneView: simple subclass from SCNView that is a drop-target
// 
using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using AppKit;
using SceneKit;

namespace SceneKitViewer {
	[Register ("SceneView")]
	public class SceneView : SCNView {
#region constructors
		// Called when created from unmanaged code
		public SceneView (IntPtr handle) : base (handle)
		{
			RegisterDND ();
		}

		// Called when loading from Xib files
		[Export ("initWithCoder:")]
		public SceneView (NSCoder coder) : base (coder)
		{
			RegisterDND ();
		}

		// Called if you want to create this programatically
		public SceneView (CGRect rect) : base (rect) 
		{
			RegisterDND ();
		}

		void RegisterDND ()
		{
			RegisterForDraggedTypes (new string [] { NSPasteboard.NSFilenamesType });
		}
#endregion

		public void LoadScene (string path)
		{
			NSError error;
			Scene = SCNScene.FromUrl (new NSUrl ("file://" + path), (SCNSceneLoadingOptions)null, out error);
		}
	}
}