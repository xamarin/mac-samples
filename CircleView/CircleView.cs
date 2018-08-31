using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;
using System.Runtime.InteropServices;

namespace CircleView
{
	public partial class CircleView : AppKit.NSView
	{
		NSTimer Timer;
		double LastAnimationTime;
		NSTextStorage TextStorage = new NSTextStorage ("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis aliquet ipsum non sem volutpat, ut posuere nunc pharetra.");
		NSLayoutManager LayoutManager = new NSLayoutManager ();
		NSTextContainer TextContainer = new NSTextContainer ();

		CGPoint center;
		public CGPoint Center
		{
			get => center;
			set { center = value; NeedsDisplay = true; }
		}

		double radius = 115;
		public double Radius
		{
			get => radius;
			set { radius = value; NeedsDisplay = true; }
		}

		double startingAngle = Math.PI / 2;
		public double StartingAngle
		{
			get => startingAngle;
			set { startingAngle = value; NeedsDisplay = true; }
		}

		double angularVelocity = Math.PI / 2;
		public double AngularVelocity
		{
			get => angularVelocity;
			set { angularVelocity = value; NeedsDisplay = true; }
		}

		public string Text
		{
			get => TextStorage.Value;
			set {
				TextStorage.Replace (new NSRange (0, TextStorage.Length), value);
				NeedsDisplay = true;
			}
		}

		NSColor textColor;
		public NSColor TextColor
		{
			get => textColor;
			set {
				textColor = value;
				TextStorage.AddAttribute (NSStringAttributeKey.ForegroundColor, value, new NSRange (0, TextStorage.Length));
				NeedsDisplay = true;
			}
		}

		public CircleView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		[Export ("initWithCoder:")]
		public CircleView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		void Initialize ()
		{
			Center = new CGPoint (180, 135);
			LayoutManager.AddTextContainer (TextContainer);
			TextStorage.AddLayoutManager (LayoutManager);

			TextColor = NSColor.Black;
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			base.DrawRect (dirtyRect);
			NSColor.White.Set ();
			NSGraphics.RectFill (Bounds);

			var glyphRange = LayoutManager.GetGlyphRange (TextContainer);
			var usedRect = LayoutManager.GetUsedRectForTextContainer (TextContainer);

			for (int glyphIndex = (int)glyphRange.Location; glyphIndex < glyphRange.Location + glyphRange.Length; glyphIndex++) {
				var context = NSGraphicsContext.CurrentContext;
				var lineFramgmentRect = LayoutManager.LineFragmentRectForGlyphAtIndex ((nuint)glyphIndex, IntPtr.Zero);
				var layoutLocation = LayoutManager.LocationForGlyphAtIndex (glyphIndex);
				NSAffineTransform transform = new NSAffineTransform ();

				layoutLocation.X += lineFramgmentRect.X;
				layoutLocation.Y += lineFramgmentRect.Y;

				var distance = Radius + usedRect.Height - layoutLocation.Y;
				nfloat angle = (nfloat)(StartingAngle + layoutLocation.X / distance);

				nfloat viewLocationX = (nfloat)(Center.X + distance * Math.Sin (angle));
				nfloat viewLocationY = (nfloat)(Center.Y + distance * Math.Cos (angle));

				transform.Translate (viewLocationX, viewLocationY);
				transform.RotateByRadians (-angle);

				context?.SaveGraphicsState ();
				transform.Concat ();

				LayoutManager.DrawGlyphsForGlyphRange (new NSRange (glyphIndex, 1), new CGPoint (-layoutLocation.X, -layoutLocation.Y));
				context?.RestoreGraphicsState ();
			}
		}

		void StartAnimation ()
		{
			Timer = NSTimer.CreateScheduledTimer (1.0 / 30, true, t => Animate (t));

			NSRunLoop.Current.AddTimer (Timer, NSRunLoopMode.ModalPanel);
			NSRunLoop.Current.AddTimer (Timer, NSRunLoopMode.EventTracking);

			LastAnimationTime = NSDate.Now.SecondsSinceReferenceDate;
		}

		void StopAnimation ()
		{
			Timer?.Invalidate ();
			Timer = null;
		}

		void Animate (NSTimer timer)
		{
			var now = NSDate.Now.SecondsSinceReferenceDate;
			StartingAngle = StartingAngle + AngularVelocity * (now - LastAnimationTime);
			LastAnimationTime = now;
		}

		public void ToggleAnimation ()
		{
			if (Timer == null)
				StartAnimation ();
			else
				StopAnimation ();
		}
	}

	// "lineFragmentRectForGlyphAtIndex:effectiveRange:" binding is missing so work around with manual bindings
	// https://github.com/xamarin/xamarin-macios/issues/4740
	public static class LayoutManagerExtensions
	{
		static readonly IntPtr selLineFragmentRectForGlyphAtIndex_EffectiveRange_Handle = ObjCRuntime.Selector.GetHandle ("lineFragmentRectForGlyphAtIndex:effectiveRange:");

		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend_stret")]
		public extern static void CGRect_objc_msgSend_stret_nuint_IntPtr (out CGRect retval, IntPtr receiver, IntPtr selector, nuint arg1, IntPtr arg2);

		public static CGRect LineFragmentRectForGlyphAtIndex (this NSLayoutManager manager, nuint glyphIndex, IntPtr effectiveGlyphRange)
		{
			NSApplication.EnsureUIThread ();
			CGRect_objc_msgSend_stret_nuint_IntPtr (out CGRect ret, manager.Handle, selLineFragmentRectForGlyphAtIndex_EffectiveRange_Handle, glyphIndex, effectiveGlyphRange);
			return ret;
		}
	}
}
