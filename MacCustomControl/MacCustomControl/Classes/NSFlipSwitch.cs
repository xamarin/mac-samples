using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using AppKit;
using CustomGraphics;
using CoreGraphics;

namespace MacCustomControl
{
	/// <summary>
	/// This class implements a custom Flip Switch User Interface Control by inheriting from
	/// <c>NSControl</c> and custom drawing it's UI state. It demonstraits how to handle mouse
	/// input using both override methods and gesture recognizers. 
	/// </summary>
	/// <remarks>
	/// The UI for the control was designed in PainCode for Xamarin.iOS and uses the 
	/// extension methods and shims in the <c>UIKit</c> folder to consume this code unchanged.
	/// </remarks>
	[Register("NSFlipSwitch")]
	public class NSFlipSwitch : NSControl
	{
		#region Private Variables
		private bool _value = false;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="MacCustomControl.NSFlipSwitch"/> is
		/// On or Off.
		/// </summary>
		/// <value><c>true</c> if value; otherwise, <c>false</c>.</value>
		public bool Value {
			get { return _value; }
			set {
				// Save value and force a redraw
				_value = value;
				NeedsDisplay = true;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MacCustomControl.NSFlipSwitch"/> class.
		/// </summary>
		public NSFlipSwitch ()
		{
			// Init
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MacCustomControl.NSFlipSwitch"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public NSFlipSwitch (IntPtr handle) : base (handle)
		{
			// Init
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MacCustomControl.NSFlipSwitch"/> class.
		/// </summary>
		/// <param name="frameRect">Frame rect.</param>
		[Export ("initWithFrame:")]
		public NSFlipSwitch (CGRect frameRect) : base(frameRect) {
			// Init
			Initialize();
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		private void Initialize() {
			this.WantsLayer = true;
			this.LayerContentsRedrawPolicy = NSViewLayerContentsRedrawPolicy.OnSetNeedsDisplay;

			// --------------------------------------------------------------------------------
			// Handle mouse with Gesture Recognizers.
			// NOTE: Use either this method or the Override Methods, NOT both!
			// --------------------------------------------------------------------------------
			var click = new NSClickGestureRecognizer (() => {
				FlipSwitchState();
			});
			AddGestureRecognizer (click);
		}
		#endregion

		#region Draw Methods
		/// <summary>
		/// Draws the User Interface for this custom control
		/// </summary>
		/// <param name="dirtyRect">Dirty rect.</param>
		public override void DrawRect (CGRect dirtyRect)
		{
			base.DrawRect (dirtyRect);

			// --------------------------------------------------------------------------------
			// We are using a custom control UI that was drawn in PaintCode
			// for iOS. The extensions methods and shims in the UIKit folder
			// allow us to consume this code unchanged.
			// --------------------------------------------------------------------------------
			CustomControlsStyleKit.DrawUISwitch (Enabled, Value);

		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Flips the state of the switch between On and Off
		/// </summary>
		private void FlipSwitchState() {
			// Update state
			Value = !Value;
			RaiseValueChanged ();
		}
		#endregion

		#region Mouse Handling Methods
		// --------------------------------------------------------------------------------
		// Handle mouse with Override Methods.
		// NOTE: Use either this method or Gesture Recognizers, NOT both!
		// --------------------------------------------------------------------------------
//		public override void MouseDown (NSEvent theEvent)
//		{
//			base.MouseDown (theEvent);
//
//			FlipSwitchState ();
//		}
//
//		public override void MouseDragged (NSEvent theEvent)
//		{
//			base.MouseDragged (theEvent);
//		}
//
//		public override void MouseUp (NSEvent theEvent)
//		{
//			base.MouseUp (theEvent);
//		}
//
//		public override void MouseMoved (NSEvent theEvent)
//		{
//			base.MouseMoved (theEvent);
//		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs when value of the switch is changed.
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		/// Raises the value changed event.
		/// </summary>
		internal void RaiseValueChanged() {
			if (this.ValueChanged != null)
				this.ValueChanged (this, EventArgs.Empty);

			// Perform any action bound to the control from Interface Builder
			// via an Action.
			if (this.Action !=null) 
				NSApplication.SharedApplication.SendAction (this.Action, this.Target, this);
		}
		#endregion
	}
}

