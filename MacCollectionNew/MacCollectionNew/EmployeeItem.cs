using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacCollectionNew
{
	public partial class EmployeeItem : NSView
	{
		
		#region Constructors
		// Called when created from unmanaged code
		public EmployeeItem(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public EmployeeItem(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		// Shared initialization code
		void Initialize()
		{
		}
		#endregion
	}
}
