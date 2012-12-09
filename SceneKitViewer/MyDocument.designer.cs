// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace SceneKitViewer
{
	[Register ("MyDocument")]
	partial class MyDocument
	{
		[Outlet]
		SceneKitViewer.SceneView sceneView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (sceneView != null) {
				sceneView.Dispose ();
				sceneView = null;
			}
		}
	}
}
