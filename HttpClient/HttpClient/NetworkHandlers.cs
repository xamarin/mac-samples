using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Foundation;

namespace HttpClient {
	public class NetHttp {
		readonly ViewController viewController;

		public NetHttp (ViewController vc)
		{
			viewController = vc;
		}

		public async Task HttpSample (bool secure)
		{
			var client = new System.Net.Http.HttpClient ();
			viewController.HandlerType = typeof(HttpMessageInvoker).GetField ("handler", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue (client).GetType ();
			viewController.RenderStream (await client.GetStreamAsync (secure ? "https://www.xamarin.com" : viewController.WisdomUrl));
		}
	}

	public class DotNet {
		readonly ViewController viewController;

		public DotNet (ViewController vc)
		{
			viewController = vc;
		}

		//
		// Asynchronous HTTP request
		//
		public void HttpSample ()
		{
			var request = WebRequest.Create (viewController.WisdomUrl);
			request.BeginGetResponse (FeedDownloaded, request);
		}

		/// <summary>
		/// Asynchornous HTTPS request to gmail.com
		/// </summary>
		public void HttpSecureSample ()
		{
			var https = (HttpWebRequest) WebRequest.Create ("https://gmail.com");

			// To not depend on the root certficates, we will
			// accept any certificates:
			ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, ssl) =>  true;

			https.BeginGetResponse (GmailDownloaded, https);
		}

		void FeedDownloaded (IAsyncResult result)
		{
			var request = result.AsyncState as HttpWebRequest;

			try {
				var response = request.EndGetResponse (result);
				viewController.RenderStream (response.GetResponseStream ());
			} catch (Exception e) {
				viewController.RenderError ("Exception: " + e.Message);
			}
		}

		/// <summary>
		/// In this method viewController get the result from calling
		/// https://gmail.com, an HTTPS secure connection,
		/// we do not attempt to parse the output, but merely
		/// dump it as text
		/// </summary>
		void GmailDownloaded (IAsyncResult result)
		{
			var request = result.AsyncState as HttpWebRequest;

			try {
				var response = request.EndGetResponse (result);
				viewController.RenderStream (response.GetResponseStream ());
			} catch (Exception e) {
				viewController.RenderError ("Exception: " + e.Message);
			}
		}

		/// <summary>
		/// For an explanation of this AcceptingPolicy class, see http://mono-project.com/UsingTrustedRootsRespectfully
		/// </summary>
		class AcceptingPolicy : ICertificatePolicy {
			public bool CheckValidationResult (ServicePoint sp, X509Certificate cert, WebRequest req, int error)
			{
				// Trust everything
				return true;
			}
		}
	}

	public class Cocoa : NSUrlConnectionDataDelegate {
		readonly ViewController viewController;
		byte [] result;

		public Cocoa (ViewController vc)
		{
			viewController = vc;
			result = new byte [0];
		}

		public void HttpSample ()
		{
			var req = new NSUrlRequest (new NSUrl (viewController.WisdomUrl), NSUrlRequestCachePolicy.ReloadIgnoringCacheData, 10);
			NSUrlConnection.FromRequest (req, this);
		}

		// Collect all the data
		public override void ReceivedData (NSUrlConnection connection, NSData data)
		{
			byte [] nb = new byte [result.Length + (int)data.Length];
			result.CopyTo (nb, 0);
			Marshal.Copy (data.Bytes, nb, result.Length, (int) data.Length);
			result = nb;
		}

		public override void FinishedLoading (NSUrlConnection connection)
		{
			var ms = new MemoryStream (result);
			viewController.RenderStream (ms);
		}

		public override void FailedWithError (NSUrlConnection connection, NSError error)
		{
			viewController.RenderError (error.LocalizedFailureReason);
		}
	}
}

