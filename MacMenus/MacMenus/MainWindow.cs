using System;

using Foundation;
using AppKit;

namespace MacMenus
{
	public partial class MainWindow : NSWindow
	{
		#region Computed Properties
		public string Text {
			get { return documentText.Value; }
			set { documentText.Value = value; }
		}
		#endregion

		#region Constructors
		public MainWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}
		#endregion

		#region Menu Handlers
		[Export ("propertyDocument:")]
		void PropertyDocument (NSObject sender) {

			propertyLabel.StringValue = "Document";
		}

		[Export ("propertyFont:")]
		void PropertyFont (NSObject sender) {

			propertyLabel.StringValue = "Font";
		}

		[Export ("propertyText:")]
		void PropertyText (NSObject sender) {

			propertyLabel.StringValue = "Text";
		}
		#endregion
	}
}
