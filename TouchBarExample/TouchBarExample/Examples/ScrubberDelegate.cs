using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace TouchBarExample
{
	public class ScrubberDelegate : TouchBarExampleWithPopover
	{
		public override int Count => 2;

		public override NSTouchBarItem MakeItem (NSTouchBar touchBar, string identifier)
		{
			if (IsPopupID (identifier)) {
				NSCustomTouchBarItem item = new NSCustomTouchBarItem (identifier);
				NSScrubber scrubber = new NSScrubber () {
					Identifier = identifier,
					SelectedIndex = 0,
					ShowsArrowButtons = true,
					SelectionOverlayStyle = NSScrubberSelectionStyle.OutlineOverlayStyle,
					SelectionBackgroundStyle = NSScrubberSelectionStyle.RoundedBackgroundStyle,
				};

				item.View = scrubber;

				switch (ParseNestedId (identifier)) {
				case 0: {
						var data = new TextScrubberData ();

						scrubber.Delegate = data;
						scrubber.DataSource = data;

						scrubber.RegisterClass (new Class ("NSScrubberTextItemView"), "com.xamarin.scrubber.text");
	
						return item;
					}
				case 1: {
						var data = new ImageScrubberData ();

						scrubber.Delegate = data;
						scrubber.DataSource = data;

						scrubber.RegisterClass (new Class (typeof (ScrubberImage)), "com.xamarin.scrubber.image");
						scrubber.ShowsAdditionalContentIndicators = true;

						return item;
					}
				}

			}
			else {
				switch (ParseId (identifier)) {
				case 0: {
						NSPopoverTouchBarItem item = new NSPopoverTouchBarItem (identifier);
						item.PopoverTouchBar = new NSTouchBar () {
							Delegate = this,
							DefaultItemIdentifiers = new string [] { CreateNestedID (0) }
						};
						item.CollapsedRepresentationLabel = "NSScrubber Text";
						return item;
					}
				case 1: {
						NSPopoverTouchBarItem item = new NSPopoverTouchBarItem (identifier);
						item.PopoverTouchBar = new NSTouchBar () {
							Delegate = this,
							DefaultItemIdentifiers = new string [] { CreateNestedID (1) }
						};
						item.CollapsedRepresentationLabel = "NSScrubber Image";
						return item;
					}
				}
			}

			return null;
		}
	}

	abstract class BaseScrubberData : NSObject, INSScrubberDelegate
	{
		[Export ("didBeginInteractingWithScrubber:")]
		public void DidBeginInteracting (NSScrubber scrubber)
		{
			Console.WriteLine ("Scrubber DidBeginInteracting");
		}

		[Export ("scrubber:didSelectItemAtIndex:")]
		public void DidSelectItem (NSScrubber scrubber, nint selectedIndex)
		{
			Console.WriteLine ("Scrubber DidSelectItem - " + selectedIndex);
		}

		[Export ("didFinishInteractingWithScrubber:")]
		public void DidFinishInteracting (NSScrubber scrubber)
		{
			Console.WriteLine ("Scrubber DidFinishInteracting");
		}
	}

	public class ScrubberImage : NSScrubberItemView
	{
		NSImageView image;

		public ScrubberImage (IntPtr p) : base (p)
		{
		}

		[Export ("initWithFrame:")]
		public ScrubberImage (CGRect rect) : base ()
		{
			WantsLayer = true;

			image = new NSImageView (rect) {
				AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable
			};

			AddSubview (image);
		}

		public NSImage Image {
			get {
				return image.Image;
			}
			set {
				image.Image = value;
			}
		}
	}

	class ImageScrubberData : BaseScrubberData, INSScrubberDataSource
	{
		public nint GetNumberOfItems (NSScrubber scrubber)
		{
			return 100;
		}

		static Dictionary<int, NSImage> imageCache = new Dictionary<int, NSImage> ();
		static NSImage Xamargon = new NSImage (Path.Combine (NSBundle.MainBundle.BundlePath, "Contents/Resources/Xamagon.png"));

		public NSScrubberItemView GetViewForItem (NSScrubber scrubber, nint index)
		{
			ScrubberImage item = (ScrubberImage)scrubber.MakeItem ("com.xamarin.scrubber.image", null);

			// Tining each image causes lag, so cache and do off the main thread
			// Using ScrubberImage instead of NSScrubberImageItemView since
			// it does not handle setting Image later
			NSImage image;
			if (!imageCache.TryGetValue ((int)index, out image)) {
				var template = (NSImage)Xamargon.Copy ();
				Task.Factory.StartNew (() => {
					var tintedImage = TintImage (template);
					BeginInvokeOnMainThread (() => {
						imageCache [(int)index] = tintedImage;
						item.Image = tintedImage;
					});
				});
			}
			else {
				item.Image = image;
			}

			// Put a nice border around it
			item.Layer.BorderColor = NSColor.DarkGray.CGColor;
			item.Layer.BorderWidth = 2;
			item.Layer.CornerRadius = 8;
			item.Layer.MasksToBounds = true;
			return item;
		}

		Random random = new Random ();

		// Based on http://stackoverflow.com/a/43235
		NSImage TintImage (NSImage image)
		{
			int r = random.Next (128, 255);
			int g = random.Next (128, 255);
			int b = random.Next (128, 255);
			NSColor baseColor = NSColor.White.UsingColorSpace (NSColorSpace.CalibratedRGB);
			r = (int)(r + baseColor.RedComponent) / 2;
			g = (int)(g + baseColor.GreenComponent) / 2;
			b = (int)(b + baseColor.BlueComponent) / 2;
			NSColor color = NSColor.FromRgb (r, g, b);

			NSImage tintedImage = (NSImage)image.Copy ();

			tintedImage.LockFocus ();
			color.Set ();
			NSGraphics.RectFill (new CGRect (0, 0, image.Size.Width, image.Size.Height), NSCompositingOperation.SourceAtop);
			tintedImage.UnlockFocus ();

			return tintedImage;
		}
	}

	class TextScrubberData : BaseScrubberData, INSScrubberDataSource
	{
		public nint GetNumberOfItems (NSScrubber scrubber)
		{
			return 100;
		}

		public NSScrubberItemView GetViewForItem (NSScrubber scrubber, nint index)
		{
			NSScrubberTextItemView item = new NSScrubberTextItemView ();
			item.TextField.StringValue = index.ToString ();
			return item;
		}
	}
}
