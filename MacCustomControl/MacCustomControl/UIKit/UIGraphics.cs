using System;
using System.Drawing;
using Foundation;
using AppKit;
using CoreGraphics;

namespace UIKit
{
	public class UIGraphics : NSObject
	{
		#region Private Variables
		private static CGBitmapContext Context = null;
		private static NSGraphicsContext PreviousContext = null;
		private static CGColorSpace ColorSpace = null;
		private static CGSize ImageSize = CGSize.Empty;
		#endregion

		#region Static Methods
		public static UIGraphicsContext GetCurrentContext() {
			return new UIGraphicsContext (NSGraphicsContext.CurrentContext);
		}

		public static void BeginImageContextWithOptions (CGSize size, bool opaque, nfloat scale) {

			// Create new image context
			ColorSpace = CGColorSpace.CreateDeviceRGB ();
			Context = new CGBitmapContext (null, (int)size.Width, (int)size.Height, 8, 0, ColorSpace, CGImageAlphaInfo.PremultipliedLast);

			// Flip context vertically
			var flipVertical = new  CGAffineTransform(1,0,0,-1,0,size.Height);
			Context.ConcatCTM (flipVertical);

			// Save previous context
			ImageSize = size;
			PreviousContext = NSGraphicsContext.CurrentContext;
			NSGraphicsContext.CurrentContext = NSGraphicsContext.FromCGContext (Context, true);
		}

		public static UIImage GetImageFromCurrentImageContext() {
			return new UIImage (new NSImage(Context.ToImage(), ImageSize));
		}

		public static void EndImageContext() {

			// Return to previous context
			if (PreviousContext != null) {
				NSGraphicsContext.CurrentContext = PreviousContext;
			}

			// Release memory
			Context = null;
			PreviousContext = null;
			ColorSpace = null;
			ImageSize = CGSize.Empty;

		}
		#endregion 
	}
}

