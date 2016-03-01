using AppKit;
using Foundation;

namespace MacInspector
{
	/// <summary>
	/// This class handles events that occur for the app.
	/// </summary>
	/// <remarks>
	/// Please see:
	/// https://developer.xamarin.com/guides/mac/getting_started/hello,_mac/#AppDelegate.cs
	/// </remarks>
	public partial class AppDelegate : NSApplicationDelegate
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.AppDelegate"/> class.
		/// </summary>
		public AppDelegate ()
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Did the app finish launching.
		/// </summary>
		/// <returns>The finish launching.</returns>
		/// <param name="notification">Notification.</param>
		public override void DidFinishLaunching (NSNotification notification)
		{
			// Insert code here to initialize your application
		}

		/// <summary>
		/// Will the app terminate.
		/// </summary>
		/// <returns>The terminate.</returns>
		/// <param name="notification">Notification.</param>
		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}
		#endregion
	}
}

