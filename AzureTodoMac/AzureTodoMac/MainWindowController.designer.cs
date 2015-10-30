// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace AzureTodo
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		AppKit.NSTableColumn descriptionColumn { get; set; }

		[Outlet]
		AppKit.NSTableColumn nameColumn { get; set; }

		[Outlet]
		AppKit.NSTextField newTodoItemName { get; set; }

		[Outlet]
		AppKit.NSTableView todoTable { get; set; }

		[Action ("addNewTodoItem:")]
		partial void AddNewTodoItem (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (descriptionColumn != null) {
				descriptionColumn.Dispose ();
				descriptionColumn = null;
			}

			if (nameColumn != null) {
				nameColumn.Dispose ();
				nameColumn = null;
			}

			if (todoTable != null) {
				todoTable.Dispose ();
				todoTable = null;
			}

			if (newTodoItemName != null) {
				newTodoItemName.Dispose ();
				newTodoItemName = null;
			}
		}
	}
}
