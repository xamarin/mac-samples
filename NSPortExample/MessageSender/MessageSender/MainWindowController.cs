using System;

using Foundation;
using AppKit;
using System.Runtime.InteropServices;

namespace MessageSender
{
	public partial class MainWindowController : NSWindowController
	{
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
			msgPort = CFMessagePortCreateRemote (IntPtr.Zero, PortName.Handle);
			if (msgPort == IntPtr.Zero) {
				NSAlert.WithMessage ("Unable to connect to port? Did you launch server first?", "OK", "", "", "").RunModal ();
				NSApplication.SharedApplication.Terminate (this);
			}
			TheButton.Activated += SendMessage;
		}

		IntPtr msgPort;
		static NSString PortName = (NSString)"com.example.app.port.server";

		[DllImport ("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		extern static IntPtr CFMessagePortCreateRemote(IntPtr allocator, IntPtr name);

		[DllImport ("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		extern static int CFMessagePortSendRequest(IntPtr remote, int msgid, IntPtr data, double sendTimeout, double rcvTimeout, IntPtr replyMode, IntPtr returnData);

		void SendMessage (object sender, EventArgs e)
		{
			using (NSData data = NSData.FromString (TextField.StringValue))
			{
				CFMessagePortSendRequest (msgPort, 0x111, data.Handle, 10.0, 10.0, IntPtr.Zero, IntPtr.Zero);
			}
		}

		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}
	}
}
