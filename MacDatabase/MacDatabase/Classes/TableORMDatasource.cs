using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;
using SQLite;

namespace MacDatabase
{
	public class TableORMDatasource : NSTableViewDataSource
	{
		#region Computed Properties
		public List<OccupationModel> Occupations { get; set;} = new List<OccupationModel>();
		public SQLiteConnection Conn { get; set; }
		#endregion

		#region Constructors
		public TableORMDatasource (SQLiteConnection conn)
		{
			// Initialize
			this.Conn = conn;
			LoadOccupations ();
		}
		#endregion

		#region Public Methods
		public void LoadOccupations() {

			// Get occupations from database
			var query = Conn.Table<OccupationModel> ();

			// Copy into table collection
			Occupations.Clear ();
			foreach (OccupationModel occupation in query) {
				Occupations.Add (occupation);
			}

		}
		#endregion

		#region Override Methods
		public override nint GetRowCount (NSTableView tableView)
		{
			return Occupations.Count;
		}
		#endregion
	}
}

