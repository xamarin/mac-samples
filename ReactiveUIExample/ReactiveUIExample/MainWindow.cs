using System;

using AppKit;
using CoreGraphics;
using Foundation;
using ReactiveUI;

namespace ReactiveUIExample
{
	// To correctly add a reference to ReativeUI to Xamarin.Mac Unified projects, the following must be true:
	//    You are targetting the Unified Mobile Framework
	// Steps:
	//    - Right click packages, add package
	//    - Add reactiveui
	//    - Right click packages, update (Old version of Splat will cause compile errors otherwise)
	public class MainWindowViewModel : ReactiveObject
	{
		private string _Text;
		public string Text
		{
			get { return _Text; }
			set { this.RaiseAndSetIfChanged(ref _Text, value); }
		}

		public ReactiveCommand<object> Send { get; private set; }

		public MainWindowViewModel ()
		{
			// We can only send tweets if we have text less that 140 characters
			var canSend = this.WhenAny (vm => vm.Text, s => !String.IsNullOrEmpty (s.Value) && s.Value.Length < 140);
			Send = ReactiveCommand.Create (canSend);

			// What happens when they push the button
			Send.Subscribe (_ => Console.WriteLine ("Send Tweet: " + Text));
		}
	}

	public partial class MainWindow : NSWindow
	{
		public MainWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
		}

		MainWindowViewModel viewModel;
		public override void AwakeFromNib ()
		{
			viewModel = new MainWindowViewModel ();


			NSTextField textField = new NSTextField (new CGRect (20, Frame.Height-50, Frame.Width-40, 20));
			textField.AutoresizingMask = NSViewResizingMask.MinYMargin | NSViewResizingMask.WidthSizable;
			// Update the view model Text when the view changes
			textField.Changed += (o, e) => viewModel.Text = textField.StringValue;
			ContentView.AddSubview (textField);

			NSButton button = new NSButton (new CGRect (Frame.Width-120, Frame.Height-90, 100, 30));
			button.AutoresizingMask = NSViewResizingMask.MinYMargin | NSViewResizingMask.MinXMargin ;
			button.Title = "Send Tweet";
			// Keep button enable view state in sync with model
			viewModel.Send.CanExecuteObservable.Subscribe (x => button.Enabled = x);
			// Invoke command when button is pressed
			button.Activated += (o, e) => viewModel.Send.Execute (null);
			ContentView.AddSubview (button);
		}
	}
}
