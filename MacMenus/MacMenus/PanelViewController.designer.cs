// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacMenus
{
	[Register ("PanelViewController")]
	partial class PanelViewController
	{
		[Outlet]
		AppKit.NSTextField propertyLabel { get; set; }

		[Action ("propertyDocument:")]
		partial void propertyDocument (Foundation.NSObject sender);

		[Action ("propertyFont:")]
		partial void propertyFont (Foundation.NSObject sender);

		[Action ("propertyText:")]
		partial void propertyText (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (propertyLabel != null) {
				propertyLabel.Dispose ();
				propertyLabel = null;
			}
		}
	}
}
