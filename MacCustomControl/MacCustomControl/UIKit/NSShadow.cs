using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace UIKit
{
	public class NSShadow : NSObject
	{
		#region Computed Variables
		public AppKit.NSShadow Shadow { get; set; }

		public UIColor ShadowColor { 
			get { return Shadow.ShadowColor; }
			set { Shadow.ShadowColor = value; }
		}

		public CGSize ShadowOffset {
			get { return Shadow.ShadowOffset; }
			set { Shadow.ShadowOffset = value; }
		}

		public nfloat ShadowBlurRadius { 
			get { return Shadow.ShadowBlurRadius; }
			set { Shadow.ShadowBlurRadius = value; }
		}
		#endregion

		#region Type Conversion
		public static implicit operator AppKit.NSShadow(NSShadow shadow) {
			return shadow.Shadow;
		}

		public static implicit operator NSShadow(AppKit.NSShadow shadow) {
			return new NSShadow(shadow);
		}
		#endregion

		#region Constructors
		public NSShadow() {
			// Initialize
			this.Shadow = new AppKit.NSShadow();
		}

		public NSShadow(AppKit.NSShadow shadow) : base() {
			// Initialize
			this.Shadow = shadow;
		}

		public NSShadow (NSObjectFlag x) : base(x) {
		}

		public NSShadow (IntPtr handle) : base(handle) {
		}
		#endregion
	}
}

