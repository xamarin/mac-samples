using System;
using AppKit;
using Foundation;
using System.Collections.Generic;
using CoreData;
using System.Linq;

namespace Earthquakes
{
	public partial class QuakesViewController : NSViewController
	{
		const int BatchSize = 128;

		NSButton fetchQuakesButton;
		NSTableView tableView;
		NSManagedObjectContext context;

		/// The managed object context for the view controller (which is bound to the persistent store coordinator for the application).
		NSManagedObjectContext ManagedObjectContext {
			get {
				if (context != null)
					return context;
					
				context = new NSManagedObjectContext (NSManagedObjectContextConcurrencyType.MainQueue);
				context.PersistentStoreCoordinator = CoreDataStackManager.SharedManager.PersistentStoreCoordinator;

				return context;
			}
		}

		public QuakesViewController (IntPtr handle) : base (handle)
		{
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public QuakesViewController (NSCoder coder) : base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			fetchQuakesButton = (NSButton)View.Subviews [0];
			fetchQuakesButton.Activated += FetchQuakes;

			tableView = (NSTableView)View.Subviews [1].Subviews [0].Subviews [0];
			tableView.Source = new QuakeTableSourse (new List<Quake> ());

			ReloadTableView ();
		}

		void FetchQuakes (object sender, EventArgs e)
		{
			fetchQuakesButton.Enabled = false;
			var jsonURL = new NSUrl ("http://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/all_month.geojson");
			var session = NSUrlSession.FromConfiguration (NSUrlSessionConfiguration.EphemeralSessionConfiguration);
			NSUrlSessionTask task = session.CreateDataTask (jsonURL, (data, response, error) => {

				if (data == null) {
					Console.WriteLine ("Error connecting: {0}", error.LocalizedDescription);
					return;
				}

				NSError anyError;
				NSManagedObjectContext taskContext = CreatePrivateQueueContext (out anyError);
				var jsonDictionary = NSJsonSerialization.Deserialize (data, NSJsonReadingOptions.AllowFragments, new NSError ());

				if (jsonDictionary == null) {
					Console.WriteLine ("Error creating JSON dictionary: {0}", anyError.LocalizedDescription);
					return;
				}

				var featuresArray = (NSArray)jsonDictionary.ValueForKey ((NSString)"features");
				int totalFeatureCount = (int)featuresArray.Count;

				int numBatches = totalFeatureCount / BatchSize;
				numBatches += totalFeatureCount % BatchSize > 0 ? 1 : 0;
				for (int batchNumber = 0; batchNumber < numBatches; batchNumber++) {
					int rangeStart = batchNumber * BatchSize;
					int rangeLength = Math.Min (BatchSize, totalFeatureCount - batchNumber * BatchSize);

					NSArray featuresBatchArray = featuresArray.SubarrayWithRange (new NSRange (rangeStart, rangeLength));
					// Create a request to fetch existing quakes with the same codes as those in the JSON data.
					// Existing quakes will be updated with new data; if there isn't a match, then create a new quake to represent the event.
					NSFetchRequest matchingQuakeRequest = NSFetchRequest.FromEntityName ("Quake");

					// Get the codes for each of the features and store them in an array.
					NSArray codesDump = (NSArray)featuresBatchArray.ValueForKeyPath ((NSString)"properties.code");

					matchingQuakeRequest.Predicate = NSPredicate.FromFormat ("code in %@", codesDump);
					var rawFetch = taskContext.ExecuteFetchRequest (matchingQuakeRequest, out anyError);
					Quake[] allMatchingQuakes = Array.ConvertAll (rawFetch, item => (Quake)item);
					NSString[] codes = NSArray.FromArray<NSString> (codesDump);

					for (int k = 0; k < codes.Length; k++) {
						var code = codes [k];
						var matchingQuakes = allMatchingQuakes.Where (q => q.Code == code).ToArray<Quake> ();

						Quake quake = null;

						int matchingLength = matchingQuakes.Length;
						switch (matchingLength) {
						case 0:
							//Insert new item
							quake = (Quake)NSEntityDescription.InsertNewObjectForEntityForName ("Quake", taskContext);
							break;
						case 1:
							//Update existing item
							quake = matchingQuakes [0];
							break;
						default:
							//Remove duplicates
							for (int i = 1; i < matchingQuakes.Length; i++)
								taskContext.DeleteObject (matchingQuakes [i]);

							quake = matchingQuakes [0];
							break;
						}

						var result = featuresBatchArray.GetItem <NSDictionary> ((nuint)k);
						var quakeDictionary = (NSDictionary)result.ObjectForKey ((NSString)"properties");
						quake.UpdateFromDictionary (quakeDictionary);
					}

					if (!taskContext.Save (out anyError)) {
						Console.WriteLine ("Error saving batch: {0}", anyError.LocalizedDescription);
						return;
					}

					taskContext.Reset ();
				}

				// Bounce back to the main queue to reload the table view and reenable the fetch button.
				NSOperationQueue.MainQueue.AddOperation (() => {
					ReloadTableView ();
					fetchQuakesButton.Enabled = true;
				});
			});

			task.Resume ();
		}

		/// Fetch quakes ordered in time and reload the table view.
		void ReloadTableView ()
		{
			NSFetchRequest request = NSFetchRequest.FromEntityName ("Quake");
			request.SortDescriptors = new NSSortDescriptor[] { new NSSortDescriptor ("time", false) };

			NSError anyError;
			NSObject[] fetchedQuakes = ManagedObjectContext.ExecuteFetchRequest (request, out anyError);

			if (fetchedQuakes == null) {
				Console.WriteLine ("Error fetching: {0}", anyError.LocalizedDescription);
				return;
			}

			var quakes = Array.ConvertAll (fetchedQuakes, item => (Quake)item).ToList<Quake> ();
			tableView.Source = new QuakeTableSourse (quakes);
			tableView.ReloadData ();
		}

		/// The managed object context for the view controller (which is bound to the persistent store coordinator for the application).
		NSManagedObjectContext CreatePrivateQueueContext (out NSError error)
		{
			// It uses the same store and model, but a new persistent store coordinator and context.
			var localCoordinator = new NSPersistentStoreCoordinator (CoreDataStackManager.SharedManager.ManagedObjectModel);
			NSPersistentStore store = localCoordinator.AddPersistentStoreWithType (NSPersistentStoreCoordinator.SQLiteStoreType, null, CoreDataStackManager.SharedManager.StoreURL, null, out error);

			if (store == null)
				return null;

			var context = new NSManagedObjectContext (NSManagedObjectContextConcurrencyType.PrivateQueue);
			context.PersistentStoreCoordinator = localCoordinator;
			context.UndoManager = null;
			return context;
		}
	}

	public static class Extension
	{
		public static NSArray SubarrayWithRange (this NSArray array, NSRange range)
		{
			var result = new object[range.Length];
			for (nint i = range.Location, j = 0; j < range.Length; i++, j++)
				result [j] = array.GetItem<NSObject> ((nuint)i);

			return NSArray.FromObjects (result);
		}
	}
}

