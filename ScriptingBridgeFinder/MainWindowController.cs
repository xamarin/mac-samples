using System;
using System.Collections.Generic;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.ScriptingBridge;

namespace ScriptingBridgeFinder
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		
		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{

		}


		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			applicationsTable.Target = this;
			finder = SBApplication.FromBundleIdentifier ("com.apple.finder");
		}

		SBApplication finder { get; set; }
		
		internal void LoadApps ()
		{
			
			applicationsTable.Enabled = false;
			progressIndicator.StartAnimation (this);
			
			List<FinderFile> currentApps = new List<FinderFile> ();
			
			appTableContent.Remove (NSArray.FromArray<NSObject> ((NSArray)appTableContent.Content));
			//((NSMutableArray)appTableContent.Content).RemoveAll();

			SBElementArray folders = (SBElementArray)finder.ValueForKeyPath ((NSString)"startupDisk.folders");
			SBObject apps = (SBObject)folders.Object ("Applications");
			bool exists = apps.ValueForKey ((NSString)"exists").ToString () == "1" ? true : false;
			
			// if the applicatoins folder exists...
			if (exists) {
				// get the applications in the folder
				SBElementArray topLevelApps = (SBElementArray)apps.ValueForKey ((NSString)"applicationFiles");

				for (int x = 0; x <= topLevelApps.Count; x++) {
					
					if (((SBObject)topLevelApps.ObjectAt (NSValue.FromObject (x))).ValueForKey((NSString)"name") != null) {
						// add each of the applications to our list of applications
						currentApps.Add (new FinderFile ((SBObject)topLevelApps.ObjectAt (NSValue.FromObject (x))));
					}
				}

				
				SBObject apps2 = (SBObject)folders.Object ("Applications");
				SBElementArray appFolders = (SBElementArray)apps2.ValueForKey ((NSString)"folders");
				
				for (int x = 0; x <= appFolders.Count; x++) {
					// get the nth Folder
					SBObject nthFolder = (SBObject)appFolders.ObjectAt (NSValue.FromObject (x));
					// get the applications contained in the nth Folder
					SBElementArray secondLevelApps = (SBElementArray)nthFolder.ValueForKey ((NSString)"applicationFiles");

					for (int s = 0; s <= secondLevelApps.Count; s++) {
						if (((SBObject)secondLevelApps.ObjectAt (NSValue.FromObject (s))).ValueForKey((NSString)"name") != null) {
							// add each of the applications to our list of applications
							currentApps.Add (new FinderFile ((SBObject)secondLevelApps.ObjectAt (NSValue.FromObject (s))));
						}
					}

				}

			}
			
			// now add each FinderFile to the table content array
			foreach (FinderFile ff in currentApps) {
				appTableContent.AddObject (ff);
			}
			
			progressIndicator.StopAnimation(this);

			applicationsTable.Enabled = true;
		}
		
		partial void launchSelected (NSObject sender) {

			int theRow = appTableContent.SelectionIndex;
			
			if (theRow != Int32.MaxValue) {
				// get the application objects
				NSObject[] applicationObjects = appTableContent.ArrangedObjects();
				// get the application object for the row selected
				FinderFile selectedApplication = (FinderFile)applicationObjects[theRow];
				// the selector to use for opening the application
				Selector sel = new Selector("openUsing:withProperties:");
				// perform the selector on the finder object
				selectedApplication.FinderObject.PerformSelector(sel,finder,0);
			}
		}
		
	}
}

