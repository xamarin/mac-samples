using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;
using OpenGL;
using OpenTK.Graphics.OpenGL;

namespace OpenGLViewSample
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;
		GLView view;
		
		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindowController = new MainWindowController ();
			view = new GLView (mainWindowController.Window.Frame, new NSOpenGLPixelFormat (new object [] {
										NSOpenGLPixelFormatAttribute.Accelerated,
										NSOpenGLPixelFormatAttribute.MinimumPolicy				
			}));
			
			mainWindowController.Window.ContentView = view;
 			mainWindowController.Window.MakeKeyAndOrderFront (this);
		}
	}
	
	public class GLView : NSOpenGLView
	{
		public GLView (CGRect rect, NSOpenGLPixelFormat format) : base (rect, format) {
		}
		
		static void DrawTriangle ()
		{
			GL.Color3 (1.0f, 0.85f, 0.35f);
			GL.Begin (BeginMode.Triangles);
			
			GL.Vertex3 (0.0, 0.6, 0.0);
			GL.Vertex3 (-0.2, -0.3, 0.0);
			GL.Vertex3 (0.2, -0.3 ,0.0);
			
			GL.End ();
		}
		
		public override void DrawRect (CGRect dirtyRect)
		{
			OpenGLContext.MakeCurrentContext ();
			
			GL.ClearColor (Color.Brown);
	
			GL.ClearColor (0, 0, 0, 0);
			GL.Clear(ClearBufferMask.ColorBufferBit);
 
			DrawTriangle ();
			
			GL.Flush ();
		}
	}
}

