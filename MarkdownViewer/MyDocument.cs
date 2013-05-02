using System;
using System.IO;
using System.Drawing;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.WebKit;
using MonoMac.CoreServices;

namespace Markdown
{
	public partial class MyDocument : MonoMac.AppKit.NSDocument
	{
		NSUrl documentUrl;
		NSData documentData;
		FSEventStream fsEvents;

		public MyDocument (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MyDocument (NSCoder coder) : base (coder)
		{
		}

		class MarkdownWebUIDelegate : WebUIDelegate
		{
			public override NSMenuItem[] UIGetContextMenuItems (WebView sender,
				NSDictionary forElement, NSMenuItem[] defaultMenuItems)
			{
				return null;
			}
		}

		public override void WindowControllerDidLoadNib (NSWindowController windowController)
		{
			base.WindowControllerDidLoadNib (windowController);

			WebView.UIDelegate = new MarkdownWebUIDelegate ();
			WebView.DecidePolicyForNavigation += OnDecidePolicyForNavigation;
			WebView.FinishedLoad += (o, e) => ReloadDocument ();

			var templatePath = Path.Combine (NSBundle.MainBundle.ResourcePath, "Template.html");
			WebView.MainFrame.LoadRequest (new NSUrlRequest (new NSUrl (templatePath)));
		}

		void OnDecidePolicyForNavigation (object sender, WebNavigationPolicyEventArgs e)
		{
			switch (e.NavigationType) {
			case WebNavigationType.BackForward:
			case WebNavigationType.FormResubmitted:
			case WebNavigationType.FormSubmitted:
				WebView.DecideIgnore (e.DecisionToken);
				break;
			case WebNavigationType.LinkClicked:
				NSWorkspace.SharedWorkspace.OpenUrl (e.OriginalUrl);
				WebView.DecideIgnore (e.DecisionToken);
				break;
			case WebNavigationType.Other:
			case WebNavigationType.Reload:
				WebView.DecideUse (e.DecisionToken);
				break;
			}
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