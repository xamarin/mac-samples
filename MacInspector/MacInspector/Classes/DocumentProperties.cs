using System;
using Foundation;
using AppKit;

namespace MacInspector
{
	/// <summary>
	/// Handles the properties for the document being displayed in the current window.
	/// </summary>
	/// <remarks>
	/// This class uses key-value coding and data binding to set the properties. Please see:
	/// https://developer.xamarin.com/guides/mac/application_fundamentals/databinding/
	/// </remarks>
	[Register("DocumentProperties")]
	public class DocumentProperties : NSObject
	{
		#region Private Variables
		/// <summary>
		/// The backing store for the title.
		/// </summary>
		private string _title = "Untitled Drawing";

		/// <summary>
		/// The backing store for the width.
		/// </summary>
		private int _width = 500;

		/// <summary>
		/// The backing store for the height
		/// </summary>
		private int _height = 500;

		/// <summary>
		/// The backing store for the background color.
		/// </summary>
		private NSColor _backgroundColor = NSColor.White;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		[Export("Title")]
		public string Title {
			get { return _title; }
			set {
				WillChangeValue ("Title");
				_title = value;
				DidChangeValue ("Title");
			}
		}

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>The width.</value>
		[Export("Width")]
		public int Width {
			get { return _width; }
			set {
				WillChangeValue ("Width");
				_width = value;
				DidChangeValue ("Width");
			}
		}

		/// <summary>
		/// Gets or sets the height.
		/// </summary>
		/// <value>The height.</value>
		[Export("Height")]
		public int Height {
			get { return _height; }
			set {
				WillChangeValue ("Height");
				_height = value;
				DidChangeValue ("Height");
			}
		}

		/// <summary>
		/// Gets or sets the color of the background.
		/// </summary>
		/// <value>The color of the background.</value>
		[Export("BackgroundColor")]
		public NSColor BackgroundColor {
			get { return _backgroundColor; }
			set {
				WillChangeValue ("BackgroundColor");
				_backgroundColor = value;
				DidChangeValue ("BackgroundColor");
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.DocumentProperties"/> class.
		/// </summary>
		public DocumentProperties ()
		{
		}
		#endregion
	}
}

