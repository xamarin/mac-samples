
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace NeHeLesson5
{
        public partial class MainWindowController : MonoMac.AppKit.NSWindowController
        {

                bool isInFullScreenMode;

                // full-screen mode
                NSWindow fullScreenWindow;
                MyOpenGLView fullScreenView;

                Scene scene;
		bool isAnimating;

                #region Constructors

                // Call to load from the XIB/NIB file
                public MainWindowController () : base("MainWindow")
                {
 
                }

                #endregion

                //strongly typed window accessor
                public new MainWindow Window {
                        get { return (MainWindow)base.Window; }
                }

		public override void AwakeFromNib ()
		{
			// Allocate the scene object
			scene = new Scene ();
			
			// Assign the view's MainController to us
			openGLView.MainController = this;
			
			// reset the viewport and update OpenGL Context
			openGLView.UpdateView ();
			
			// Activate the display link now
			openGLView.StartAnimation ();
			
			isAnimating = true;
		}

		partial void goFullScreen (NSObject sender)
		{
			isInFullScreenMode = true;
			
			// Pause the non-fullscreen view
			openGLView.StopAnimation ();
			
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
			
			// Set the scene with the full-screen viewport and viewing transformation
			Scene.ResizeGLScene (viewRect);
			
			// Assign the view's MainController to self
			fullScreenView.MainController = this;
			
			if (!isAnimating) {
				// Mark the view as needing drawing to initalize its contents
				fullScreenView.NeedsDisplay = true;
			} else {
				// Start playing the animation
				fullScreenView.StartAnimation ();
				
			}
		}

		public void goWindow ()
		{
			isInFullScreenMode = false;
			
			// use OrderOut here instead of Close or nasty things will happen with Garbage Collection and a double free
			fullScreenWindow.OrderOut (this);
			fullScreenView.DeAllocate ();
			fullScreenWindow.Dispose ();
			fullScreenWindow = null;
			
			// Switch to the non-fullscreen context
			openGLView.OpenGLContext.MakeCurrentContext ();
			
			if (!isAnimating) {
				// Mark the view as needing drawing
				// The animation has advanced while we were in full-screen mode, so its current contents are stale
				openGLView.NeedsDisplay = true;
				
			} else {
				// continue playing the animation
				openGLView.StartAnimation ();
			}
		}

		public void startAnimation ()
		{
			if (isAnimating)
				return;
			
			if (!isInFullScreenMode)
				openGLView.StartAnimation ();
			else
				fullScreenView.StartAnimation ();
			
			isAnimating = true;
		}

		public void stopAnimation ()
		{
			if (!isAnimating)
				return;
			
			if (!isInFullScreenMode)
				openGLView.StopAnimation ();
			else
				fullScreenView.StopAnimation ();
			
			isAnimating = false;
		}

                public override void KeyDown (NSEvent theEvent)
                {
                        var c = theEvent.CharactersIgnoringModifiers[0];
                        
                        switch (c) {
                        
                        // [Esc] exits full-screen mode
                        case (char)27:
                                if (isInFullScreenMode)
                                        goWindow ();
                                break;
                        default:
                                break;
                                
                        }
                }
  
                // Accessor property for our scene object
                public Scene Scene {
                        get { return scene; }
                }
                
                public void toggleFullScreen ( NSObject sender )
                {
                        if (!isInFullScreenMode)
                                goFullScreen(sender);
                        else
                                goWindow();
                }

        }
}

