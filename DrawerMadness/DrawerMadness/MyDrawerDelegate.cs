using System;
using System.Drawing;
using MonoMac.AppKit;

namespace DrawerMadness
{
	/*
	 * Our Custom Drawer Delegate class which is used by the upperRightDrawer 
	 * 
	 */
	public class MyDrawerDelegate : NSDrawerDelegate
	{
		
		private ParentWindowController controller;
		public MyDrawerDelegate(ParentWindowController controller):base() 
		{
			this.controller = controller;
		}
		
//		public override void DrawerDidOpen(NSNotification notification) 
//		{
//			Console.WriteLine("Drawer Did Open");
//		}
//		
//		
//		public override void DrawerWillOpen (NSNotification notification)
//		{
//			Console.WriteLine("Drawer Will Open");
//		}
//		
//		public override bool DrawerShouldOpen (NSDrawer sender)
//		{
//			Console.WriteLine("Drawer Should Open");
//			return true;
//		}
//		
//		public override void DrawerDidClose (NSNotification notification)
//		{
//			Console.WriteLine("Drawer Did Close");
//		}
//		
		public override bool DrawerShouldClose (NSDrawer sender)
		{
			return controller.AllowUpperRightDrawerToClose;
			
		}
		
//		public override void DrawerWillClose (NSNotification notification)
//		{
//			Console.WriteLine("Drawer Will Close");
//		}
		
		public override SizeF DrawerWillResizeContents (NSDrawer sender, SizeF contentSize)
		{
			Console.WriteLine("Drawer Resize from MyDrawerDelegate");
			contentSize.Width = 10 * (float)Math.Ceiling(contentSize.Width / 10);
			if (contentSize.Width < 50) 
				contentSize.Width = 50;
			if (contentSize.Width > 250) 
				contentSize.Width = 250;
			if (sender == controller.upperRightDrawer) {
				controller.lowerRightDrawer.ContentSize = new SizeF(300 - contentSize.Width, 
				                                                    controller.lowerRightDrawer.ContentSize.Height);
			} 
			else if (sender == controller.lowerRightDrawer) {
				controller.upperRightDrawer.ContentSize = new SizeF(300 - contentSize.Width, 
				                                                    controller.upperRightDrawer.ContentSize.Height);
				
			}
			
			return contentSize;
		}
	}
}

