using System;

using Foundation;
using AppKit;

namespace MacOutlines
{
	public partial class RotationWindow : NSWindow
	{
		#region Constructors
		public RotationWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public RotationWindow (NSCoder coder) : base (coder)
		{
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

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
