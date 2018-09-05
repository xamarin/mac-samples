using System;

using AppKit;
using Foundation;

namespace CircleView
{
	public partial class ViewController : NSViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		partial void ApplyRadius (Foundation.NSObject sender)
		{
			CircleView.Radius = ((NSSlider)sender).DoubleValue;
		}

		partial void ApplyColor (Foundation.NSObject sender)
		{
			CircleView.TextColor = ((NSColorWell)sender).Color;
		}

		partial void ApplySpin (Foundation.NSObject sender)
		{
			CircleView.ToggleAnimation ();
		}

		partial void ApplyStartingAngle (Foundation.NSObject sender)
		{
			CircleView.StartingAngle = ((NSSlider)sender).DoubleValue;
		}

		partial void ApplyText (Foundation.NSObject sender)
		{
			CircleView.Text = ((NSTextField)sender).StringValue;
		}

		public override void MouseDown (NSEvent theEvent)
		{
			AdjustView (theEvent);
		}

		public override void MouseDragged (NSEvent theEvent)
		{
			AdjustView (theEvent);
		}

		void AdjustView (NSEvent theEvent)
		{
			CircleView.Center = CircleView.ConvertPointFromView (theEvent.LocationInWindow, CircleView);
		}

	}
}
