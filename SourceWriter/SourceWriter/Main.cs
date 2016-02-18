using AppKit;

namespace SourceWriter
{
	/// <summary>
	/// The main starting point for the application.
	/// </summary>
	/// <remarks>For more details, see:https://developer.xamarin.com/guides/mac/getting_started/hello,_mac/#Main.cs</remarks>
	static class MainClass
	{
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		static void Main (string[] args)
		{
			NSApplication.Init ();
			NSApplication.Main (args);
		}
	}
}
