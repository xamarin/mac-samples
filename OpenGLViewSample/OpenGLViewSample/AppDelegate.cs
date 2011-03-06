using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.OpenGL;

namespace OpenGLViewSample
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;
		GLView view;
		
		public AppDelegate ()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			mainWindowController = new MainWindowController ();
			view = new GLView (mainWindowController.Window.Frame, new NSOpenGLPixelFormat (new object [] {
										NSOpenGLPixelFormatAttribute.FullScreen,
										NSOpenGLPixelFormatAttribute.MinimumPolicy				
			}));
			
			mainWindowController.Window.ContentView = view;
 			mainWindowController.Window.MakeKeyAndOrderFront (this);
		}
	}
	
	public class GLView : NSOpenGLView
	{
		public GLView (RectangleF rect, NSOpenGLPixelFormat format) : base (rect, format) {
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
		
		public override void DrawRect (RectangleF dirtyRect)
		{
			OpenGLContext.MakeCurrentContext ();
			
			GL.ClearColor(Color.Brown);
	
			GL.ClearColor (0, 0, 0, 0);
			GL.Clear(ClearBufferMask.ColorBufferBit);
 
			DrawTriangle ();
			
			GL.Flush ();
		}
	}
}

