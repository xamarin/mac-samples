using System;
using System.Drawing;
using Foundation;
using AppKit;
using CoreGraphics;

namespace UIKit
{
	public class UIGraphicsContext : NSGraphicsContext
	{
		#region Private Variables
		private NSGraphicsContext context;
		private UIColor shadowColor = NSColor.Black;
		#endregion

		#region Constructors
		public UIGraphicsContext (NSGraphicsContext context) : base () {
			// Initialize
			this.context = context;
		}

		public UIGraphicsContext (NSObjectFlag x) : base(x) {
		}

		public UIGraphicsContext (IntPtr handle) : base(handle) {
		}
		#endregion

		#region Public Methods
		public void SaveState() {
			context.SaveGraphicsState ();
		}

		public void RestoreState() {
			context.RestoreGraphicsState ();
		}

		public void DrawLinearGradient (CGGradient gradient, CGPoint startPoint, CGPoint endPoint, CGGradientDrawingOptions options){

			context.CGContext.DrawLinearGradient (gradient, startPoint, endPoint, options);

		}

		public void DrawRadialGradient (CGGradient gradient, CGPoint startCenter, nfloat startRadius, CGPoint endCenter, nfloat endRadius, CGGradientDrawingOptions options){

			context.CGContext.DrawRadialGradient (gradient, startCenter, startRadius, endCenter, endRadius, options);

		}

		public void BeginTransparencyLayer() {
			context.CGContext.BeginTransparencyLayer ();
		}

		public void EndTransparencyLayer() {
			context.CGContext.EndTransparencyLayer ();
		}

		public void SetShadow(CGSize ShadowOffset, nfloat ShadowBlurRadius, CGColor ShadowColor) {
			context.CGContext.SetShadow (ShadowOffset, ShadowBlurRadius, ShadowColor);
		}

		public void SetShadow(CGSize ShadowOffset, nfloat ShadowBlurRadius) {
			context.CGContext.SetShadow (ShadowOffset, ShadowBlurRadius, shadowColor.CGColor);
		}

		public void TranslateCTM(nfloat tx, nfloat ty) {
			context.CGContext.TranslateCTM (tx, ty);
		}

		public void RotateCTM(nfloat angle) {
			context.CGContext.RotateCTM (angle);
		}

		public void SetAlpha(nfloat alpha) {
			context.CGContext.SetAlpha (alpha);
		}

		public void SetBlendMode (CGBlendMode mode) {
			context.CGContext.SetBlendMode (mode);
		}

		public void SetFillColor(CGColor color) {
			context.CGContext.SetFillColor (color);
		}

		public void ClipToRect(CGRect rect) {
			context.CGContext.ClipToRect (rect);
		}
		#endregion
	}
}

