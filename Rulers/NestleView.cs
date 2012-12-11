using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Rulers
{
	
	/// <summary>
	/// NestleView's job is to provide a buffer zone around the ruler's
	/// client, an instance of RectsView. I did this to test what happens
	/// to the coordinate mapping when the client isn't the scroll view's
	/// document view.
	/// </summary>
	public partial class NestleView : MonoMac.AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public NestleView (IntPtr handle) : base(handle)
		{
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public NestleView (NSCoder coder) : base(coder)
		{
		}
		
		[Export("initWithFrame:")]
		public NestleView (RectangleF frame) : base(frame)
		{
		}
		#endregion
		
		#region Class Overrides
		
		public override bool IsFlipped {
			get {
				return true;
			}
		}
		
		#endregion		
	}
}

