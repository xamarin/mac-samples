using System;
using AppKit;
using Foundation;

namespace AppKit
{
	[Register("SourceListView")]
	public class SourceListView : NSOutlineView
	{
		#region Computed Properties
		/// <summary>
		/// Gets the data.
		/// </summary>
		/// <value>The data.</value>
		public SourceListDataSource Data {
			get {return (SourceListDataSource)this.DataSource; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Rotation.OutlineViewController"/> class.
		/// </summary>
		public SourceListView ()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Rotation.OutlineViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public SourceListView (IntPtr handle) : base(handle)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Rotation.OutlineViewController"/> class.
		/// </summary>
		/// <param name="coder">Coder.</param>
		public SourceListView (NSCoder coder) : base(coder)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Rotation.OutlineViewController"/> class.
		/// </summary>
		/// <param name="t">T.</param>
		public SourceListView (NSObjectFlag t) : base(t)
		{

		}

		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Initialize this instance.
		/// </summary>
		public void Initialize() {

			// Initialize this instance
			this.DataSource = new SourceListDataSource (this);
			this.Delegate = new SourceListDelegate (this);

		}

		/// <summary>
		/// Adds the item.
		/// </summary>
		/// <param name="item">Item.</param>
		public void AddItem(SourceListItem item) {
			if (Data != null) {
				Data.Items.Add (item);
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Item selected delegate.
		/// </summary>
		public delegate void ItemSelectedDelegate(SourceListItem item);
		/// <summary>
		/// Occurs when item selected.
		/// </summary>
		public event ItemSelectedDelegate ItemSelected;

		/// <summary>
		/// Raises the item selected.
		/// </summary>
		/// <param name="item">Item.</param>
		internal void RaiseItemSelected(SourceListItem item) {
			// Inform caller
			if (this.ItemSelected != null) {
				this.ItemSelected (item);
			}
		}
		#endregion
	}
}

