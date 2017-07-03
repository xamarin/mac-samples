using System;
using Foundation;
using AppKit;
namespace MacCollectionNew
{
	/// <summary>
	/// Manual toolbar item allows the developer to controll when it is 
	/// enabled/disabled.
	/// </summary>
	[Register("ManualToolbarItem")]
	public class ManualToolbarItem : NSToolbarItem
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:MacCollectionNew.ManualToolbarItem"/> should be enabled.
		/// </summary>
		/// <value><c>true</c> if should be enabled; otherwise, <c>false</c>.</value>
		public bool ShouldBeEnabled { get; set; } = true;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacCollectionNew.ManualToolbarItem"/> class.
		/// </summary>
		public ManualToolbarItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacCollectionNew.ManualToolbarItem"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ManualToolbarItem(IntPtr handle) : base(handle)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacCollectionNew.ManualToolbarItem"/> class.
		/// </summary>
		/// <param name="t">T.</param>
		public ManualToolbarItem(NSObjectFlag t) : base(t)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacCollectionNew.ManualToolbarItem"/> class.
		/// </summary>
		/// <param name="title">Title.</param>
		public ManualToolbarItem(string title) : base(title)
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Validate this instance.
		/// </summary>
		public override void Validate()
		{
			base.Validate();

			// Should this toolbar item be enabled?
			Enabled = ShouldBeEnabled;
		}
		#endregion
	}
}
