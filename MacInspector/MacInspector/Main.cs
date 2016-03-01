using AppKit;

namespace MacInspector
{
	/// <summary>
	/// Contains a static <c>Main</c> method which creates a new Xamarin.Mac application 
	/// instance and passes the name of the class that will handle OS events, 
	/// which in our case is the <see cref="T:MacInspector.AppDelegate"/> class.
	/// </summary>
	/// <remarks>
	/// Please see:
	/// https://developer.xamarin.com/guides/mac/getting_started/hello,_mac/#Main.cs
	/// </remarks>
	static class MainClass
	{
		static void Main (string [] args)
		{
			NSApplication.Init ();
			NSApplication.Main (args);
		}
	}
}
