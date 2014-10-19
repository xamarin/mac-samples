using System;
using System.Drawing;
using System.CodeDom.Compiler;

using AppKit;
using Foundation;
using CoreGraphics;

namespace SceneKitReel
{
	public partial class GameView : SceneKit.SCNView
	{
		CGPoint ClickLocation { get; set; }

		public GameView (IntPtr Handle) : base (Handle) {}

		// forward click event to the game view controller
		public override void MouseDown (AppKit.NSEvent theEvent)
		{
			ClickLocation = ConvertPointFromView (theEvent.LocationInWindow, null);

			GameViewController.GestureDidBegin ();

			if (theEvent.ClickCount == 2) {
				GameViewController.HandleDoubleTap (ClickLocation);
			} else {
				if (theEvent.ModifierFlags != NSEventModifierMask.AlternateKeyMask) {
					GameViewController.HandleTap (ClickLocation);
				}
			}

			base.MouseDown (theEvent);
		}

		// forward drag event to the view controller as "pan" events
		public override void MouseDragged (NSEvent theEvent)
		{
			if (theEvent.ModifierFlags == NSEventModifierMask.AlternateKeyMask) {
				var p = ConvertPointFromView (theEvent.LocationInWindow, null);
				GameViewController.TiltCamera (new CGPoint (p.X - ClickLocation.X, p.Y - ClickLocation.Y));
			}
			else {
				GameViewController.HandlePan (ConvertPointFromView (theEvent.LocationInWindow, null));
			}

			base.MouseDragged (theEvent);
		}

		// forward mouse up events as "end gesture"
		public override void MouseUp (NSEvent theEvent)
		{
			GameViewController.GestureDidEnd ();
			base.MouseUp (theEvent);
		}
	}
}

