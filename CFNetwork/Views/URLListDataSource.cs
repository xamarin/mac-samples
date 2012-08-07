using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MonoMac.CFNetwork.Test.Views {

	public class URLListDataSource : NSComboBoxDataSource {
		List<string> list;

		public URLListDataSource ()
		{
			list = new List<string> ();
			list.Add ("http://localhost/");
			list.Add ("http://xamarin.com/");
			list.Add ("http://localhost:8080/Protected/");
			list.Add ("http://www.mono-project.com/");
			list.Add ("http://www.google.com/ncr");
			list.Add ("http://www.heise.de/");
			list.Add ("http://wwf.panda.org/");
			// Sets Content-Language: lb
			list.Add ("http://lb.wikipedia.org/wiki/Haaptsäit");
			list.Add ("http://monodevelop.com/files/MacOSX/trunk-builds/MonoDevelop-master-20110920.dmg");
		}

		public void Add (string item)
		{
			if (!list.Contains (item))
				list.Add (item);
		}

		public override int ItemCount (NSComboBox comboBox)
		{
			return list.Count;
		}

		public override NSObject ObjectValueForItem (NSComboBox comboBox, int index)
		{
			return (NSString)list [index];
		}
	}
}

