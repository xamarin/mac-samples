using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace Constraints
{
	[global::Foundation.Register ("MainWindow")]
	public class MainWindow : NSWindow
	{
		const int PADDING = 10;

		#region Constructors
		public MainWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
		}

		public MainWindow(CGRect contentRect, NSWindowStyle aStyle, NSBackingStore bufferingType, bool deferCreation): base (contentRect, aStyle,bufferingType,deferCreation) {
			var windowSize = new CGSize (640, 480);
			var windowLocation = new CGPoint (NSScreen.MainScreen.Frame.Width / 2 - windowSize.Width / 2, NSScreen.MainScreen.Frame.Height / 2 - windowSize.Height / 2);

			var centerRect = new CGRect (windowLocation, windowSize);

			Title = "Programmatic window";

			ContentView = new NSView (centerRect);

			var title = new NSTextField ();
			title.StringValue = "Title your problem";
			title.Editable = true;
			title.UsesSingleLineMode = true;
			title.PlaceholderString = "Title your problem";
			title.AccessibilityLabel = title.PlaceholderString;

			title.TranslatesAutoresizingMaskIntoConstraints = false;
			ContentView.AddSubview (title);

			// There are three ways to set auto layout constraints programmatically.  The first is by setting layout anchors 
			// https://developer.apple.com/library/content/documentation/UserExperience/Conceptual/AutolayoutPG/ProgrammaticallyCreatingConstraints.html#//apple_ref/doc/uid/TP40010853-CH16-SW5
			title.LeadingAnchor.ConstraintEqualToAnchor (ContentView.LeadingAnchor, PADDING).Active = true; // Don't forget to set .Active = true on the constraint or it won't show up
			title.TrailingAnchor.ConstraintEqualToAnchor (ContentView.TrailingAnchor, -PADDING).Active = true;
			title.TopAnchor.ConstraintEqualToAnchor (ContentView.TopAnchor, PADDING).Active = true;

			var scroll = new NSScrollView (new CGRect (0, 0, ContentView.Frame.Width - PADDING - PADDING, 100));
			scroll.BorderType = NSBorderType.BezelBorder;
			scroll.HasHorizontalScroller = false;
			scroll.HasVerticalScroller = true;
			var scrollSize = scroll.ContentSize;

			var description = new NSTextView (new CGRect (0, 0, scrollSize.Width, scrollSize.Height));
			description.MinSize = new CGSize (0, scrollSize.Height);
			description.MaxSize = new CGSize (float.MaxValue, float.MaxValue);
			description.Editable = true;
			description.Font = title.Font;
			description.VerticallyResizable = true;
			description.HorizontallyResizable = false;
			description.AutoresizingMask = NSViewResizingMask.WidthSizable;

			description.TextContainer.ContainerSize = new CGSize (scrollSize.Width, float.MaxValue);
			description.TextContainer.WidthTracksTextView = true;


			scroll.DocumentView = description;

			scroll.TranslatesAutoresizingMaskIntoConstraints = false;
			ContentView.AddSubview (scroll);

			// The second option is to create NSLayoutConstraints
			// https://developer.apple.com/library/content/documentation/UserExperience/Conceptual/AutolayoutPG/ProgrammaticallyCreatingConstraints.html#//apple_ref/doc/uid/TP40010853-CH16-SW8
			ContentView.AddConstraints (new [] {
				NSLayoutConstraint.Create(scroll, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Leading, 1, PADDING),
				NSLayoutConstraint.Create(scroll, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Trailing, 1, -PADDING)
			});

			// Alternatively, you can create the constraints as shown here and set .Active = true, same as with the anchor method above
			NSLayoutConstraint.Create (scroll, NSLayoutAttribute.Top, NSLayoutRelation.Equal, title, NSLayoutAttribute.Bottom, 1, PADDING).Active = true;
		
			title.Activated += (sender, e) => this.MakeFirstResponder (description);

			var labelFont = NSFont.LabelFontOfSize (10);

			var publicLabel = new NSTextField () {
				StringValue = "Your title and description will be public",
				Editable = false,
				Bezeled = false,
				DrawsBackground = false,
				Selectable = false,
				Font = labelFont,
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			ContentView.AddSubview (publicLabel);

			// You can also use different types of constraints for the same view
			ContentView.AddConstraints (new [] {
				NSLayoutConstraint.Create(publicLabel, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, ContentView, NSLayoutAttribute.Leading, 1, 40),
				NSLayoutConstraint.Create(publicLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Trailing, 1, -PADDING),
			});
			publicLabel.TopAnchor.ConstraintEqualToAnchor (scroll.BottomAnchor, 5).Active = true;

			var email = new NSTextField {
				Editable = true,
				PlaceholderString = "Optional email address",
			};

			email.AccessibilityLabel = email.PlaceholderString;

			email.TranslatesAutoresizingMaskIntoConstraints = false;
			ContentView.AddSubview (email);
			ContentView.AddConstraints (new [] {
				NSLayoutConstraint.Create(email, NSLayoutAttribute.Top, NSLayoutRelation.Equal, publicLabel, NSLayoutAttribute.Bottom, 1, PADDING),
			});

			// The third option for setting layout constraints is to use Visual Format Language
			// https://developer.apple.com/library/content/documentation/UserExperience/Conceptual/AutolayoutPG/ProgrammaticallyCreatingConstraints.html#//apple_ref/doc/uid/TP40010853-CH16-SW9
			string emailFormat = "|-10-[email]-10-|";
			NSDictionary emailViews = NSDictionary.FromObjectAndKey (email, (NSString)"email");
			var emailConstraints = NSLayoutConstraint.FromVisualFormat (emailFormat, NSLayoutFormatOptions.None, null, emailViews);
			NSLayoutConstraint.ActivateConstraints (emailConstraints);

			var sendButton = new NSButton {
				Title = "OK"
			};

			sendButton.Activated += (sender, e) => {
				NSAlert.WithMessage ("Button pressed", "Okay", null, null, "").RunModal ();
				Dispose ();
			};

			sendButton.TranslatesAutoresizingMaskIntoConstraints = false;
			ContentView.AddSubview (sendButton);
			ContentView.AddConstraints (new [] {
				NSLayoutConstraint.Create(sendButton, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Trailing, 1, -PADDING),
				NSLayoutConstraint.Create(sendButton, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, ContentView, NSLayoutAttribute.Leading, 1, 40),
			});

			//To do vertical constraints with visual format language, start the format string with V:"
			string sendButtonFormat = "V:[email]-10-[sendButton]-10-|";
			var sendButtonViews = NSDictionary.FromObjectsAndKeys (new NSObject [] { email, sendButton }, new NSObject [] { (NSString)"email", (NSString)"sendButton" });
			var sendButtonConstraints = NSLayoutConstraint.FromVisualFormat (sendButtonFormat, NSLayoutFormatOptions.None, null, sendButtonViews);
			NSLayoutConstraint.ActivateConstraints (sendButtonConstraints);

			email.Activated += (sender, e) => {
				if (sendButton.Enabled) {
					this.MakeFirstResponder (sendButton);
				}
			};

			bool hasTitle = false;
			bool hasDescription = false;

			title.Changed += (sender, e) => {
				var titleStr = title.StringValue;
				hasTitle = !string.IsNullOrWhiteSpace (titleStr) && titleStr.Length > 5;
				sendButton.Enabled = hasTitle && hasDescription;
			};

			description.TextStorage.DidProcessEditing += (sender, e) => {
				hasDescription = description.TextStorage.Length > 10;
				sendButton.Enabled = hasTitle && hasDescription;
			};
		}
		#endregion
	}
}
