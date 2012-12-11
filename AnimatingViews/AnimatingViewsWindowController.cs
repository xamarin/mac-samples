
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;

namespace AnimatingViews
{
	public partial class AnimatingViewsWindowController : MonoMac.AppKit.NSWindowController
	{
		private enum Layout {
			ColumnLayout,
			RowLayout,
			GridLayout
		}
		
		private Layout layoutStyle = Layout.ColumnLayout;
		
		/* Default separation between items, and default size of added subviews. */
		private const float SEPARATION = 10.0f;
		private const float BOX_WIDTH = 80.0f;
		private const float BOX_HEIGHT = 80.0f;
		
		// Called when created from unmanaged code
		public AnimatingViewsWindowController (IntPtr handle) : base(handle)
		{
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public AnimatingViewsWindowController (NSCoder coder) : base(coder)
		{
		}

		// Call to load from the XIB/NIB file
		public AnimatingViewsWindowController () : base("AnimatingViewsWindow")
		{
		}
		
		private Layout LayoutStyle {
			get {
				return layoutStyle;
			}
			
			set {
				if (layoutStyle != value) {
					layoutStyle = value;
					layout ();
				}
			}
		}
		
		//strongly typed window accessor
		public new AnimatingViewsWindow Window {
			get { return (AnimatingViewsWindow)base.Window; }
		}
		
		// Action for change layout Matrix
		partial void changeLayout (NSMatrix sender)
		{
			LayoutStyle = (Layout)sender.SelectedTag;
			layout ();
			
		} 
		
		// Action for Add pushbutton
		partial void addABox (NSButton sender)
		{
			simpleView.AddSubview (viewToBeAdded ());
			layout ();
		}
		
		// Action for Remove pushbutton
		partial void removeLastBox (NSButton sender)
		{
			if (simpleView.Subviews.Length == 0)
				return;
			
			simpleView.Subviews.Last ().RemoveFromSuperview ();
			layout ();
		}
		
		private NSView viewToBeAdded() 
		{
			return new NSBox (new RectangleF (0.0f, 0.0f, BOX_WIDTH, BOX_HEIGHT)){
				BoxType = NSBoxType.NSBoxCustom,
				BorderType = NSBorderType.LineBorder,
				TitlePosition = NSTitlePosition.NoTitle,
				FillColor = colorWell.Color
			};
		}
		
		/* This method returns a rect that is integral in base coordinates. */
		private RectangleF integralRect (RectangleF rect) 
		{
			// missing NSIntegralRect
			//return simpleView.ConvertRectFromBase(NSIntegralRect(simpleView.ConvertRectToBase(rect)));
			return simpleView.ConvertRectFromBase(simpleView.ConvertRectToBase(rect));	
		}
		
		// Layout the sub views
		private void layout ()
		{
			NSView[] subviews = simpleView.Subviews;
			PointF curPoint;
				
			switch (LayoutStyle){
			case Layout.ColumnLayout:
				curPoint = new PointF(simpleView.Bounds.Size.Width / 2.0f, 0.0f);
				foreach (NSView subview in subviews) {
					RectangleF frame = new RectangleF(curPoint.X - BOX_WIDTH /2.0f, curPoint.Y, BOX_WIDTH, BOX_HEIGHT);
					animateView(subview, frame);
					curPoint.Y += frame.Size.Height + SEPARATION;
				}
				break;

			case Layout.RowLayout:
				curPoint = new PointF(0.0f , simpleView.Bounds.Size.Height / 2.0f);
				foreach (NSView subview in subviews) {
					RectangleF frame = new RectangleF(curPoint.X, curPoint.Y - BOX_HEIGHT /2.0f, BOX_WIDTH, BOX_HEIGHT);
					animateView(subview, frame);
					curPoint.X += frame.Size.Width + SEPARATION;
				}
				break;

			case Layout.GridLayout:
				curPoint = new PointF(0.0f, 0.0f);
				int viewsPerSide = (int)Math.Ceiling( Math.Sqrt(subviews.Count()) ); 
				
				int idx = 0;
				foreach (NSView subview in subviews) {
					RectangleF frame = new RectangleF(curPoint.X, curPoint.Y, BOX_WIDTH, BOX_HEIGHT);
					
					animateView(subview, frame);
					curPoint.X += frame.Size.Width + SEPARATION;
					
					if (++idx % viewsPerSide == 0) {
						curPoint.X = 0;
						curPoint.Y += BOX_HEIGHT + SEPARATION;
					}
				}
				break;
				
			}
		}
		
		// Helper method to animate the sub view
		private void animateView(NSView subView, RectangleF toFrame) 
		{
#if true
			// Simple animation: assign the new value, and let CoreAnimation
			// take it from here
			
			((NSView) subView.Animator).Frame = toFrame;
#else
			//
			// Performing the animation by hand, every step of the way
			//
			var animationY = CABasicAnimation.FromKeyPath("position.y");
			animationY.To = NSNumber.FromFloat(toFrame.Y);
			animationY.AnimationStopped += delegate {
				//Console.WriteLine("animation stopped");
				subView.Layer.Frame = toFrame;
			};
			
			var animationX = CABasicAnimation.FromKeyPath("position.x");
			animationX.To = NSNumber.FromFloat(toFrame.X);
			
			animationY.AutoReverses = false;
			animationX.AutoReverses = false;
			
			animationY.RemovedOnCompletion = false;
			animationX.RemovedOnCompletion = false;
			
			animationY.FillMode = CAFillMode.Forwards;
			animationX.FillMode = CAFillMode.Forwards;
			
			subView.Layer.AddAnimation(animationX,"moveX");
			subView.Layer.AddAnimation(animationY,"moveY");
#endif
		}
	}
}

