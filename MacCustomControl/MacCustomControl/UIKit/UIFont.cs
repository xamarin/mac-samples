using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace UIKit
{
	[Register("UIFont")]
	public class UIFont : NSObject
	{
		#region Computed Properties
		public NSFont NSFont { get; set; }

		public static nfloat LabelFontSize {
			get { return NSFont.LabelFontSize; }
		}
		#endregion

		#region Type Conversion
		public static implicit operator NSFont(UIFont font) {
			return font.NSFont;
		}

		public static implicit operator UIFont(NSFont font) {
			return new UIFont(font);
		}
		#endregion

		#region Constructors
		public UIFont (NSFont font)
		{
			// Initialize
			this.NSFont = font;
		}
		#endregion

		#region Static Methods
		public static UIFont BoldSystemFontOfSize(nfloat size) {
			return new UIFont (NSFont.BoldSystemFontOfSize (size));
		}

		public static UIFont SystemFontOfSize(nfloat size) {
			return new UIFont (NSFont.SystemFontOfSize (size));
		}

		public static UIFont FromName(string name, nfloat size) {
			return new UIFont (NSFont.FromFontName(name,size));
		}
		#endregion
	}
}

