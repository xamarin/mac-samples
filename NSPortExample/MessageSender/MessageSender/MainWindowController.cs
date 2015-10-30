using System;

using AppKit;
using CoreFoundation;
using Foundation;

namespace MessageSender
{
	public partial class MainWindowController : NSWindowController
	{
		CFMessagePort msgPort;

		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}

		public MainWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}

		public MainWindowController () : base ("MainWindow")
		{
		}

		public override void AwakeFromNib ()
		{
			msgPort = CFMessagePort.CreateRemotePort (CFAllocator.Default, "com.example.app.port.server");
			if (msgPort == null) {
				var alert = new NSAlert {
					MessageText = "Unable to connect to port? Did you launch server first?",
				};
				alert.AddButton ("OK");
				alert.RunSheetModal (Window);
			}
			TheButton.Activated += SendMessage;
		}

		void SendMessage (object sender, EventArgs e)
		{
			using (var data = NSData.FromString (TextField.StringValue)) {
				NSData responseData;
				msgPort.SendRequest (0x111, data, 10.0, 10.0, (NSString)string.Empty, out responseData);
			}
		}
	}
}
