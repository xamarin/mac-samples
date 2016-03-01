using System;
using Foundation;
using AppKit;

namespace MacInspector
{
	/// <summary>
	/// Controls the adjustable properties of a <see cref="T:MacInspector.DocumentProperties"/> when displayed
	/// in an Inspector Panel.
	/// </summary>
	/// <remarks>
	/// This class uses key-value coding and data binding to set the properties. Please see:
	/// https://developer.xamarin.com/guides/mac/application_fundamentals/databinding/
	/// </remarks>
	public partial class DocPrefsViewController : NSViewController
	{
		#region Private Variables
		/// <summary>
		/// The backing store for the <see cref="T:MacInspector.DocumentProperties"/>.
		/// </summary>
		private DocumentProperties _properties;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the properties.
		/// </summary>
		/// <value>The <see cref="T:MacInspector.DocumentProperties"/>.</value>
		[Export("Properties")]
		public DocumentProperties Properties {
			get { return _properties; }
			set {
				WillChangeValue ("Properties");
				_properties = value;
				DidChangeValue ("Properties");
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.DocPrefsViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public DocPrefsViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion
	}
}
