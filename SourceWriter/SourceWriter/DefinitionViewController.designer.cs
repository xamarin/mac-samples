// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SourceWriter
{
	[Register ("DefinitionViewController")]
	partial class DefinitionViewController
	{
		[Outlet]
		AppKit.NSTextField DefinitionDescription { get; set; }

		[Outlet]
		AppKit.NSTextField DefinitionTitle { get; set; }

		[Action ("CloseDefinition:")]
		partial void CloseDefinition (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (DefinitionDescription != null) {
				DefinitionDescription.Dispose ();
				DefinitionDescription = null;
			}

			if (DefinitionTitle != null) {
				DefinitionTitle.Dispose ();
				DefinitionTitle = null;
			}
		}
	}
}
