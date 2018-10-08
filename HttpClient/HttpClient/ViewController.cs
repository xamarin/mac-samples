using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

using AppKit;
using Foundation;

namespace HttpClient {
	public partial class ViewController : NSViewController, INSTableViewDataSource {
		readonly string [] values = {
			"http - WebRequest",
			"https - WebRequest",
			"http - NSUrlConnection",
			"http - HttpClient",
			"https - HttpClient",
			"https - TLSv1.2"
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
			case 5:
				RunTls12Request ();
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

		void RunTls12Request ()
		{
			var actual = ServicePointManager.SecurityProtocol;

			try {
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

				var request = WebRequest.CreateHttp (new Uri ("https://howsmyssl.com:443/a/check"));
				ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
				var clientCertificate = new X509Certificate2 ("cert.pem");
				request.ClientCertificates.Add (clientCertificate);
				var msg = request.GetResponse ();

				using (var stream = msg.GetResponseStream ())
					RenderStream (stream);
			} catch (WebException ex) {
				Console.WriteLine (ex.Message);
			} finally {
				ServicePointManager.SecurityProtocol = actual;
			}
		}
	}
}
