using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreVideo;
using MonoMac.OpenGL;

namespace GLFullScreen
{
	public class Scene : NSObject
	{
		Texture texture;
		int textureName;

		Sphere sphere;

		float animationPhase;
		float rollAngle;
		float sunAngle;
		bool wireFrame;

		static float[] lightDirection = new float[] { -0.7071f, 0.0f, 0.7071f, 0.0f };
		static float radius = 0.25f;
		static float[] materialAmbient = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };
		static float[] materialDiffuse = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };

		public Scene () : base()
		{
			textureName = 0;
			animationPhase = 0;
			rollAngle = 0;
			sunAngle = 135;
			wireFrame = false;
		}

		public float RollAngle {
			get { return rollAngle; }
			set { rollAngle = value; }
		}

		public float SunAngle {
			get { return sunAngle; }
			set { sunAngle = value; }
		}

		public void advanceTimeBy (float seconds)
		{
			float phaseDelta = seconds - (float)Math.Floor (seconds);
			//float newAnimationPhase = (float)(animationPhase + 0.015625 * phaseDelta);
			float newAnimationPhase = (float)(animationPhase + 0.000500 * phaseDelta);
			newAnimationPhase = newAnimationPhase - (float)Math.Floor (newAnimationPhase);
			animationPhase = newAnimationPhase;
		}

		public void toggleWireFrame ()
		{
			wireFrame = !wireFrame;
		}

		public void setViewportRect (RectangleF bounds)
		{
			
			GL.Viewport (0, 0, (int)bounds.Size.Width, (int)bounds.Size.Height);
			GL.MatrixMode (MatrixMode.Projection);
			GL.LoadIdentity ();
			
			// Set perspective here - Calculate The Aspect Ratio Of The Window
			Geometry.Perspective (30, bounds.Size.Width / bounds.Size.Height, 0.1, 100);
			
			GL.MatrixMode (MatrixMode.Modelview);
			
			setupSphere ();
		}

		void setupSphere ()
		{
			if (sphere != null)
				sphere.Dispose ();
			
			sphere = new Sphere (radius, 48, 24, true);
		}

		// This method renders our scene.
		// We could optimize it in any of several ways, including factoring out the repeated OpenGL initialization calls and 
		// hanging onto the GLU quadric object, but the details of how it's implemented aren't important here. 
		// The main thing to note is that we've factored the drawing code out of the NSView subclass so that
		// the full-screen and non-fullscreen views share the same states for rendering 
		// (and MainController can use it when rendering in full-screen mode on pre-10.6 systems).
		public void render ()
		{
			
			GL.Enable (EnableCap.DepthTest);
			GL.Enable (EnableCap.CullFace);
			GL.Enable (EnableCap.Lighting);
			GL.Enable (EnableCap.Light0);
			GL.Enable (EnableCap.Texture2D);
			
			GL.ClearColor (0, 0, 0, 0);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
			// Upload the texture
			// Since we are sharing OpenGL objects between the full-screen and non-fullscreen contexts, we only need to do this once
			if (textureName == 0) {
				var path = NSBundle.MainBundle.PathForResource ("Earth", "jpg");
				texture = new Texture (path);
				textureName = texture.TextureName;
			}
			
			// Set up texturing parameters
			GL.BindTexture (TextureTarget.Texture2D, textureName);
			GL.TexEnv (TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)All.Modulate);
			
			lightDirection[0] = (float)Math.Cos (degreesToRadians (SunAngle));
			lightDirection[2] = (float)Math.Sin (degreesToRadians (SunAngle));
			GL.Light (LightName.Light0, LightParameter.Position, lightDirection);
			
			GL.PushMatrix ();
			
			// Back the camera off a bit
			GL.Translate (0, 0, -1.5f);
			
			// Draw the Earth!
			if (wireFrame)
				GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Line);
			else
				GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Fill);
			
			GL.Material (MaterialFace.Front, MaterialParameter.Ambient, materialAmbient);
			GL.Material (MaterialFace.Front, MaterialParameter.Diffuse, materialDiffuse);
			
			GL.Rotate (RollAngle, 1, 0, 0);
			// Earth's axial tilt is 23.45 degrees from the plane of the ecliptic
			GL.Rotate (-23.45, 0, 0, 1);
			GL.Rotate ((animationPhase * 360), 0, 1, 0);
			GL.Rotate (-90, 1, 0, 0);
			
			sphere.Draw ();
			
			GL.PopMatrix ();
			
			GL.BindTexture (TextureTarget.Texture2D, 0);
			GL.Flush ();
		}

		static double degreesToRadians (double degrees)
		{
			return degrees * Math.PI / 180.0;
		}
	}
}

