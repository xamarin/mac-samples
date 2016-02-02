using System;
using System.IO;

using AppKit;
using Foundation;

namespace HttpClient {
	public partial class ViewController : NSViewController, INSTableViewDataSource {
		readonly string [] values = {
			"http - WebRequest",
			"https - WebRequest",
			"http - NSUrlConnection",
			"http - HttpClient",
			"https - HttpClient"
		};

		public string WisdomUrl { get; } = "http://httpbin.org/ip";

		public Type HandlerType { get; set; }

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			TheTable.DataSource = this;
			TheTable.SelectRow (0, false);
			TheTable.HeaderView = null;
		}

		async partial void OnPress (NSObject sender)
		{
			HandlerType = null;
			TheButton.Enabled = false;
			switch (TheTable.SelectedRow) {
			case 0:
				new DotNet (this).HttpSample ();
				break;
			case 1:
				new DotNet (this).HttpSecureSample ();
				break;
			case 2:
				new Cocoa (this).HttpSample ();
				break;
			case 3:
				await new NetHttp (this).HttpSample (false);
				break;
			case 4:
				await new NetHttp (this).HttpSample (true);
				break;
			}
		}

		public void RenderStream (Stream stream)
		{
			var reader = new StreamReader (stream);
			InvokeOnMainThread (() => {
				string clientType = HandlerType != null ? $"HttpClient is using {HandlerType.Name}\n" : string.Empty;
				TheLog.Value = $"{clientType} The HTML returned by the server: {reader.ReadToEnd ()}";
				TheButton.Enabled = true;
			});
		}

		public void RenderError (string s)
		{
			InvokeOnMainThread (() => {
				TheLog.Value = $"Error: {s}";
				TheButton.Enabled = true;
			});
		}

		[Export ("numberOfRowsInTableView:")]
		public nint GetRowCount (NSTableView tableView)
		{
			return values.Length;
		}

		[Export ("tableView:objectValueForTableColumn:row:")]
		public NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			return (NSString)values[(int)row];
		}
	}

}
