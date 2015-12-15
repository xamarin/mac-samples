using System;
using Foundation;
using AppKit;

namespace MacCopyPaste
{
	public partial class ImageWindowController : NSWindowController
	{
		#region Computed Properties
		public ImageWindow ContentWindow {
			get { return Window as ImageWindow; }
		}
		#endregion

		#region Constructors
		public ImageWindowController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Actions
		partial void CopyImage (NSObject sender)
		{
			ContentWindow.CopyImage (sender);
		}

		partial void PasteImage (NSObject sender)
		{
			ContentWindow.PasteImage (sender);
		}

		partial void ImageOne (NSObject sender)
		{
			ContentWindow.ImageOne (sender);
		}

		partial void ImageTwo (NSObject sender)
		{
			ContentWindow.ImageTwo (sender);
		}

		partial void ImageThree (NSObject sender)
		{
			ContentWindow.ImageThree (sender);
		}

		partial void ImageFour (NSObject sender)
		{
			ContentWindow.ImageFour (sender);
		}
		#endregion
	}
}
