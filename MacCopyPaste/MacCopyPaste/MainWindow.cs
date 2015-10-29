using System;

using AppKit;
using Foundation;

namespace MacCopyPaste
{
	public partial class MainWindow : NSWindow
	{
		#region Private Variables
		ImageDocument document;
		#endregion

		#region Computed Properties
		[Export ("Document")]
		public ImageDocument Document {
			get {
				return document;
			}
			set {
				WillChangeValue ("Document");
				document = value;
				DidChangeValue ("Document");
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
		#endregion

		#region Constructors
		public MainWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Create a new document instance
			Document = new ImageDocument ();

			// Attach to image view
			Document.ImageView = ImageView;
		}
		#endregion

		#region Actions
		partial void CopyImage (NSObject sender)
		{
			Document.CopyImage (sender);
		}

		partial void PasteImage (NSObject sender)
		{
			Document.PasteImage (sender);
		}

		partial void ImageOne (NSObject sender)
		{
			// Load image
			ImageView.Image = NSImage.ImageNamed ("Image01.jpg");

			// Set image info
			Document.Info.Name = "city";
			Document.Info.ImageType = "jpg";
		}

		partial void ImageTwo (NSObject sender)
		{
			// Load image
			ImageView.Image = NSImage.ImageNamed ("Image02.jpg");

			// Set image info
			Document.Info.Name = "theater";
			Document.Info.ImageType = "jpg";
		}

		partial void ImageThree (NSObject sender)
		{
			// Load image
			ImageView.Image = NSImage.ImageNamed ("Image03.jpg");

			// Set image info
			Document.Info.Name = "keyboard";
			Document.Info.ImageType = "jpg";
		}

		partial void ImageFour (NSObject sender)
		{
			// Load image
			ImageView.Image = NSImage.ImageNamed ("Image04.jpg");

			// Set image info
			Document.Info.Name = "trees";
			Document.Info.ImageType = "jpg";
		}
		#endregion
	}
}
