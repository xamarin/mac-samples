using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2013 {
	public class Slide : NSObject {
		public SCNNode ContentNode { get; set; }

		public SCNNode GroundNode { get; set; }

		public SlideTextManager TextManager { get; set; }

		public float[] LightIntensities { get; set; }

		public SCNVector3 MainLightPosition { get; set; }

		public bool EnableShadows { get; set; }

		public SCNMaterial FloorWarmupMaterial { get; set; }
		// used to retain a material to prevent it from being released before the slide is presented. This used for preloading and caching.
		public string FloorImageName { get; set; }

		public string FloorImageExtension { get; set; }

		public float FloorReflectivity { get; set; }

		public float FloorFalloff { get; set; }

		public float TransitionDuration { get; set; }

		public float TransitionOffsetX { get; set; }

		public float TransitionOffsetZ { get; set; }

		public float TransitionRotation { get; set; }

		public float Altitude { get; set; }

		public float Pitch { get; set; }

		public bool IsNewIn10_9 { get; set; }

		public Slide ()
		{
			ContentNode = SCNNode.Create ();

			GroundNode = SCNNode.Create ();
			ContentNode.AddChildNode (GroundNode);

			TextManager = new SlideTextManager ();
			ContentNode.AddChildNode (TextManager.TextNode);

			// Default parameters
			LightIntensities = new float[] { 1.0f };
			MainLightPosition = new SCNVector3 (0, 3, -13);
			EnableShadows = false;
			FloorImageName = null;
			FloorImageExtension = null;
			FloorReflectivity = 0.25f;
			FloorFalloff = 3.0f;
			TransitionDuration = 1.0f;
			TransitionOffsetX = 0.0f;
			TransitionOffsetZ = 0.0f;
			TransitionRotation = 0.0f;
			Altitude = 5.0f;
			Pitch = 0.0f;
			IsNewIn10_9 = false;
		}

		public void Fetch (Slide slide)
		{
			LightIntensities = slide.LightIntensities;
			MainLightPosition = slide.MainLightPosition;
			EnableShadows = slide.EnableShadows;
			if (slide.FloorImageName != null) {
				FloorImageName = slide.FloorImageName.Split ('.') [0];
				FloorImageExtension = slide.FloorImageName.Split ('.') [1];
			}
			FloorReflectivity = slide.FloorReflectivity;
			FloorFalloff = slide.FloorFalloff;
			TransitionDuration = slide.TransitionDuration;
			TransitionOffsetX = slide.TransitionOffsetX;
			TransitionOffsetZ = slide.TransitionOffsetZ;
			TransitionRotation = slide.TransitionRotation;
			Altitude = slide.Altitude;
			Pitch = slide.Pitch;
			IsNewIn10_9 = slide.IsNewIn10_9;
		}

		public virtual int NumberOfSteps ()
		{
			return 0;
		}

		public virtual void SetupSlide (PresentationViewController presentationViewController)
		{
		}

		public virtual void PresentStep (int index, PresentationViewController presentationViewController)
		{
		}

		public virtual void WillOrderOut (PresentationViewController presentationViewController)
		{
		}

		public virtual void DidOrderIn (PresentationViewController presentationViewController)
		{
		}
	}
}

