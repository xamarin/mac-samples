using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace ImageProtocolSample
{
	public class ImageProtocol : NSUrlProtocol
	{
		[Export ("canInitWithRequest:")]
		public static bool canInitWithRequest (NSUrlRequest request)
		{
			return request.Url.Scheme == "custom";
		}

		[Export ("canonicalRequestForRequest:")]
		public static new NSUrlRequest GetCanonicalRequest (NSUrlRequest forRequest)
		{
			return forRequest;
		}

		[Export ("initWithRequest:cachedResponse:client:")]
		public ImageProtocol (NSUrlRequest request, NSCachedUrlResponse cachedResponse, NSUrlProtocolClient client)
			: base (request, cachedResponse, client)
		{
		}

		NSData ImageAsJPEG (NSImage i)
		{
			NSData d = i.AsTiff ();
			NSBitmapImageRep rep = new NSBitmapImageRep (d);
			return rep.RepresentationUsingTypeProperties (NSBitmapImageFileType.Jpeg, NSDictionary.FromObjectAndKey (NSNumber.FromInt32 (1), NSBitmapImageRep.CompressionFactor));
		}

		public override void StartLoading ()
		{
			var value = Request.Url.Path.Substring (1);
			using (var image = Render (value))
			{
				using (var response = new NSUrlResponse (Request.Url, "image/jpeg", -1, null))
				{
					var client = (NSUrlProtocolClient)this.WeakClient;
					client.ReceivedResponse (this, response, NSUrlCacheStoragePolicy.NotAllowed);
					this.InvokeOnMainThread (delegate
					{
						using (var data = ImageAsJPEG (image))
						{
							client.DataLoaded (this, data);
						}
						client.FinishedLoading (this);
					});
				}
			}
		}

		public override void StopLoading ()
		{
		}

		static NSImage Render (string value)
		{
			NSImage image = null;

			NSApplication.SharedApplication.InvokeOnMainThread (() =>
			{
				NSString text = new NSString (string.IsNullOrEmpty (value) ? " " : value);
				NSFont font = NSFont.FromFontName ("Arial", 20);
				var fontDictionary = NSDictionary.FromObjectsAndKeys (new NSObject[] { font, NSColor.Red }, new NSObject[] { NSStringAttributeKey.Font, NSStringAttributeKey.ForegroundColor });
				CGSize size = text.StringSize (fontDictionary);

				image = new NSImage (new CGSize (size));

				image.LockFocus ();
				text.DrawString (new CGPoint (0, 0), fontDictionary);
				image.UnlockFocus ();
			});

			return image;
		}
	}
}
