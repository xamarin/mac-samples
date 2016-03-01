using System;
using Foundation;
using AppKit;

namespace MacInspector
{
	/// <summary>
	/// Controls the adjustable properties of a <see cref="T:MacInspector.CustomBox"/> when displayed
	/// in an Inspector Panel.
	/// </summary>
	/// <remarks>
	/// This class uses key-value coding and data binding to set the properties. Please see:
	/// https://developer.xamarin.com/guides/mac/application_fundamentals/databinding/
	/// </remarks>
	public partial class BoxPrefsViewController : NSViewController
	{
		#region Private Variables
		/// <summary>
		/// The backing store for the <see cref="T:MacInspector.CustomBox"/> that properties
		/// are being edited on.
		/// </summary>
		private CustomBox _box = null;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the <see cref="T:MacInspector.CustomBox"/> that properties
		/// are being edited on.
		/// </summary>
		/// <value>The box.</value>
		[Export("Box")]
		public CustomBox Box {
			get { return _box; }
			set {
				WillChangeValue ("Box");
				_box = value;
				DidChangeValue ("Box");
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.BoxPrefsViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public BoxPrefsViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion
	}
}
