using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AppKit;
using Foundation;

namespace MonoMac.CFNetwork.Test.Views
{
	[Register("HeaderTableDataSource")]
	public class HeaderTableDataSource : NSTableViewDataSource 
	{
		KeyValuePair<string,string>[] data;

		public void Clear ()
		{
			data = null;
		}

		public void SetHeaders (HttpResponseMessage response)
		{
			var list = new List<KeyValuePair<string,string>> ();
			foreach (var header in response.Content.Headers) {
				var value = header.Value.First ();
				list.Add (new KeyValuePair<string, string> (header.Key, value));
			}
			list.Add (new KeyValuePair<string, string> (string.Empty, string.Empty));
			foreach (var header in response.Headers) {
				var value = header.Value.First ();
				list.Add (new KeyValuePair<string, string> (header.Key, value));
			}
			data = list.ToArray ();
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			if (data == null)
				return 0;
			else
				return data.Length;
		}

		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn col, nint row)
		{
			var entry = data [row];
			string value = entry.Value;
			if (col.DataCell.Tag == 0)
				return new NSString (entry.Key);
			else
				return new NSString (entry.Value);
		}
	}
}

