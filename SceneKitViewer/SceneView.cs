
namespace SceneKitViewer
{
	using AppKit;
	using CoreAnimation;
	using CoreGraphics;
	using Foundation;
	using SceneKit;
	using System;
	using System.Linq;

	/// <summary>
	/// A subclass of SCNView on which the user can drop .dae files. It handles mouse events to pick 3D objects and highlight their materials. 
	/// </summary>
	[Register("SceneView")]
	public class SceneView : SCNView
	{
		private SCNMaterial selectedMaterial;

		#region constructors

		// Called when created from unmanaged code
		public SceneView(IntPtr handle) : base(handle)
		{
			this.RegisterDraggedTypes();
		}

		// Called when loading from Xib files
		[Export("initWithCoder:")]
		public SceneView(NSCoder coder) : base(coder)
		{
			this.RegisterDraggedTypes();
		}

		// Called if you want to create this programatically
		public SceneView(CGRect rect) : base(rect)
		{
			this.RegisterDraggedTypes();
		}

		private void RegisterDraggedTypes()
		{
			base.RegisterForDraggedTypes(new string[] { NSPasteboard.NSUrlType });
		}

		#endregion

		public void LoadScene(string path)
		{
			NSError error;
			this.selectedMaterial = null;

			// Load the specified scene. First create a dictionary containing the options we want.
			var options = new SCNSceneLoadingOptions
			{
				// Create normals if absent.
				CreateNormalsIfAbsent = true,
				// Optimize the rendering by flattening the scene graph when possible. Note that this would prevent you from animating objects independantly.
				FlattenScene = true,
			};

			var scene = SCNScene.FromUrl(new NSUrl($"file://{path}"), options, out error);
			if (scene != null)
			{
				base.Scene = scene;
			}
			else
			{
				Console.WriteLine($"Problem loading scene from: {path}\n{error.LocalizedDescription}");
			}
		}

		#region Drag and drop

		/// <summary>
		/// Support drag and drop of new dae files.
		/// </summary>
		/// <returns>The operation for pasteboard.</returns>
		/// <param name="pasteboard">Pasteboard.</param>
		private NSDragOperation DragOperationForPasteboard(NSPasteboard pasteboard)
		{
			var result = NSDragOperation.None;

			// Only support drags from .dae files.
			if (pasteboard.Types.Contains(NSPasteboard.NSUrlType))
			{
				var fileURL = NSUrl.FromPasteboard(pasteboard);
				if (fileURL.PathExtension == "dae")
				{
					result = NSDragOperation.Copy;
				}
			}

			return result;
		}

		public override NSDragOperation DraggingEntered(NSDraggingInfo sender)
		{
			return this.DragOperationForPasteboard(sender.DraggingPasteboard);
		}

		public override NSDragOperation DraggingUpdated(NSDraggingInfo sender)
		{
			return this.DragOperationForPasteboard(sender.DraggingPasteboard);
		}

		public override bool PerformDragOperation(NSDraggingInfo sender)
		{
			var result = false;

			var pasteboard = sender.DraggingPasteboard;
			if (pasteboard.Types.Contains(NSPasteboard.NSUrlType))
			{
				var fileURL = NSUrl.FromPasteboard(pasteboard);
				this.LoadScene(fileURL.AbsoluteString);

				result = true;
			}

			return result;
		}

		#endregion

		#region Mouse

		public override void MouseDown(NSEvent theEvent)
		{
			// Convert the mouse location in screen coordinates to local coordinates, then perform a hit test with the local coordinates.
			var mouseLocation = this.ConvertPointFromView(theEvent.LocationInWindow, null);
			var hits = this.HitTest(mouseLocation, (SCNHitTestOptions)null);
    
    		// If there was a hit, select the nearest object; otherwise unselect.
			if (hits.Count() > 0) 
			{
       			var hit = hits[0]; // Choose the nearest object hit.

				this.SelectNode(hit.Node, hit.GeometryIndex);
			}
			else 
			{
				this.SelectNode(null, -1);
			}

			base.MouseDown(theEvent);
		}

		private void SelectNode(SCNNode node, nint index)
		{
			// Unhighlight the previous selection.
			this.selectedMaterial?.Emission?.RemoveAllAnimations();
			// Clear the selection.
			this.selectedMaterial = null;
    
		    // Highight the selection, if there is one.
			if (node != null && index != -1)
			{
		        // Convert the geometry element index to a material index.
				index = index % node.Geometry.Materials.Count();
		        
		        // Make the material unique (i.e. unshared).
				SCNMaterial unsharedMaterial = node.Geometry.Materials[index].Copy() as SCNMaterial;
				node.Geometry.ReplaceMaterial(index, unsharedMaterial);
		        
		        // Select the material.
		        this.selectedMaterial = unsharedMaterial;

				// Animate the material.
				var highlightAnimation = CABasicAnimation.FromKeyPath("contents") as CABasicAnimation;
				highlightAnimation.To = NSColor.Blue;
				highlightAnimation.From = NSColor.Black;
				highlightAnimation.RepeatCount = float.MaxValue;
				highlightAnimation.AutoReverses = true;
				highlightAnimation.Duration = 0.5;
				highlightAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);

				this.selectedMaterial.Emission.AddAnimation(highlightAnimation, new NSString("highlight"));
		    }
		}

		#endregion
	}
}