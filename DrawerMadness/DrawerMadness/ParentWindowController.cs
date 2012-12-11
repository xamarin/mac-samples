
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace DrawerMadness
{
	public partial class ParentWindowController : MonoMac.AppKit.NSWindowController
		
	{
		
		NSDrawer bottomDrawer;
		internal NSDrawer upperRightDrawer;
		internal NSDrawer lowerRightDrawer;
		
		// Drawer Delegate
		// The upper right drawer will use a delegate class
		//  whereas the lower right drawer will use events
		MyDrawerDelegate myDrawerDelegate;
		
		#region Constructors

		// Called when created from unmanaged code
		public ParentWindowController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public ParentWindowController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public ParentWindowController () : base("ParentWindow")
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new ParentWindow Window {
			get { return (ParentWindow)base.Window; }
		}
		
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			setupLeftDrawer();
			setupBottomDrawer();
			setupUpperRightDrawer();
			setupLowerRightDrawer();
			
			setRightDrawerOffsets();
			setBottomDrawerOffsets();
			
			Window.DidResize += HandleWindowDidResize;
			
		}

		void HandleWindowDidResize (object sender, EventArgs e)
		{
			setBottomDrawerOffsets();
			setRightDrawerOffsets();
		}

		
		
		/****************** Left drawer ******************/
		/* Our left drawer is a simple drawer, created in IB, with nothing more than a minimum and maximum size. */
		private void setupLeftDrawer() 
		{
			leftDrawer.MinContentSize = new SizeF(100,100);
			leftDrawer.MaxContentSize = new SizeF(400,400);
		}
		
		
		partial void openLeftDrawer (NSObject sender)
		{
			leftDrawer.OpenOnEdge(NSRectEdge.MinXEdge);
		}
		
		partial void closeLeftDrawer (NSObject sender)
		{
			leftDrawer.Close(sender);
		}
		
		partial void toggleLeftDrawer (NSObject sender)
		{
			
			NSDrawerState state = leftDrawer.State;
		    if (state == NSDrawerState.Opening 
			    	|| state == NSDrawerState.Open) {
		        leftDrawer.Close(sender);
		    } else {
		        leftDrawer.OpenOnEdge(NSRectEdge.MinXEdge);
		    }
			
		}
		
		/****************** Bottom drawer ******************/
		/* Our bottom drawer is created programmatically rather than in IB, and has a 
		fixed size both vertically and horizontally.  The fixed vertical size is achieved
		by setting min and max content sizes equal to the content size.  The fixed horizontal
		size is achieved by setting leading and trailing offsets when the parent window resizes. */ 
		
		private void setupBottomDrawer() 
		{
			SizeF contentSize = new SizeF(100,100);
			bottomDrawer = new NSDrawer(contentSize,NSRectEdge.MinYEdge) {
				ParentWindow = myParentWindow,
				MinContentSize = contentSize,
				MaxContentSize = contentSize
			};
		}
		
		partial void openBottomDrawer (NSObject sender)
		{
			bottomDrawer.OpenOnEdge(NSRectEdge.MinYEdge);
		}
		
		partial void closeBottomDrawer (NSObject sender)
		{
			bottomDrawer.Close(sender);
		}
		
		partial void toggleBottomDrawer (NSObject sender)
		{
			var state = bottomDrawer.State;
			if (state == NSDrawerState.Opening || state == NSDrawerState.Open) 
				bottomDrawer.Close(sender);
			else
				bottomDrawer.OpenOnEdge(NSRectEdge.MinYEdge);
		}
		
		private void setBottomDrawerOffsets() 
		{
			SizeF frameSize = ((NSWindow)myParentWindow).Frame.Size;
			bottomDrawer.LeadingOffset = 50;
			// we want a bottomDrawer width of approximately 220 unscaled.  
			//	Figure out an offset to accomplish that size.
			float bottomDrawerWidth = 220 * myParentWindow.UserSpaceScaleFactor;
			bottomDrawer.TrailingOffset = frameSize.Width - bottomDrawerWidth - 50;
    
		}
		
		/****************** Upper right drawer ******************/

		/* Our two right drawers divide the right edge of the parent window between them. 
		In addition, they resize together horizontally, in such a way as to maintain a
		constant total width. 
		
		The upper right drawer will use our custom delegate class and the lower right drawer will events.
		*/
		
		private void setupUpperRightDrawer() {
			SizeF contentSize = new SizeF(150,150);
			upperRightDrawer = new NSDrawer(contentSize,NSRectEdge.MaxXEdge) {
				ParentWindow = myParentWindow,
				MinContentSize = contentSize
			};

			// setup delegate to recompute the sizes and control close
			myDrawerDelegate = new MyDrawerDelegate(this);
			upperRightDrawer.Delegate = myDrawerDelegate;
		}
		
			                                                                
		partial void openUpperRightDrawer (NSObject sender)
		{
			upperRightDrawer.OpenOnEdge(NSRectEdge.MaxXEdge);
		}
		
		partial void closeUpperRightDrawer (NSObject sender)
		{
			upperRightDrawer.Close(sender);
		}
		
		partial void toggleUpperRightDrawer (NSObject sender)
		{
			var state = upperRightDrawer.State;
			if (state == NSDrawerState.Opening || state == NSDrawerState.Open) 
				upperRightDrawer.Close(sender);
			else
				upperRightDrawer.OpenOnEdge(NSRectEdge.MaxXEdge);
		}
		
		/****************** Lower right drawer ******************/
		private void setupLowerRightDrawer()
		{
			SizeF contentSize = new SizeF(150,150);
			lowerRightDrawer = new NSDrawer(contentSize,NSRectEdge.MaxXEdge) {
				ParentWindow = myParentWindow,
				MinContentSize = new SizeF(50,50)
			};
			
			// Attach our delegate methods
			lowerRightDrawer.DrawerWillResizeContents = DrawerWillResizeContents;
			lowerRightDrawer.DrawerShouldClose = DrawerShouldClose;
		}
		
		partial void openLowerRightDrawer (NSObject sender)
		{
			lowerRightDrawer.OpenOnEdge(NSRectEdge.MaxXEdge);
		}
		
		partial void closeLowerRightDrawer (NSObject sender)
		{
			lowerRightDrawer.Close(sender);
		}
		
		partial void toggleLowerRightDrawer (NSObject sender)
		{
			var state = lowerRightDrawer.State;
			if (state == NSDrawerState.Opening || state == NSDrawerState.Open) 
				lowerRightDrawer.Close(sender);
			else
				lowerRightDrawer.OpenOnEdge(NSRectEdge.MaxXEdge);
		}
		
		private void setRightDrawerOffsets() 
		{
			SizeF frameSize = myParentWindow.Frame.Size;
			uint halfHeight = (uint)frameSize.Height / 2, remainder = (uint)frameSize.Height - 2 * halfHeight;
			upperRightDrawer.LeadingOffset = 50;
			upperRightDrawer.TrailingOffset = halfHeight;
			lowerRightDrawer.LeadingOffset = halfHeight;
			lowerRightDrawer.TrailingOffset = 50 + remainder;
		}
		
		
		#region Drawer Helper Delegate Methods
		
		private SizeF DrawerWillResizeContents (NSDrawer sender, SizeF contentSize)
		{
			Console.WriteLine("Drawer Resize");
			contentSize.Width = 10 * (float)Math.Ceiling(contentSize.Width / 10);
			if (contentSize.Width < 50) 
				contentSize.Width = 50;
			if (contentSize.Width > 250) 
				contentSize.Width = 250;
			if (sender == upperRightDrawer)
				lowerRightDrawer.ContentSize = new SizeF(300 - contentSize.Width, 
									 lowerRightDrawer.ContentSize.Height);
			else if (sender == lowerRightDrawer) 
				upperRightDrawer.ContentSize = new SizeF(300 - contentSize.Width, 
									 upperRightDrawer.ContentSize.Height);
			return contentSize;
			
		}
		
		private bool DrawerShouldClose (NSDrawer sender)
		{
			return (lowerRightAllowClose.State == NSCellStateValue.On);
		}
		
		#endregion
		
		#region Accessor Properties from controller
		
		internal bool AllowUpperRightDrawerToClose  {
			get {
				return (upperRightAllowClose.State == NSCellStateValue.On);
			}
		}
		
		#endregion
	}
}		


