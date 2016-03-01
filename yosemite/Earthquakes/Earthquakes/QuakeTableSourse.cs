using System;
using AppKit;
using System.Collections.Generic;
using Foundation;

namespace Earthquakes
{
	public class QuakeTableSourse : NSTableViewSource
	{
		List<Quake> quakesSource;
		const string ColumnIdentifierPlace = "placeName";
		const string ColumnIdentifierTime = "time";
		const string ColumnIdentifierMagnitude = "magnitude";

		public QuakeTableSourse (List<Quake> quakes)
		{
			quakesSource = quakes;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return quakesSource.Count;
		}

		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			string identifier = tableColumn.Identifier;
			NSTableCellView cellView = (NSTableCellView)tableView.MakeView (identifier, this);
			Quake quake = quakesSource [(int)row];

			if (identifier == ColumnIdentifierPlace)
				cellView.TextField.StringValue = quake.Location;
			else if (identifier == ColumnIdentifierTime)
				cellView.TextField.ObjectValue = quake.Date;
			else if (identifier == ColumnIdentifierMagnitude)
				cellView.TextField.ObjectValue = quake.Magnitude;

			return cellView;
		}
	}
}

