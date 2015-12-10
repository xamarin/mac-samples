using System;

using AppKit;
using Foundation;

namespace MacSourceLists
{
	public partial class ViewController : NSViewController
	{
		#region Computed Properties
		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
		#endregion

		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Populate source list
			SourceList.Initialize ();

			var library = new SourceListItem ("Library");
			library.AddItem ("Venues", "house.png", () => {
				Console.WriteLine("Venue Selected");
			});
			library.AddItem ("Singers", "group.png");
			library.AddItem ("Genre", "cards.png");
			library.AddItem ("Publishers", "box.png");
			library.AddItem ("Artist", "person.png");
			library.AddItem ("Music", "album.png");
			SourceList.AddItem (library);

			// Add Rotation 
			var rotation = new SourceListItem ("Rotation"); 
			rotation.AddItem ("View Rotation", "redo.png");
			SourceList.AddItem (rotation);

			// Add Kiosks
			var kiosks = new SourceListItem ("Kiosks");
			kiosks.AddItem ("Sign-in Station 1", "imac");
			kiosks.AddItem ("Sign-in Station 2", "ipad");
			SourceList.AddItem (kiosks);

			// Display side list
			SourceList.ReloadData ();
			SourceList.ExpandItem (null, true);
		}
		#endregion
	}
}
