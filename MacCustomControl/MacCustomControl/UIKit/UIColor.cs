using System;
using System.Drawing;
using Foundation;
using AppKit;
using CoreGraphics;

namespace UIKit
{
	[Register("UIColor")]
	public class UIColor : NSObject
	{
		#region Computed Properties
		public NSColor NSColor { get; set; }

		public CGColor CGColor {
			get { return this.NSColor.CGColor; }
		}
		#endregion

		#region Type Conversion
		public static implicit operator NSColor(UIColor color) {
			return color.NSColor;
		}

		public static implicit operator UIColor(NSColor color) {
			return new UIColor(color);
		}
		#endregion

		#region Constructors
		public UIColor(NSColor color) : base() {
			// Initialize
			this.NSColor = color;
		}

		public UIColor(nfloat red, nfloat green, nfloat blue, nfloat alpha) : base() {
			// Initialize
			this.NSColor = NSColor.FromRgba (red, green, blue, alpha);
		}

		public UIColor (NSObjectFlag x) : base(x) {
		}

		public UIColor (IntPtr handle) : base(handle) {
		}
		#endregion

		#region Static Methods
		public static UIColor FromRGBA(nfloat red, nfloat green, nfloat blue, nfloat alpha) {

			return new UIColor (NSColor.FromRgba (red, green, blue, alpha));
		}
		#endregion

		#region Public Methods
		public void SetStroke() {
			this.NSColor.SetStroke ();
		}

		public void SetFill() {
			// Send color change to the String drawing routines
			UIStringDrawing.FillColor = this.NSColor;

			this.NSColor.SetFill ();
		}

		public void GetRGBA (out nfloat red, out nfloat green, out nfloat blue, out nfloat alpha){
			this.NSColor.GetRgba (out red, out green, out blue, out alpha);
		}

		public UIColor ColorWithAlpha(nfloat alpha) {
			return new UIColor (this.NSColor.ColorWithAlphaComponent (alpha));
		}
		#endregion
	}
}

