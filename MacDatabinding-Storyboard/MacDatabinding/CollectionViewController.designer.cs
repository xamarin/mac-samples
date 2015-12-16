// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacDatabinding
{
	[Register ("CollectionViewController")]
	partial class CollectionViewController
	{
		[Outlet]
		AppKit.NSArrayController PeopleArray { get; set; }

		[Outlet]
		AppKit.NSCollectionView PeopleCollection { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (PeopleCollection != null) {
				PeopleCollection.Dispose ();
				PeopleCollection = null;
			}

			if (PeopleArray != null) {
				PeopleArray.Dispose ();
				PeopleArray = null;
			}
		}
	}
}
