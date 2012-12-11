using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.OpenGL;

namespace MonoMacGameView
{
	public partial class MonoMacGameWindowController : MonoMac.AppKit.NSWindowController
	{

		bool isInFullScreenMode;

		// full-screen mode
		NSWindow fullScreenWindow;
		MyOpenGLView fullScreenView;
		bool isAnimating;
		
		double updatesPerSecond = 0;
		
		// Call to load from the XIB/NIB file
		public MonoMacGameWindowController () : base ("MonoMacGameWindow")
		{
		}

		//strongly typed window accessor
		public new MonoMacGameWindow Window {
			get {
				return (MonoMacGameWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{

			openGLView.WindowStateChanged += delegate {

				if (openGLView.WindowState == WindowState.Fullscreen)
					FullScreen = true;
				else
					FullScreen = false;
			};

			openGLView.Run (updatesPerSecond);
			
			isAnimating = true;

		}

		public bool FullScreen {

			get { return isInFullScreenMode; }
			set {
				if (isInFullScreenMode != value) {
					isInFullScreenMode = value;
					if (isInFullScreenMode)
						GoFullScreenMode ();
					else
						GoWindowMode ();		
				}
			}
		}

		private void StartAnimation ()
		{
			if (isAnimating)
				return;

			if (!isInFullScreenMode)
				openGLView.Run (updatesPerSecond);
			else
				fullScreenView.Run (updatesPerSecond);

			isAnimating = true;
		}

		private void StopAnimation ()
		{
			if (!isAnimating)
				return;

			if (!isInFullScreenMode)
				openGLView.Stop ();
			else
				fullScreenView.Stop ();

			isAnimating = false;
		}

		private void GoFullScreenMode ()
		{
			isInFullScreenMode = true;

			// Pause the non-fullscreen view
			openGLView.Stop ();

			RectangleF mainDisplayRect;
			RectangleF viewRect;

			// Create a screen-sized window on the display you want to take over
			// Note, mainDisplayRect has a non-zero origin if the key window is on a secondary display
			mainDisplayRect = NSScreen.MainScreen.Frame;

			fullScreenWindow = new NSWindow (mainDisplayRect, NSWindowStyle.Borderless, NSBackingStore.Buffered, true);

			// Set the window level to be above the menu bar
			fullScreenWindow.Level = NSWindowLevel.MainMenu + 1;

			// Perform any other window configuration you desire
			fullScreenWindow.IsOpaque = true;
			fullScreenWindow.HidesOnDeactivate = true;

			// Create a view with a double-buffered OpenGL context and attach it to the window
			// By specifying the non-fullscreen context as the shareContext, we automatically inherit the 
			// OpenGL objects (textures, etc) it has defined
			viewRect = new RectangleF (0, 0, mainDisplayRect.Size.Width, mainDisplayRect.Size.Height);

			fullScreenView = new MyOpenGLView (viewRect, openGLView.OpenGLContext);
			fullScreenWindow.ContentView = fullScreenView;

			// Show the window
			fullScreenWindow.MakeKeyAndOrderFront (this);

			if (!isAnimating) {
				// Mark the view as needing drawing to initalize its contents
				fullScreenView.NeedsDisplay = true;
			} else {
				// Start playing the animation
				fullScreenView.Run (updatesPerSecond);

			}
		}

		private void GoWindowMode ()
		{
			isInFullScreenMode = false;

			// use OrderOut here instead of Close or nasty things will happen with Garbage Collection and a double free
			fullScreenWindow.OrderOut (this);
			fullScreenWindow.Dispose ();
			fullScreenWindow = null;

			// Switch to the non-fullscreen context
			openGLView.MakeCurrent ();

			if (!isAnimating) {
				// Mark the view as needing drawing
				// The animation has advanced while we were in full-screen mode, so its current contents are stale
				openGLView.NeedsDisplay = true;

			} else {
				// continue playing the animation
				openGLView.Run (updatesPerSecond);
			}
		}		

	}
}

