using System;
using AppKit;
using Foundation;

namespace VisualEffectPlayground
{
	[Register ("WindowMovableImageView")]
	public class WindowMovableImageView : NSImageView
	{
		public override bool MouseDownCanMoveWindow {
			get {
				return true;
			}
		}

		public WindowMovableImageView (IntPtr handle) : base (handle)
		{
		}
	}
}

