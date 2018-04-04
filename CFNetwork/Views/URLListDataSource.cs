using System;
using System.Collections.Generic;
using AppKit;
using Foundation;

namespace MonoMac.CFNetwork.Test.Views
{
	public class URLListDataSource : NSComboBoxDataSource 
	{
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

		public override nint ItemCount (NSComboBox comboBox)
		{
			return list.Count;
		}

		public override NSObject ObjectValueForItem (NSComboBox comboBox, nint index)
		{
			return (NSString)list [(int)index];
		}
	}
}