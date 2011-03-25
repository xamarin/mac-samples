//
// This code was created by Jeff Molofee '99
//
// If you've found this code useful, please let me know.
//
// Visit me at www.demonews.com/hosted/nehe
//
//=====================================================================
// Converted to C# and MonoMac by Kenneth J. Pouncey
// http://www.cocoa-mono.org
//
// Copyright (c) 2011 Kenneth J. Pouncey
//
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
using System;
using System.Drawing;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreVideo;
using MonoMac.CoreGraphics;
using MonoMac.OpenGL;

namespace NeHeLesson9
{
	public partial class MyOpenGLView : MonoMac.AppKit.NSView
	{

		NSOpenGLContext openGLContext;
		NSOpenGLPixelFormat pixelFormat;
		MainWindowController controller;
		CVDisplayLink displayLink;
		NSObject notificationProxy;

		[Export("initWithFrame:")]
		public MyOpenGLView (RectangleF frame) : this(frame, null)
		{
		}

		public MyOpenGLView (RectangleF frame,NSOpenGLContext context) : base(frame)
		{
			var attribs = new object [] {
				NSOpenGLPixelFormatAttribute.Accelerated,
				NSOpenGLPixelFormatAttribute.NoRecovery,
				NSOpenGLPixelFormatAttribute.DoubleBuffer,
				NSOpenGLPixelFormatAttribute.ColorSize, 24,
				NSOpenGLPixelFormatAttribute.DepthSize, 16 };

			pixelFormat = new NSOpenGLPixelFormat (attribs);

			if (pixelFormat == null)
				Console.WriteLine ("No OpenGL pixel format");

			// NSOpenGLView does not handle context sharing, so we draw to a custom NSView instead
			openGLContext = new NSOpenGLContext (pixelFormat, context);

			openGLContext.MakeCurrentContext ();

			// Synchronize buffer swaps with vertical refresh rate
			openGLContext.SwapInterval = true;

			// Initialize our newly created view.
			InitGL ();

			SetupDisplayLink ();

			// Look for changes in view size
			// Note, -reshape will not be called automatically on size changes because NSView does not export it to override 
			notificationProxy = NSNotificationCenter.DefaultCenter.AddObserver (NSView.NSViewGlobalFrameDidChangeNotification, HandleReshape);
		}

		public override void DrawRect (RectangleF dirtyRect)
		{
			// Ignore if the display link is still running
			if (!displayLink.IsRunning && controller != null)
				DrawView ();
		}

		public override bool AcceptsFirstResponder ()
		{
			// We want this view to be able to receive key events
			return true;
		}

		public override void LockFocus ()
		{
			base.LockFocus ();
			if (openGLContext.View != this)
				openGLContext.View = this;
		}

		public override void KeyDown (NSEvent theEvent)
		{
			controller.KeyDown (theEvent);
		}

		public override void MouseDown (NSEvent theEvent)
		{
			controller.MouseDown (theEvent);
		}

		// All Setup For OpenGL Goes Here
		public bool InitGL ()
		{

			// Enable Texture Mapping
			GL.Enable (EnableCap.Texture2D);
			// Enables Smooth Shading  
			GL.ShadeModel (ShadingModel.Smooth);
			// Set background color to black     
			GL.ClearColor (0, 0, 0, 0.5f);

			// Setup Depth Testing

			// Depth Buffer setup
			GL.ClearDepth (1.0);

			// Really Nice Perspective Calculations
			GL.Hint (HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

			GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
			GL.Enable (EnableCap.Blend);
			return true;
		}

		private void DrawView ()
		{
			// This method will be called on both the main thread (through -drawRect:) and a secondary thread (through the display link rendering loop)
			// Also, when resizing the view, -reshape is called on the main thread, but we may be drawing on a secondary thread
			// Add a mutex around to avoid the threads accessing the context simultaneously 
			openGLContext.CGLContext.Lock ();

			// Make sure we draw to the right context
			openGLContext.MakeCurrentContext ();

			// Delegate to the scene object for rendering
			controller.Scene.DrawGLScene ();

			openGLContext.FlushBuffer ();

			openGLContext.CGLContext.Unlock ();
		}

		private void SetupDisplayLink ()
		{
			// Create a display link capable of being used with all active displays
			displayLink = new CVDisplayLink ();

			// Set the renderer output callback function
			displayLink.SetOutputCallback (MyDisplayLinkOutputCallback);

			// Set the display link for the current renderer
			CGLContext cglContext = openGLContext.CGLContext;
			CGLPixelFormat cglPixelFormat = PixelFormat.CGLPixelFormat;
			displayLink.SetCurrentDisplay (cglContext, cglPixelFormat);

		}

		public CVReturn MyDisplayLinkOutputCallback (CVDisplayLink displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, ref CVOptionFlags flagsOut)
		{
			CVReturn result = GetFrameForTime (inOutputTime);

			return result;
		}

		private CVReturn GetFrameForTime (CVTimeStamp outputTime)
		{
			// There is no autorelease pool when this method is called because it will be called from a background thread
			// It's important to create one or you will leak objects
			using (NSAutoreleasePool pool = new NSAutoreleasePool ()) {

				// Update the animation
				DrawView ();
			}

			return CVReturn.Success;

		}

		public NSOpenGLContext OpenGLContext {
			get { return openGLContext; }
		}

		public NSOpenGLPixelFormat PixelFormat {
			get { return pixelFormat; }
		}

		public MainWindowController MainController {
			set { controller = value; }
		}

		public void UpdateView ()
		{
			// This method will be called on the main thread when resizing, but we may be drawing on a secondary thread through the display link
			// Add a mutex around to avoid the threads accessing the context simultaneously
			openGLContext.CGLContext.Lock ();

			// Delegate to the scene object to update for a change in the view size
			controller.Scene.ResizeGLScene (Bounds);
			openGLContext.Update ();

			openGLContext.CGLContext.Unlock ();
		}

		private void HandleReshape (NSNotification note)
		{
			UpdateView ();
		}

		public void StartAnimation ()
		{
			if (displayLink != null && !displayLink.IsRunning)
				displayLink.Start ();
		}

		public void StopAnimation ()
		{
			if (displayLink != null && displayLink.IsRunning)
				displayLink.Stop ();
		}

		// Clean up the notifications
		public void DeAllocate ()
		{
			displayLink.Stop ();
			displayLink.SetOutputCallback (null);

			NSNotificationCenter.DefaultCenter.RemoveObserver (notificationProxy); 
		}

		[Export("toggleFullScreen:")]
		public void toggleFullScreen (NSObject sender)
		{
			controller.toggleFullScreen (sender);
		}		
	}
}

