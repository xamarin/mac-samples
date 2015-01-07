using System;
using AppKit;
using CoreGraphics;

namespace VisualEffectPlayground
{
	public class MyKeyWindow : NSWindow
	{
		public MyKeyWindow (CGRect rect, NSWindowStyle style, NSBackingStore store, bool deferCreation)
			: base (rect, style, store, deferCreation)
		{
		}

		public override bool CanBecomeKeyWindow {
			get {
				return true; // Borderless windows normally can't become key
			}
		}
	}
}

