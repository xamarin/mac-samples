using System;
using Foundation;
using AppKit;

namespace MacCopyPaste
{
	public partial class ImageWindow : NSWindow
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

		public ViewController ImageViewController {
			get { return ContentViewController as ViewController; }
		}

		public NSImage Image {
			get {
				return ImageViewController.Image;
			}
			set {
				ImageViewController.Image = value;
			}
		}
		#endregion

		#region Constructor
		public ImageWindow (IntPtr handle) : base (handle)
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
			Document.ImageView = ImageViewController.ContentView;
		}
		#endregion

		#region Public Methods
		public void CopyImage (NSObject sender)
		{
			Document.CopyImage (sender);
		}

		public void PasteImage (NSObject sender)
		{
			Document.PasteImage (sender);
		}

		public void ImageOne (NSObject sender)
		{
			// Load image
			Image = NSImage.ImageNamed ("Image01.jpg");

			// Set image info
			Document.Info.Name = "city";
			Document.Info.ImageType = "jpg";
		}

		public void ImageTwo (NSObject sender)
		{
			// Load image
			Image = NSImage.ImageNamed ("Image02.jpg");

			// Set image info
			Document.Info.Name = "theater";
			Document.Info.ImageType = "jpg";
		}

		public void ImageThree (NSObject sender)
		{
			// Load image
			Image = NSImage.ImageNamed ("Image03.jpg");

			// Set image info
			Document.Info.Name = "keyboard";
			Document.Info.ImageType = "jpg";
		}

		public void ImageFour (NSObject sender)
		{
			// Load image
			Image = NSImage.ImageNamed ("Image04.jpg");

			// Set image info
			Document.Info.Name = "trees";
			Document.Info.ImageType = "jpg";
		}
		#endregion
	}
}

