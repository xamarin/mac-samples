using System;
using System.Collections.Generic;
using System.Text;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using SceneKit;
using AppKit;

namespace Bananas
{
	public class SharedSceneView : SCNView
	{
		public static readonly string LeftKey = "LeftKey";
		public static readonly string RightKey = "RightKey";
		public static readonly string JumpKey = "JumpKey";
		public static readonly string RunKey = "RunKey";

		public List<string> KeysPressed { get; private set; }

		public SharedSceneView (CGRect frame) : base (frame)
		{
			Init ();
		}

		public SharedSceneView (IntPtr handle) : base (handle)
		{
			Init ();
		}

		void Init ()
		{
			KeysPressed = new List<string> ();
		}

		void UpdateKey (string key, bool pressed)
		{
			if (pressed)
				KeysPressed.Add (key);
			else
				KeysPressed.RemoveAll (k => k == key);
		}

		void HandleKeyAction (NSEvent theEvent, bool isUp)
		{
			ulong code = DecodeFromUnicode (theEvent.Characters);
			NSKey keyPressed;

			if (code == 32)
				keyPressed = NSKey.Space;
			else if (code == 114)
				keyPressed = NSKey.R;
			else
				keyPressed = (NSKey)DecodeFromUnicode (theEvent.Characters);

			switch (keyPressed) {
			case NSKey.RightArrow:
				UpdateKey (RightKey, isUp);
				break;
			case NSKey.LeftArrow:
				UpdateKey (LeftKey, isUp);
				break;
			case NSKey.R:
				UpdateKey (RunKey, isUp);
				break;
			case NSKey.Space:
				UpdateKey (JumpKey, isUp);
				break;
			default:
				break;
			}
		}

		public override void KeyDown (NSEvent theEvent)
		{
			HandleKeyAction (theEvent, true);
		}

		public override void KeyUp (NSEvent theEvent)
		{
			HandleKeyAction (theEvent, false);
		}

		public override void MouseUp (NSEvent theEvent)
		{
			var skScene = (InGameScene)OverlayScene;
			CGPoint p = skScene.ConvertPointFromView (theEvent.LocationInWindow);
			skScene.TouchUpAtPoint (p);

			base.MouseUp (theEvent);
		}

		ulong DecodeFromUnicode (string utf8String)
		{
			byte[] bytes = System.Text.Encoding.Unicode.GetBytes (utf8String);
			ulong code = 0;
			var length = bytes.Length;
			for (int i = 0; i < length; ++i) {
				code = code << 8;
				code = code | bytes [length - i - 1];
			}

			return code;
		}
	}
}

