using System;
using System.IO;
using CoreGraphics;

using Foundation;
using AppKit;
using WebKit;
using CoreServices;

namespace Markdown
{
	public partial class MyDocument : AppKit.NSDocument
	{
		NSUrl documentUrl;
		NSData documentData;
		FSEventStream fsEvents;

		public MyDocument (IntPtr handle) : base (handle)
		{
		}

	/*	[Export ("initWithCoder:")]
		public MyDocument (NSCoder coder) : base (coder)
		{
		}*/

		class MarkdownWebUIDelegate : WebUIDelegate
		{
			public override NSMenuItem[] UIGetContextMenuItems (WebView sender,
				NSDictionary forElement, NSMenuItem[] defaultMenuItems)
			{
				return null;
			}
		}

		class MarkdownWebPolicyDelegate : WebPolicyDelegate
		{
			public override void DecidePolicyForNavigation (WebView webView, NSDictionary actionInformation, NSUrlRequest request, WebFrame frame, NSObject decisionToken)
			{
				switch ((WebNavigationType)((NSNumber)actionInformation [WebPolicyDelegate.WebActionNavigationTypeKey]).Int32Value) {
				case WebNavigationType.BackForward:
				case WebNavigationType.FormResubmitted:
				case WebNavigationType.FormSubmitted:
					WebPolicyDelegate.DecideIgnore (decisionToken);
					break;
				case WebNavigationType.LinkClicked:
					NSWorkspace.SharedWorkspace.OpenUrl (actionInformation[WebPolicyDelegate.WebActionOriginalUrlKey] as NSUrl);
					WebPolicyDelegate.DecideIgnore (decisionToken);
					break;
				case WebNavigationType.Other:
				case WebNavigationType.Reload:
					WebPolicyDelegate.DecideUse (decisionToken);
					break;
				}
			}
		}

		class MarkdownWebFrameLoadDelegate : WebFrameLoadDelegate
		{
			MyDocument document;

			public MarkdownWebFrameLoadDelegate (MyDocument document)
			{
				this.document = document;
			}

			public override void FinishedLoad (WebView sender, WebFrame forFrame)
			{
				document.ReloadDocument ();
			}
		}

		public override void WindowControllerDidLoadNib (NSWindowController windowController)
		{
			base.WindowControllerDidLoadNib (windowController);

			WebView.UIDelegate = new MarkdownWebUIDelegate ();
			WebView.PolicyDelegate = new MarkdownWebPolicyDelegate ();
			WebView.FrameLoadDelegate = new MarkdownWebFrameLoadDelegate (this);

			var templatePath = Path.Combine (NSBundle.MainBundle.ResourcePath, "Template.html");
			WebView.MainFrame.LoadRequest (new NSUrlRequest (new NSUrl (templatePath)));
		}

		public override bool ReadFromUrl (NSUrl url, string typeName, out NSError outError)
		{
			fsEvents = new FSEventStream (new [] { Path.GetDirectoryName (url.Path) },
				TimeSpan.FromSeconds (0), FSEventStreamCreateFlags.FileEvents);

			fsEvents.Events += (sender, e) => {
				foreach (var evnt in e.Events) {
					if (evnt.Path == url.Path && evnt.Flags.HasFlag (FSEventStreamEventFlags.ItemModified)) {
						ReloadDocument ();
						break;
					}
				}
			};

			fsEvents.ScheduleWithRunLoop (NSRunLoop.Main);
			fsEvents.Start ();

			documentUrl = url;
			outError = null;
			return true;
		}

		void ReloadDocument ()
		{
			documentData = NSData.FromUrl (documentUrl);

			var document = WebView.MainFrame.DomDocument;
			var container = document.GetElementById ("markdown-container");
			if (container != null) {
				using (var sundown = new Sundown.Renderer ()) {
					((DomHtmlElement)container).InnerHTML = sundown.Render (documentData.ToString ());
				}

				document.EvaluateWebScript ("markdownUpdated ()");
			}
		}

		public override string WindowNibName { 
			get { return "MyDocument"; }
		}
	}
}