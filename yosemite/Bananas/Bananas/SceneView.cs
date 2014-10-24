using System;
using SceneKit;


namespace Bananas
{
	[global::Foundation.Register ("SceneView")]
	public class SceneView : SharedSceneView
	{
		public SceneView (IntPtr handle) : base (handle)
		{
		}
	}
}

