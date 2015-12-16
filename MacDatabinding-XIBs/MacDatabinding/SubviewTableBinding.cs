using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacDatabinding
{
	public partial class SubviewTableBinding : AppKit.NSView
	{
		#region Computed Properties
		public nint SelectedRow {
			get { return PeopleTable.SelectedRow; }
		}

		public NSTableView Table {
			get { return PeopleTable; }
		}
		#endregion

		#region Constructors
		// Called when created from unmanaged code
		public SubviewTableBinding (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewTableBinding (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

		}
		#endregion
	}
}
