using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacDatabinding
{
	public partial class SubviewCollectionView : AppKit.NSView
	{
		#region Computed Properties
		public nint SelectionIndex {
			get { return (nint)PeopleArray.SelectionIndex; }
			set { PeopleArray.SelectionIndex = (ulong)value; }
		}
		#endregion

		#region Constructors
		// Called when created from unmanaged code
		public SubviewCollectionView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewCollectionView (NSCoder coder) : base (coder)
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

			// Watch for the selection value changing
			PeopleArray.AddObserver ("selectionIndexes", NSKeyValueObservingOptions.New, (sender) => {
				// Inform caller of selection change
				RaisePersonSelected((nint)PeopleArray.SelectionIndex);
			});
		}
		#endregion

		#region Events
		public delegate void PersonSelectedDelegate(nint index);
		public event PersonSelectedDelegate PersonSelected;

		internal void RaisePersonSelected(nint index) {
			if (this.PersonSelected != null)
				this.PersonSelected (index);
		}
		#endregion
	}
}
