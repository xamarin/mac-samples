using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ImageKit;

namespace ImageKitDemo
{
	public class BrowseData : IKImageBrowserDataSource 
	{
		public BrowseData ()
		{
		}
		
		List<BrowseItem> images = new List<BrowseItem>();

		# region Required IKImageBrowserDataSource methods
		public override int ItemCount (IKImageBrowserView aBrowser)
		{
			//Console.WriteLine ("DataSource: Image Count was requested");
			return images.Count;
		}
		
		public override IKImageBrowserItem GetItem (IKImageBrowserView aBrowser, int index)
		{
			//Console.WriteLine ("DataSource: Returning image at {0}, named {1}", index, images[index].ImageTitle);
			return images[index];
		}
		#endregion
		
		#region optional IKImageBrowserDataSource methods
		
		public override bool MoveItems (IKImageBrowserView aBrowser, NSIndexSet indexes, int destinationIndex)
		{
			//indexes are not sequential, and may be on both sides of destinationIndex.
			//indexes will change, but I will put the items in after the item at destination
			//FIXME - missing methods on NSIndexSet
			//FIXME make an extension method on List<>
			int destination = destinationIndex - indexes.Where (x => x < destinationIndex).Count ();
			List<BrowseItem> movingImages = new List<BrowseItem> ();
			foreach (int index in indexes)
				movingImages.Add (images[index]);
			foreach (BrowseItem item in movingImages)
				images.Remove (item);
			images.InsertRange (destination, movingImages);
			aBrowser.ReloadData();
			return true;
		}
		
		public override void RemoveItems (IKImageBrowserView aBrowser, NSIndexSet indexes)
		{
			//FIXME - add extension for List<T>
			//images.RemoveAt(indexes)
			List<BrowseItem> movingImages = new List<BrowseItem> ();
			foreach (int index in indexes)
				movingImages.Add (images[index]);
			foreach (BrowseItem item in movingImages)
				images.Remove (item);
			aBrowser.ReloadData();
		}
		#endregion
		
		#region Searching/filtering
		public void SetFilter (string searchText)
		{
			// This simple filtering solution is simple but flawed.
			// The user can use the UI to add, remove and reorganize the image list while it is filtered.
			// However these UI changes are lost when the filter is changed.
			// Since a more robust solution potentially using Linq, and a visible attribute on the
			// browse item would not enhance the demo of ImageKit, I will leave it as an exercise
			// for the reader.
			
			if (unfilteredImages == null)
				unfilteredImages = images.ToList ();
			
			if (string.IsNullOrEmpty (searchText))
				images = unfilteredImages.ToList ();
			else
				images = unfilteredImages.Where (i => i.ImageTitle.Contains (searchText)).ToList ();
		}
		List<BrowseItem> unfilteredImages;
		#endregion

		public void AddImages (NSUrl path)
		{
			AddImages (path, -1);
		}

		public void AddImages (NSUrl uri, int index)
		{
			if (uri.IsFileUrl)
			{
				string path = uri.Path;
				if (Directory.Exists (path))
				{
					foreach (var file in Directory.GetFiles (path))
					{
						AddImageFile (file, index);
					}
				}
				if (File.Exists (path))
				{
					AddImageFile (path, index);
				}
			} else {
				images.Add (new BrowseItem (uri));
			}
		}

		private void AddImageFile (string path, int index)
		{
			string name = Path.GetFileNameWithoutExtension (path);
			//Skip .* files
			if (name.IndexOf ('.') != 0)
			{
				if (-1 < index && index < images.Count)
					images.Insert (index, new BrowseItem (NSUrl.FromFilename (path)));
				else
					images.Add (new BrowseItem (NSUrl.FromFilename (path)));
			}
		}

	}
}

