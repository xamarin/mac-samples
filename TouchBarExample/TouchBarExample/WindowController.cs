using System;
using System.Linq;

using Foundation;
using AppKit;

namespace TouchBarExample
{
	public abstract class TouchBarExampleDelegate : NSTouchBarDelegate
	{
		public abstract int Count { get; }
		public virtual bool AllowCustomization { get; } = false;

		internal static int ParseId (string identifier)
		{
			return int.Parse (identifier.Replace ("com.xamarin.example.", ""));
		}

		internal static string CreateID (int number)
		{
			return string.Format ("com.xamarin.example.{0}", number);
		}
	}

	public abstract class TouchBarExampleWithPopover : TouchBarExampleDelegate
	{
		internal int ParseNestedId (string identifier)
		{
			return int.Parse (identifier.Replace ("com.xamarin.example.popup.", ""));
		}

		internal static string CreateNestedID (int number)
		{
			return string.Format ("com.xamarin.example.popup.{0}", number);
		}

		internal bool IsPopupID (string identifier)
		{
			return identifier.Contains ("popup");
		}
	}
	
	public partial class WindowController : NSWindowController, INSTabViewDelegate, INSTouchBarDelegate
	{
		TouchBarExampleDelegate[] examples  = {
			new CustomTouchBarDelegate (),
			new CanidateBarDelegate (),
			new ColorPickerDelegate (),
			new PopoverDelegate (),
			new SliderDelegate (),
			new SharingDelegate (),
			new ScrubberDelegate (),
			new CustomizeBarDelegate ()
		};

		NSTabView Tab => (NSTabView)Window.ContentView.Subviews [0];

		public WindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public WindowController (NSCoder coder) : base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			NSApplication.SharedApplication.SetAutomaticCustomizeTouchBarMenuItemEnabled (true);
			Tab.Delegate = this;
		}

		[Export ("tabView:didSelectTabViewItem:")]
		public void DidSelect (NSTabView tabView, NSTabViewItem item)
		{
			// Invalidate touch bar so we get a new one
			this.SetTouchBar (null);
		}

		[Export ("makeTouchBar")]
		public NSTouchBar MakeTouchBar ()
		{
			int index = (int)Tab.IndexOf (Tab.Selected);
			TouchBarExampleDelegate current = examples [index];

			var bar = new NSTouchBar ()
			{
				Delegate = current,
			};

			if (current.AllowCustomization) {
				var idList = GenerateCurrentIDList (current);
				bar.CustomizationIdentifier = "com.xamarin.example.customBar";
				bar.DefaultItemIdentifiers = idList.Take (2).ToArray ();
				bar.CustomizationAllowedItemIdentifiers = idList;
			}
			else {
				bar.DefaultItemIdentifiers = GenerateCurrentIDList (current);
			}

			return bar;
		}

		static string [] GenerateCurrentIDList (TouchBarExampleDelegate current)
		{
			int count = current.Count;
			string [] ids = new string [count];
			for (int i = 0; i < count; ++i)
				ids [i] = TouchBarExampleDelegate.CreateID (i);
			return ids;
		}
	}
}
