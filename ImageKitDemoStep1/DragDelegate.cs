using System;
using System.Linq;
using MonoMac.ObjCRuntime;
using MonoMac.AppKit;
using MonoMac.ImageKit;
using MonoMac.Foundation;

namespace ImageKitDemo
{
	public class DragDelegate: NSDraggingDestination
	{
		public DragDelegate (IKImageBrowserView view)
		{
			browserView = view;
		}
		IKImageBrowserView browserView;

		public override NSDragOperation DraggingEntered (NSDraggingInfo sender)
		{
			Console.WriteLine ("Drag Delegate received 'DraggingEntered'");
			return DraggingUpdated (sender);
		}

		public override NSDragOperation DraggingUpdated (NSDraggingInfo sender)
		{
			//The ImageBrowserView uses this method to set the icon during dragging. Regardless
			//of what we return the view will send a moveitems message to the datasource.
			//so it is best to not display the copy icon.

			//Console.WriteLine ("Drag Delegate received 'DraggingUpdated'");
			NSObject obj = GetSource(sender);
			if (obj != null && obj.Equals (browserView))
			{
				return NSDragOperation.Move;
			}
			return NSDragOperation.Copy;
		}

		public override bool PerformDragOperation (NSDraggingInfo sender)
		{
			Console.WriteLine ("Drag Delegate received 'PerformDragOperation' sender: {0}", sender);

			//It seems that browserView does not send this message when it is an internal move.
			//It does all the work by sending a moveitems message to the datasource,
			// but I return false here just to be safe.
			NSObject obj = GetSource (sender);
			if (obj != null && obj.Equals (browserView))
			{
				Console.WriteLine ("\tLet the image browser handle it.");
				return false;
			}
			//I'm not sure this is the best way to get data from the pasteboard, but it works
			//for me today.
			NSPasteboard pb = GetPasteboard (sender);
			NSArray data = null;
//			if (pb.Types.Contains (NSPasteboard.NSUrlType))
//				data = pb.GetPropertyListForType (NSPasteboard.NSUrlType) as NSArray;
			if (pb.Types.Contains (NSPasteboard.NSFilenamesType))
				data = pb.GetPropertyListForType (NSPasteboard.NSFilenamesType) as NSArray;
			if (data != null)
			{
				//Console.WriteLine ("Got Data");
				for (int i = 0; i < data.Count; i++) {
					string path = (string)NSString.FromHandle (data.ValueAt ((uint)i));
					Console.WriteLine ("From pasteboard Item {0} = {1}", i, path);
					((BrowseData)browserView.DataSource).AddImages (
						NSUrl.FromFilename (path), browserView.GetIndexAtLocationOfDroppedItem ());
					browserView.ReloadData();
				}
			}
			return true;
		}


		#region communicating with sender
		//calling NSDraggingInfo methods on the sender crashes the app, so we need to use the low level system.
		//Thanks to http://mono.1490590.n4.nabble.com/MonoMac-Drag-and-Drop-tp2533506p2539358.html

		static IntPtr selDraggingPasteboard = Selector.GetHandle ("draggingPasteboard");
		static IntPtr selDraggingSouce = Selector.GetHandle ("draggingSource");

		private NSPasteboard GetPasteboard (NSDraggingInfo sender)
		{
			return (NSPasteboard) Runtime.GetNSObject (Messaging.IntPtr_objc_msgSend (sender.Handle, selDraggingPasteboard));
		}

		private NSObject GetSource(NSDraggingInfo sender)
		{
			return Runtime.GetNSObject (Messaging.IntPtr_objc_msgSend (sender.Handle, selDraggingSouce));
		}

		#endregion


		#region implemented only for testing
		public override bool PrepareForDragOperation (NSDraggingInfo sender)
		{
			Console.WriteLine ("Drag Delegate received 'PrepareForDragOperation'");
			return true;
		}

		public override void ConcludeDragOperation (NSDraggingInfo sender)
		{
			Console.WriteLine ("Drag Delegate received 'ConcludeDragOperation'");
		}

		public override void DraggingExited (NSDraggingInfo sender)
		{
			Console.WriteLine ("Drag Delegate received 'DraggingExited'");
		}

		public override void DraggingEnded (NSDraggingInfo sender)
		{
			Console.WriteLine ("Drag Delegate received 'DraggingEnded'");
		}

		public override bool WantsPeriodicDraggingUpdates {
			get {
				Console.WriteLine ("Drag Delegate was queried for 'WantsPeriodicDraggingUpdates'");
				return true;
			}
		}
		#endregion
	}
}

