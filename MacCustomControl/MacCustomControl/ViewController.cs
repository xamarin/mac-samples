using System;

using AppKit;
using Foundation;

namespace MacCustomControl
{
	public partial class ViewController : NSViewController
	{
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
			OptionTwo.ValueChanged += (sender, e) => {
				// Display the state of the option switch
				Console.WriteLine("Option Two: {0}", OptionTwo.Value);
			};
		}
		#endregion

		#region Actions
		partial void OptionOneFlipped (Foundation.NSObject sender) {
			// Display the state of the option switch
			Console.WriteLine("Option One: {0}", OptionOne.Value);
		}
		#endregion
	}
}
