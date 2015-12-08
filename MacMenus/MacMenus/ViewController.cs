using System;

using AppKit;
using Foundation;

namespace MacMenus
{
	public partial class ViewController : NSViewController
	{
		#region Application Access
		public static AppDelegate App {
			get { return (AppDelegate)NSApplication.SharedApplication.Delegate; }
		}
		#endregion

		#region Computed Properties
		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}

		public string Text {
			get { return documentText.Value; }
			set { documentText.Value = value; }
		} 
		#endregion

		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any additional setup after loading the view.
		}

		public override void ViewWillAppear ()
		{
			base.ViewWillAppear ();

			App.textEditor = this;
		}

		public override void ViewWillDisappear ()
		{
			base.ViewDidDisappear ();

			App.textEditor = null;
		}
		#endregion
	}
}
