/* 
 * This sample is based on the "Skinnable App" from
 * http://mattgemmell.com/2008/02/24/skinnable-cocoa-ui-with-webkit-and-css
 * See http://mattgemmell.com/license for license on the original code.
 *  
 * Ported by Maxi Combina <maxi.combina@passwordbank.com> or <maxi.combina@gmail.com>
 * 
 * -----------------------------------------------------------------------------------
 * 
 * This is a simple Mono/MonoMac application showing how to use an embedded
 * MonoMac.WebKit.WebView to "skin" you application using standard loadable 
 * CSS files, and how to exchange data between your C# code and the HTML document.
 * 
 * The project shows you:
 * - How to switch CSS themes dynamically
 * - how to add content into the WebView from C#
 * - how to retrieve data from inside the HTML document
 * - how to replace existing content in the HTML document
 * - how to detect clicks in an HTML button end execute C# code.
 * 
 * The last line differs from the original Skinnable App. The original reads:
 * - how to allow HTML controls (like form elements, or links) to call methods in Cocoa objects
 * 
 * Due to the lack of bindings at the moment of the writing of this sample, this project shows 
 * a different feature. Once the missing APIs are in place, it should be easy to implenent based
 * on the Objective-C code from Skinnable App. 
*/ 

using System;
using System.IO;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.WebKit;

namespace SkinnableApp
{
	public class ShowMessageClickListener : DomEventListener {
		
		// The user just clicked the Show Message NSButton, so we show him/her
		// a greeting. This code shows you how to execute C# code when a click is done in
		// and HTML button.
		public override void HandleEvent (DomEvent evt)
		{
			var alert = new NSAlert () {
				MessageText = "Hello there",
				InformativeText = "Saying hello from C# code. Event type: " + evt.Type
			};
			alert.RunModal();
		}
	}
	
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		public SkinnableApp.ShowMessageClickListener clickListener = new ShowMessageClickListener ();
		
		public MainWindowController (IntPtr handle) : base(handle)
		{
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base(coder)
		{
		}

		// Call to load from the XIB/NIB file
		public MainWindowController () : base("MainWindow")
		{
		}
		
		/*
		 * Auto generated coded starts here. Let's have some fun!
		 */
		public override void AwakeFromNib()
		{
			// Set up delegates for the relevant events
			webView.FinishedLoad += delegate {
				loadTitle(); 
				installClickHandlers();
			};
			webView.UIGetContextMenuItems = (sender, forElements, defaultItems) => { 
				// Disable contextual (right click) menu for the webView
				return null; 
			};
			webView.DrawsBackground = false;
			
			// Load the HTML document
			var htmlPath = Path.Combine (NSBundle.MainBundle.ResourcePath, "index.html");
			webView.MainFrame.LoadRequest(new NSUrlRequest (new NSUrl (htmlPath)));
			
			// Setup the theme chooser
	    	themeChooser.RemoveAllItems ();
			DirectoryInfo resourceDir = new DirectoryInfo (NSBundle.MainBundle.ResourcePath);
			FileInfo[] cssFiles = resourceDir.GetFiles("*.css");
			
			Array.Sort (cssFiles, delegate (FileInfo f1, FileInfo f2) {
				// Sort by name, GetFiles does not seem to use naming order.
				return f1.Name.CompareTo(f2.Name); 
			});
			
			foreach (var cssFile in cssFiles){
				var themeName = cssFile.Name.Substring(0, cssFile.Name.IndexOf(".css"));
				
				var nsItem = new NSMenuItem (themeName, "", delegate { changeTheme (null); }) {
					RepresentedObject = new NSString(cssFile.Name)
				};
				
				if (themeName == "Default")
					nsItem.State = NSCellStateValue.On;
				themeChooser.Menu.AddItem(nsItem);
			}
			
			themeChooser.SelectItem("Default");
			
		}
		
		// The user just clicked the Add Content NSButton, so we'll add a new P tag
		// and a new I tag to the HTML, with some default content.
		// This shows you how to add content into an HTML document without reloading the page.
		partial void addContent (MonoMac.AppKit.NSButton sender)
		{
			var document = webView.MainFrameDocument;
			var paraBlock = document.GetElementById("main_content");
	
			var newPara = document.CreateElement("p");
			var newItal = document.CreateElement("i");
			var newText = document.CreateTextNode("Some new italic content");

			newPara.AppendChild (newItal);
			newItal.AppendChild (newText);
			paraBlock.AppendChild (newPara);

			
			// This is a different way to change the whole text.
			// Useful for replacing the ".InnerText = some string" used by Internet Explorer engine
			//paraBlock.TextContent = "Text replaced from Mono";

		}
		
		// The user clicked the Set Title button, so we'll take whatever text is in 
		// the titleText NSTextField and replace the current content of the 'contentTitle' 
		// H1 tag in the HTML with the new text. This shows you how to replace some HTML 
		// content with new content.
		partial void setTitle (MonoMac.AppKit.NSButton sender)
		{
			var document = webView.MainFrame.DomDocument;
			DomText newText = document.CreateTextNode(titleText.StringValue);
			
			var contentTitle = document.GetElementById("contentTitle");
			contentTitle.ReplaceChild(newText, contentTitle.FirstChild);
		}
		
		// The user just chose a theme in the NSPopUpButton, so we replace the HTML
		// document's CSS file using JavaScript.
		partial void changeTheme (MonoMac.AppKit.NSPopUpButton sender)
		{
			WebScriptObject scriptObject = webView.WindowScriptObject;
			NSString theme = (NSString) themeChooser.SelectedItem.RepresentedObject;
			scriptObject.EvaluateWebScript("document.getElementById('ss').href = '" + theme.ToString () + "'");	
		}
		
		
		// Grab the 'contentTitle' H1 tag, and put it in the titleText NSTextField
		// This code shows you how to get a value from the HTML document
		public void loadTitle ()
		{
			var document = webView.MainFrame.DomDocument;
			var contentTitle = document.GetElementById("contentTitle");
			titleText.StringValue = contentTitle.FirstChild.Value;
		}
		
		// Install the click handle for the Show Message HTML button
		public void installClickHandlers()
		{
			var dom = webView.MainFrameDocument;
			var element = dom.GetElementById ("message_button");
			element.AddEventListener ("click", clickListener, true);
		}
	}
}

