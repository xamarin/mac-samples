using System;

using AppKit;
using Foundation;

namespace MacCopyPaste
{
	public partial class ViewController : NSViewController
	{
		#region Computed Properties
		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}

		public NSImage Image {
			get {
				return ImageView.Image;
			}
			set {
				ImageView.Image = value;
			}
		}
			
		public NSImageView ContentView {
			get {
				return ImageView;
			}
			set {
				ImageView = value;
			}
		}
		#endregion

		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any additional setup after loading the view.
		}
		#endregion

	}
}
