//
// Copyright 2010, Novell, Inc.
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
using MonoMac.ObjCRuntime;

[Register]
public class MyView : NSView {
	public MyView (RectangleF frame) : base (frame) {
	}

	public override void DrawRect (RectangleF rect) {
		Graphics g = Graphics.FromHwnd (Handle);
		g.FillRectangle (Brushes.Red, 0, 0, 200, 100);
		g.Dispose ();
	}
}

[Register]
public class HelloAppDelegate : NSApplicationDelegate {
	NSWindow window;
	NSTextField text;
	MyView view;
	
	public HelloAppDelegate ()
	{
	}

	public override void FinishedLaunching (NSObject notification)
	{
		view = new MyView (new RectangleF (10, 10, 200, 200));

		text = new NSTextField (new RectangleF (44, 32, 232, 31)) {
			StringValue = "Hello Mono Mac!"
		};
			
		window = new NSWindow (new RectangleF (50, 50, 400, 400), (NSWindowStyle) (1 | (1 << 1) | (1 << 2) | (1 << 3)), 0, false);
		window.ContentView.AddSubview (text);
		window.ContentView.AddSubview (view);



		window.MakeKeyAndOrderFront (this);
	}
}

class Demo {
	static void Main (string [] args)
	{
		NSApplication.Init ();
		NSApplication.InitDrawingBridge ();
		NSApplication.Main (args);
	}		
}
