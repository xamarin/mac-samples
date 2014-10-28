using System;
using Foundation;
using CoreData;

namespace Earthquakes
{
	public class CoreDataStackManager : NSObject
	{
		static CoreDataStackManager sharedManager;

		string ApplicationDocumentsDirectoryName = NSBundle.MainBundle.BundleIdentifier;
		string MainStoreFileName = "Earthquakes.storedata";

		NSManagedObjectModel managedObjectModel;
		NSPersistentStoreCoordinator persistentStoreCoordinator;

		/// URL for directory the application uses to store the Core Data store file.
		NSUrl applicationDocumentsDirectory;

		/// URL for the Core Data store file.
		NSUrl storeURL;

		public static CoreDataStackManager SharedManager {
			get {
				if (sharedManager == null)
					sharedManager = new CoreDataStackManager ();

				return sharedManager;
			}
		}

		/// Managed object model for the application.
		public NSManagedObjectModel ManagedObjectModel {
			get {
				if (managedObjectModel != null)
					return managedObjectModel;

				NSUrl modelURL = NSBundle.MainBundle.GetUrlForResource ("Earthquakes", "mom");
				managedObjectModel = new NSManagedObjectModel (modelURL);
				return managedObjectModel;
			}
		}

		public NSUrl StoreURL {
			get {
				if (storeURL != null)
					return storeURL;

				storeURL = ApplicationSupportDirectory.Append (MainStoreFileName, false);
				return storeURL;
			}
		}

		/// Primary persistent store coordinator for the application.
		public NSPersistentStoreCoordinator PersistentStoreCoordinator {
			get {
				persistentStoreCoordinator = persistentStoreCoordinator ?? CreateCoordinator ();
				return persistentStoreCoordinator;
			}
		}

		NSUrl ApplicationSupportDirectory {
			get {
				applicationDocumentsDirectory = applicationDocumentsDirectory ?? CreateApplicationSupportDirectory ();
				return applicationDocumentsDirectory;
			}
		}

		public CoreDataStackManager ()
		{
		}

		NSPersistentStoreCoordinator CreateCoordinator ()
		{
			NSUrl url = StoreURL;

			if (url == null)
				return null;

			var psc = new NSPersistentStoreCoordinator (ManagedObjectModel);

			var keys = new object[] { NSPersistentStoreCoordinator.MigratePersistentStoresAutomaticallyOption,
				NSPersistentStoreCoordinator.InferMappingModelAutomaticallyOption
			};
			var values = new object[] { true, true };

			NSDictionary options = NSDictionary.FromObjectsAndKeys (values, keys);

			NSError error;
			NSPersistentStore store = psc.AddPersistentStoreWithType (NSPersistentStoreCoordinator.SQLiteStoreType, null, url, options, out error);

			if (store == null) {
				Console.WriteLine (error.Description);
				return null;
			}

			return psc;
		}

		NSUrl CreateApplicationSupportDirectory ()
		{
			NSFileManager fileManager = NSFileManager.DefaultManager;
			NSUrl[] URLs = fileManager.GetUrls (NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User);
			NSUrl url = URLs [URLs.Length - 1];
			url = url.Append (ApplicationDocumentsDirectoryName, false);
			NSError error;

			NSDictionary properties = url.GetResourceValues (new NSString[] { NSUrl.IsDirectoryKey }, out error);
			if (properties != null) {
				var isDirectoryNumber = (NSNumber)properties [NSUrl.IsDirectoryKey];
				if (isDirectoryNumber != null && !isDirectoryNumber.BoolValue) {
					Console.WriteLine ("Could not access the application data folder.");
					return null;
				}
			} else if (error.Code == (int)NSCocoaError.FileReadNoSuchFile) {
				bool ok = fileManager.CreateDirectory (url.Path, true, null);
				if (!ok) {
					Console.WriteLine ("Error occured: {0}", error.LocalizedDescription);
					return null;
				}
			}

			return url;
		}
	}
}

