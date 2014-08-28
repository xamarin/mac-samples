using System;
using System.Runtime.InteropServices;

using AppKit;
using SceneKit;
using CoreImage;
using Foundation;
using CoreGraphics;
using CoreAnimation;
using CoreFoundation;

namespace SceneKitSessionWWDC2013
{
	public class SlideCoreImage : Slide
	{
		const int ContactImageCount = 44;
		const int RowCount = 4;
		const int ColumnCount = 11;

		private SCNNode GroupNode { get; set; }

		private SCNNode HeroNode { get; set; }

		private CGSize ViewportSize { get; set; }

		public override int NumberOfSteps ()
		{
			return 5;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			BuildImageGrid ();
			ViewportSize = ((SCNView)presentationViewController.View).ConvertSizeToBacking (((SCNView)presentationViewController.View).Frame.Size);
		}

		public override void PresentStep (int switchIndex, PresentationViewController presentationViewController)
		{
			switch (switchIndex) {
			case 0:
				// Set the slide's title and subtitle and add some text
				TextManager.SetTitle ("Core Image");
				TextManager.SetSubtitle ("CI Filters");

				TextManager.AddBulletAtLevel ("Screen-space effects", 0);
				TextManager.AddBulletAtLevel ("Applies to a node hierarchy", 0);
				TextManager.AddBulletAtLevel ("Filter parameters are animatable", 0);
				TextManager.AddCode ("#aNode.#Filters# = new CIFilter[] { filter1, filter2 };#");
				break;
			case 1:
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				// Dim the text and move back a little
				TextManager.TextNode.Opacity = 0.0f;
				presentationViewController.CameraHandle.Position = presentationViewController.CameraNode.ConvertPositionToNode (new SCNVector3 (0, 0, 5.0f), presentationViewController.CameraHandle.ParentNode);
				SCNTransaction.Commit ();

				// Reveal the grid
				GroupNode.Opacity = 1;
				break;
			case 2:
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				// Highlight an item
				HighlightContact (13, presentationViewController);
				SCNTransaction.Commit ();
				break;
			case 3:
				var index = 13;
				var subStep = 0;

				// Successively select items
				for (var i = 0; i < 5; ++i) {
					var popTime = new DispatchTime (DispatchTime.Now, (Int64)(i * 0.2 * Utils.NSEC_PER_SEC));
					DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
						SCNTransaction.Begin ();
						SCNTransaction.AnimationDuration = 0.2f;
						UnhighlightContact (index);

						if (subStep++ == 3)
							index += ColumnCount;
						else
							index++;

						HighlightContact (index, presentationViewController);
						SCNTransaction.Commit ();
					});
				}
				break;
			case 4:
				// BLUR+DESATURATE in the background, GLOW in the foreground

				// Here we will change the node hierarchy in order to group all the nodes in the background under a single node.
				// This way we can use a single Core Image filter and apply it on the whole grid, and have another CI filter for the node in the foreground.

				var selectionParent = HeroNode.ParentNode;

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0;
				// Stop the animations of the selected node
				HeroNode.Transform = HeroNode.PresentationNode.Transform; // set the current rotation to the current presentation value
				HeroNode.RemoveAllAnimations ();

				// Re-parent the node by preserving its world tranform
				var wantedWorldTransform = selectionParent.WorldTransform;
				GroupNode.ParentNode.AddChildNode (selectionParent);
				selectionParent.Transform = selectionParent.ParentNode.ConvertTransformFromNode (wantedWorldTransform, null);
				SCNTransaction.Commit ();

				// Add CIFilters

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				// A negative 'centerX' value means no scaling.
				//TODO HeroNode.Filters [0].SetValueForKey (new NSNumber (-1), new NSString ("centerX"));

				// Move the selection to the foreground
				selectionParent.Rotation = new SCNVector4 (0, 1, 0, 0);
				HeroNode.Transform = ContentNode.ConvertTransformToNode (SCNMatrix4.CreateTranslation (0, Altitude, 29), selectionParent);
				HeroNode.Scale = new SCNVector3 (1, 1, 1);
				HeroNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 4) * 0.25f);

				// Upon completion, rotate the selection forever
				SCNTransaction.SetCompletionBlock (() => {
					var animation = CABasicAnimation.FromKeyPath ("rotation");
					animation.Duration = 4.0f;
					animation.From = NSValue.FromVector (new SCNVector4 (0, 1, 0, 0));
					animation.To = NSValue.FromVector (new SCNVector4 (0, 1, 0, NMath.PI * 2));
					animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
					animation.RepeatCount = float.MaxValue;

					HeroNode.ChildNodes [0].AddAnimation (animation, new NSString ("heroNodeAnimation"));
				});

				// Add the filters
				var blurFilter = CIFilter.FromName ("CIGaussianBlur");
				blurFilter.SetDefaults ();
				blurFilter.Name = "blur";
				blurFilter.SetValueForKey (new NSNumber (0), CIFilterInputKey.Radius);

				var desaturateFilter = CIFilter.FromName ("CIColorControls");
				desaturateFilter.SetDefaults ();
				desaturateFilter.Name = "desaturate";
				GroupNode.Filters = new CIFilter[] { blurFilter, desaturateFilter };
				SCNTransaction.Commit ();

				// Increate the blur radius and desaturate progressively
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 2;
				GroupNode.SetValueForKey (new NSNumber (10), new NSString ("filters.blur.inputRadius"));
				GroupNode.SetValueForKey (new NSNumber (0.1), new NSString ("filters.desaturate.inputSaturation"));
				SCNTransaction.Commit ();

				break;
			}
		}

		private void BuildImageGrid ()
		{
			// Create a root node for the grid
			GroupNode = SCNNode.Create ();

			// Retrieve the template node to replicate
			var scene = SCNScene.FromFile ("Contacts/contact");
			var templateNode = scene.RootNode.FindChildNode ("people", true);

			for (int k = 0, j = 0; j < RowCount; j++) {
				for (var i = 0; i < ColumnCount; i++, k++) {

					// Hierarchy : __groupNode > container > node
					var container = SCNNode.Create ();
					var node = templateNode.Clone ();
					node.Name = "contact" + k;

					GroupNode.AddChildNode (container);
					container.AddChildNode (node);

					if (k == 28)
						HeroNode = node;

					// Curved layout
					var angle = 0.12f * ((ColumnCount - 1) / 2.0f - i);
					var x = NMath.Cos (angle + (float)(Math.PI / 2)) * 500.0f;
					var z = NMath.Sin (angle + (float)(Math.PI / 2)) * 500.0f;
					container.Position = new SCNVector3 (x, j * 60, -z + 400);
					container.Rotation = new SCNVector4 (0, 1, 0, angle);

					// We want a different image on each elemement and to do that we need to
					// unshare the geometry first and then unshare the material

					var geometryNode = node.ChildNodes [0];
					geometryNode.Geometry = (SCNGeometry)geometryNode.Geometry.Copy ();

					var materialCopy = (SCNMaterial)geometryNode.Geometry.Materials [1].Copy ();
					materialCopy.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Contacts/contact" + (k % ContactImageCount), "jpg"));
					geometryNode.Geometry.ReplaceMaterial (1, materialCopy);

					// Animate (rotate forever)
					var animation = CAKeyFrameAnimation.GetFromKeyPath ("rotation");
					animation.Duration = 4.0f;
					animation.KeyTimes = new NSNumber[] { 0.0f, 0.3f, 1.0f };
					animation.Values = new NSObject[] { NSValue.FromVector (new SCNVector4 (0, 1, 0, 0)),
						NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2))),
						NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2)))
					};

					var tf = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
					animation.TimingFunctions = new CAMediaTimingFunction[] { tf, tf, tf };
					animation.RepeatCount = float.MaxValue;
					animation.BeginTime = CAAnimation.CurrentMediaTime () + 1.0f + j * 0.1f + i * 0.05f; // desynchronize the animations
					node.AddAnimation (animation, new NSString ("animation"));
				}
			}

			// Add the group to the scene
			GroupNode.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
			GroupNode.Position = new SCNVector3 (0, Altitude - 2.8f, 18);
			GroupNode.Opacity = 0.0f;

			GroundNode.AddChildNode (GroupNode);
		}

		private void UnhighlightContact (int index)
		{
			var contactNode = GroundNode.FindChildNode ("contact" + index, true);
			//TODO contactNode.Filters = null;

			// Restore the original position and scale
			contactNode.Scale = new SCNVector3 (1, 1, 1);
			contactNode.Position = new SCNVector3 (contactNode.Position.X, contactNode.Position.Y, contactNode.Position.Z - 50);
		}

		// Highlight the node at index 'index' by setting a CI filter
		private void HighlightContact (int index, PresentationViewController presentationViewController)
		{
			// Create a filter
			var glowFilter = new SCGlowFilter ();
			glowFilter.Name = "aGlow";
			glowFilter.SetDefaults ();

			// Retrieve the node to highlight
			// Scale up and move to front a little
			var contactNode = GroupNode.FindChildNode ("contact" + index, true);
			contactNode.Scale = new SCNVector3 (1.2f, 1.2f, 1.2f);
			contactNode.Position = new SCNVector3 (contactNode.Position.X, contactNode.Position.Y, contactNode.Position.Z + 50);

			// Compute the screenspace position of this node because the glow filter needs this info
			var worldPosition = contactNode.ConvertPositionToNode (new SCNVector3 (0, 0, 0), null);
			var screenPosition = ((SCNView)presentationViewController.View).ProjectPoint (worldPosition);
			var screenPositionInPixels = ((SCNView)presentationViewController.View).ConvertPointToBacking (new CGPoint (screenPosition.X, screenPosition.Y));

			glowFilter.CenterX = screenPositionInPixels.X;
			glowFilter.CenterY = screenPositionInPixels.Y;

			glowFilter.InputRadius = new NSNumber (10);

			// Set the filter
			// TODO contactNode.Filters = new CIFilter[] { glowFilter };

			// Animate the radius parameter of the glow filter
			/* TODO CABasicAnimation animation = CABasicAnimation.FromKeyPath ("filters.aGlow.inputRadius");
			animation.To = new NSNumber (20);
			animation.From = new NSNumber (10);
			animation.AutoReverses = true;
			animation.RepeatCount = float.MaxValue;
			animation.Duration = 1.0f;
			animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
			contactNode.AddAnimation (animation, new NSString ("filterAnimation"));*/
		}
	}

	public class SCGlowFilter : CIFilter
	{
		public CIImage InputImage { get; set; }

		public NSNumber InputRadius { get; set; }

		public nfloat CenterX { get; set; }

		public nfloat CenterY { get; set; }

		public CIImage OutputImage ()
		{
			var inputImage = ValueForKey (new NSString ("inputImage"));
			if (inputImage == null)
				return null;

			// Monochrome
			var monochromeFilter = CIFilter.FromName ("CIColorMatrix");
			monochromeFilter.SetDefaults ();
			monochromeFilter.SetValueForKey (CIVector.Create (0, 0, 0), new NSString ("inputRVector"));
			monochromeFilter.SetValueForKey (CIVector.Create (0, 0, 0.4f), new NSString ("inputGVector"));
			monochromeFilter.SetValueForKey (CIVector.Create (0, 0, 1), new NSString ("inputBVector"));
			monochromeFilter.SetValueForKey (CIVector.Create (0, 0, 1), new NSString ("inputAVector"));
			monochromeFilter.SetValueForKey (inputImage, new NSString ("inputImage"));
			var glowImage = (CIImage)monochromeFilter.ValueForKey (new NSString ("outputImage"));

			// Scale
			var centerX = CenterX;
			var centerY = CenterY;
			if (centerX > 0) {
				var transform = new NSAffineTransform ();
				transform.Translate (centerX, centerY);
				transform.Scale (1.2f);
				transform.Translate (-centerX, -centerY);

				var affineTransformFilter = CIFilter.FromName ("CIAffineTransform");
				affineTransformFilter.SetDefaults ();
				affineTransformFilter.SetValueForKey (transform, new NSString ("inputTransform"));
				affineTransformFilter.SetValueForKey (glowImage, new NSString ("inputImage"));
				glowImage = (CIImage)affineTransformFilter.ValueForKey (new NSString ("outputImage"));
			}

			// Blur
			var gaussianBlurFilter = CIFilter.FromName ("CIGaussianBlur");
			gaussianBlurFilter.SetDefaults ();
			gaussianBlurFilter.SetValueForKey (glowImage, new NSString ("inputImage"));
			gaussianBlurFilter.SetValueForKey (InputRadius != null ? InputRadius : new NSNumber (10.0f), new NSString ("inputRadius"));
			glowImage = (CIImage)gaussianBlurFilter.ValueForKey (new NSString ("outputImage"));

			// Blend
			var blendFilter = CIFilter.FromName ("CISourceOverCompositing");
			blendFilter.SetDefaults ();
			blendFilter.SetValueForKey (glowImage, new NSString ("inputBackgroundImage"));
			blendFilter.SetValueForKey (blendFilter, new NSString ("inputImage"));
			glowImage = (CIImage)blendFilter.ValueForKey (new NSString ("outputImage"));

			return glowImage;
		}
	}
}