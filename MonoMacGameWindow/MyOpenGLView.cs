//
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
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreVideo;
using MonoMac.CoreGraphics;
using MonoMac.OpenGL;

namespace MonoMacGameView
{
	public partial class MyOpenGLView : MonoMac.OpenGL.MonoMacGameView
	{
		Scene scene;
		
		[Export("initWithFrame:")]
		public MyOpenGLView (RectangleF frame) : this(frame, null)
		{
		}

		public MyOpenGLView (RectangleF frame, NSOpenGLContext context) : base(frame, context)
		{
			scene = new Scene();
			
			Resize += delegate {
				scene.ResizeGLScene(Bounds);	
			};
			
			Load += loader;
			
			UpdateFrame += delegate(object src, FrameEventArgs fea) {
				//Console.WriteLine("update " + fea.Time);	
			};
			
			RenderFrame += delegate(object src, FrameEventArgs fea) {
				scene.DrawGLScene();
			};
			
		}
		
		public void loader (object src, EventArgs fea)
		{
			//Console.WriteLine("load ");	
			InitGL();
			UpdateView();
			
		}
		
		public override bool AcceptsFirstResponder ()
		{
			// We want this view to be able to receive key events
			return true;
		}
		
				// All Setup For OpenGL Goes Here
		public bool InitGL ()
		{
			// Enables Smooth Shading  
			GL.ShadeModel (ShadingModel.Smooth);
			// Set background color to black     
			GL.ClearColor (Color.Black);

			// Setup Depth Testing

			// Depth Buffer setup
			GL.ClearDepth (1.0);
			// Enables Depth testing
			GL.Enable (EnableCap.DepthTest);
			// The type of depth testing to do
			GL.DepthFunc (DepthFunction.Lequal);

			// Really Nice Perspective Calculations
			GL.Hint (HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
			
			return true;
		}
		
		[Export("toggleFullScreen:")]
		public void toggleFullScreen (NSObject sender)
		{
			if (WindowState == WindowState.Fullscreen)
				WindowState = WindowState.Normal;
			else
				WindowState = WindowState.Fullscreen;
		}

//		public override void KeyDown (NSEvent theEvent)
//		{
//			controller.KeyDown (theEvent);
//		}
//
//		public override void MouseDown (NSEvent theEvent)
//		{
//			controller.MouseDown (theEvent);
//		}

	}
}

