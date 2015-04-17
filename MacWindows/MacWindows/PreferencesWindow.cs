using System;

using Foundation;
using AppKit;
using CoreGraphics;

namespace MacWindows
{
	public partial class PreferencesWindow : NSPanel
	{
		#region Private Variables
		private NSViewController _subviewController = null;
		private NSView _subview = null;
		#endregion

		#region Constructors
		public PreferencesWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public PreferencesWindow (NSCoder coder) : base (coder)
		{
		}
		#endregion

		#region Private Methods
		private void ShowPanel(NSViewController controller) {

			// Is there a view already being displayed?
			if (_subview != null) {
				// Yes, remove it from the view
				_subview.RemoveFromSuperview ();

				// Release memory
				_subview = null;
				_subviewController = null;
			}

			// Save values
			_subviewController = controller;
			_subview = controller.View;

			// Define frame and display
			_subview.Frame = new CGRect (0, 0, panelContainer.Frame.Width, panelContainer.Frame.Height);
			panelContainer.AddSubview (_subview);
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Automatically select the first item
			mainToolbar.SelectedItemIdentifier = "global";
			ShowPanel (new preferenceGlobalController ());
		}
		#endregion

		#region Toolbar Handlers
		[Export ("preferencesProfile:")]
		void PreferencesProfile (NSObject sender) {
			mainToolbar.SelectedItemIdentifier = "profile";
			ShowPanel (new PreferencesProfileController ());
		}

		[Export ("preferencesGlobal:")]
		void PreferencesGlobal (NSObject sender) {
			mainToolbar.SelectedItemIdentifier = "global";
			ShowPanel (new preferenceGlobalController ());
		}

		[Export ("preferencesKeyboard:")]
		void PreferencesKeyboard (NSObject sender) {
			mainToolbar.SelectedItemIdentifier = "keyboard";
			ShowPanel (new PreferencesKeyboardController ());
		}

		[Export ("preferencesVIOP:")]
		void PreferencesVOIP (NSObject sender) {
			mainToolbar.SelectedItemIdentifier = "voip";
			ShowPanel (new PreferencesVOIPController ());
		}
		#endregion
	}
}
