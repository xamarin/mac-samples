using System;
using Foundation;
using AppKit;

namespace MacInspector
{
	/// <summary>
	/// Defines a custom <c>NSBox</c> that allow the user to adjust its properties using a 
	/// <see cref="T:MacInspector.BoxPrefsViewController"/> and responds to the user clicking
	/// on the box.
	/// </summary>
	/// <remarks>
	/// This class uses key-value coding and data binding to set the properties. Please see:
	/// https://developer.xamarin.com/guides/mac/application_fundamentals/databinding/
	/// </remarks>
	[Register("CustomBox")]
	public class CustomBox : NSBox
	{
		/// <summary>
		/// Gets or sets the box title.
		/// </summary>
		/// <value>The box title.</value>
		#region Computed Properties
		[Export("BoxTitle")]
		public string BoxTitle {
			get { return Title; }
			set {
				WillChangeValue ("BoxTitle");
				Title = value;
				DidChangeValue ("BoxTitle");
			}
		}

		/// <summary>
		/// Gets or sets the color of the box border.
		/// </summary>
		/// <value>The color of the box border.</value>
		[Export ("BoxBorderColor")]
		public NSColor BoxBorderColor {
			get { return BorderColor; }
			set {
				WillChangeValue ("BoxBorderColor");
				BorderColor = value;
				DidChangeValue ("BoxBorderColor");
			}
		}

		/// <summary>
		/// Gets or sets the width of the box border.
		/// </summary>
		/// <value>The width of the box border.</value>
		[Export ("BoxBorderWidth")]
		public nfloat BoxBorderWidth {
			get { return BorderWidth; }
			set {
				WillChangeValue ("BoxBorderWidth");
				BorderWidth = value;
				DidChangeValue ("BoxBorderWidth");
			}
		}

		/// <summary>
		/// Gets or sets the color of the box fill.
		/// </summary>
		/// <value>The color of the box fill.</value>
		[Export ("BoxFillColor")]
		public NSColor BoxFillColor {
			get { return FillColor; }
			set {
				WillChangeValue ("BoxFillColor");
				FillColor = value;
				DidChangeValue ("BoxFillColor");
			}
		}

		/// <summary>
		/// Gets or sets the adjustable state of the box's <c>BoxBorderWidth</c>, <c>BoxBorderColor</c>
		/// and <c>BoxFillColor</c>.
		/// </summary>
		/// <value>If <c>true</c>, the <c>BoxBorderWidth</c>, <c>BoxBorderColor</c>
		/// and <c>BoxFillColor</c> are adjustable, else not.</value>
		[Export("Adjustable")]
		public bool Adjustable {
			get { return (BoxType == NSBoxType.NSBoxCustom); }
			set {
				WillChangeValue ("Adjustable");
				if (value) {
					BoxType = NSBoxType.NSBoxCustom;
				} else {
					BoxType = NSBoxType.NSBoxPrimary;
				}
				DidChangeValue ("Adjustable");
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.CustomBox"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public CustomBox (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Handles the mouse going down on the box.
		/// </summary>
		/// <returns>The down.</returns>
		/// <param name="theEvent">The event.</param>
		public override void MouseDown (NSEvent theEvent)
		{
			base.MouseDown (theEvent);

			// Inform caller we've been clicked
			RaiseBoxClicked ();
		}

		/// <summary>
		/// Handles the mouse being moved while down on the box.
		/// </summary>
		/// <returns>The moved.</returns>
		/// <param name="theEvent">The event.</param>
		public override void MouseMoved (NSEvent theEvent)
		{
			base.MouseMoved (theEvent);
		}

		/// <summary>
		/// Handles the mouse moving outside of the box's bounds while
		/// down.
		/// </summary>
		/// <returns>The exited.</returns>
		/// <param name="theEvent">The event.</param>
		public override void MouseExited (NSEvent theEvent)
		{
			base.MouseExited (theEvent);
		}

		/// <summary>
		/// Handles the mouse being released over the box.
		/// </summary>
		/// <returns>The up.</returns>
		/// <param name="theEvent">The event.</param>
		public override void MouseUp (NSEvent theEvent)
		{
			base.MouseUp (theEvent);
		}
		#endregion

		#region Events
		/// <summary>
		/// Box clicked delegate.
		/// </summary>
		public delegate void BoxClickedDelegate(CustomBox box);

		/// <summary>
		/// Occurs when box clicked.
		/// </summary>
		public event BoxClickedDelegate BoxClicked;

		/// <summary>
		/// Raises the box clicked event.
		/// </summary>
		/// <returns>The box clicked.</returns>
		internal void RaiseBoxClicked ()
		{
			if (this.BoxClicked != null) this.BoxClicked (this);
		}
		#endregion
	}
}

