using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace UIKit
{
	[Register("UIImage")]
	public class UIImage : NSObject
	{
		#region Computed Properties
		public NSImage NSImage { get; set; }
		#endregion

		#region Type Conversion
		public static implicit operator NSImage(UIImage image) {
			return image.NSImage;
		}

		public static implicit operator UIImage(NSImage image) {
			return new UIImage(image);
		}
		#endregion

		#region Constructors
		public UIImage(NSImage image) : base() {
			// Initialize
			this.NSImage = image;
		}

		public UIImage (NSObjectFlag x) : base(x) {
		}

		public UIImage (IntPtr handle) : base(handle) {
		}
		#endregion
	}
}

