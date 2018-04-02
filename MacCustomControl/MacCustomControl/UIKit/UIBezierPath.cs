using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace UIKit
{
	public class UIBezierPath : NSObject
	{
		#region Private Variables
		private NSBezierPath path;
		#endregion

		#region Computed Properties
		public bool UsesEvenOddFillRule {
			get {
				return (path.WindingRule == NSWindingRule.EvenOdd);
			}
			set {
				if (value) {
					path.WindingRule = NSWindingRule.EvenOdd;
				} else {
					path.WindingRule = NSWindingRule.NonZero;
				}
			}
		}

		public CGPath CGPath {
			get {
				var cgpath = new CGPath ();
				cgpath.AddRect (path.Bounds);
				return cgpath;
			}
		}

		public CGRect Bounds {
			get { return path.Bounds; }
		}

		public nfloat LineWidth {
			get { return path.LineWidth; }
			set { path.LineWidth = value; }
		}

		public nfloat MiterLimit {
			get { return path.MiterLimit; }
			set { path.MiterLimit = value; }
		}
		#endregion

		#region Constructors
		public UIBezierPath () : base()
		{
			// Initialize
			this.path = new NSBezierPath();
		}

		public UIBezierPath (NSBezierPath path) : base()
		{
			// Initialize
			this.path = path;
		}

		public UIBezierPath (NSObjectFlag x) : base(x) {
		}

		public UIBezierPath (IntPtr handle) : base(handle) {
		}
		#endregion

		#region Static Methods
		public static UIBezierPath FromOval(CGRect rect) {
			return new UIBezierPath(NSBezierPath.FromOvalInRect (rect));
		}

		public static UIBezierPath FromRect(CGRect rect) {
			return new UIBezierPath(NSBezierPath.FromRect (rect));
		}

		public static UIBezierPath FromRoundedRect(CGRect rect, nfloat radius) {
			return new UIBezierPath (NSBezierPath.FromRoundedRect (rect, radius, radius));
		}
		#endregion

		#region Public Methods
		public virtual void AddLineTo (CGPoint point) {
			path.LineTo (point);
		}

		public virtual void AddCurveToPoint (CGPoint endPoint, CGPoint controlPoint1, CGPoint controlPoint2){
			path.CurveTo (endPoint, controlPoint1, controlPoint2);
		}

		public virtual void MoveTo (CGPoint point) {
			path.MoveTo (point);
		}

		public virtual void AddClip() {
			path.AddClip ();
		}

		public virtual void Stroke() {
			path.Stroke ();
		}

		public virtual void ClosePath() {
			path.ClosePath ();
		}

		public virtual void Fill() {
			path.Fill ();
		}
		#endregion
	}
}

