using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace VillainTracker
{
	public partial class VillainTrackerAppDelegate: NSApplicationDelegate
	{	
		static readonly string[] motivations = new [] {
			"Greed", "Revenge", "Bloodlust", "Nihilism", "Insanity"};
		//static readonly string[] powers = new[] {
		//	"Strength", "Intellect", "Psionics", "Imperviousness", "Speed", "Stealth",
		//	"Fighting ability", "Time control", "Cosmic consciousness", "Size", "Special weapon attack", "Leadership"};
		
		List<Villain> villains = new List<Villain> ();
		Villain villain;
		
		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
		
		public override void DidFinishLaunching (NSNotification notification)
		{
			villain = new Villain {
				Name = "Lex Luthor",
				LastKnownLocation = "Smallville",
				SwornEnemy = "Superman",
				PrimaryMotivation = "Revenge",
				Powers = new [] {"Intellect", "Leadership"},
				PowerSource = "Superhero Action",
				Evilness = 9
			};
			
			villains.Add (villain);
			
			// initialize delegates after critical data initialized
			villainsTableView.DataSource = new DataSource (this); 
			villainsTableView.Delegate = new VillainsTableViewDelegate (this);
			
			notesView.TextDidChange += delegate {
				villain.Notes = notesView.Value;
			};
			villainsTableView.ReloadData ();
			villainsTableView.SelectRow (0, false);
			
			UpdateDetailViews ();
		}
		
		void UpdateDetailViews ()
		{
			nameView.StringValue = villain.Name;
			lastKnownLocationView.StringValue = villain.LastKnownLocation;
			lastSeenDateView.DateValue = (NSDate)villain.LastSeenDate;
			evilnessView.IntValue = villain.Evilness;
			powerSourceView.Title = villain.PowerSource;
			mugshotView.Image = villain.Mugshot;
			notesView.Value = villain.Notes;

			var villainSwornEnemy = (NSString)villain.SwornEnemy;
			if (swornEnemyView.IndexOf(villainSwornEnemy) == int.MaxValue)
				swornEnemyView.Add(villainSwornEnemy);

			swornEnemyView.Select(villainSwornEnemy);
			
			var cellTag = Array.IndexOf (motivations, villain.PrimaryMotivation);
			primaryMotivationView.SelectCellWithTag (cellTag >= 0 ? cellTag : 0);
			
			powersView.DeselectAllCells ();
			var powers = villain.Powers;

			foreach(var tag in from p in powers where powers.Contains (p) select powers.IndexOf (p))
				powersView.CellWithTag (tag).State = NSCellStateValue.On;
		}
		
		#region responding to actions
		
		partial void takeName (NSObject sender)
		{
			villain.Name = ((NSTextField)sender).StringValue;
			villainsTableView.ReloadData ();
			
			Console.WriteLine(string.Format("current villain properties: {0}", villain.Name));
		}
		
		partial void takeEvilness (NSObject sender)
		{
			villain.Evilness = ((NSLevelIndicator)sender).IntValue;
		}
		
		partial void takeLastKnownLocation (NSObject sender)
		{
			villain.LastKnownLocation = ((NSTextField)sender).StringValue;
		}
		
		partial void takeLastSeenDate (NSObject sender)
		{
			villain.LastSeenDate = (DateTime)((NSDatePicker)sender).DateValue;
			villainsTableView.ReloadData();
		}
		
		partial void takeMugshot (NSObject sender)
		{
			villain.Mugshot = ((NSImageView)sender).Image;
			villainsTableView.ReloadData();
		}
		
		partial void takePowers (NSObject sender)
		{
			villain.Powers = (from cell in ((NSMatrix) sender).Cells
				where cell.State == NSCellStateValue.On
				select cell.Title).ToList();
		}
		
		partial void takePowerSource (NSObject sender)
		{
			villain.PowerSource = ((NSPopUpButton)sender).Title;
		}
		
		partial void takePrimaryMotivation (NSObject sender)
		{
			villain.PrimaryMotivation = ((NSMatrix)sender).SelectedCell.Title;
		}
		
		partial void takeSwornEnemy (NSObject sender)
		{
			villain.SwornEnemy = ((NSTextField)sender).StringValue;
		}
		
		partial void newVillain (NSObject sender)
		{
			window.EndEditingFor (null);
		
			villains.Add (new Villain ());
			
			villainsTableView.ReloadData ();
			villainsTableView.SelectRow (villains.Count - 1, false);
		}
		
		partial void deleteVillain (NSObject sender)
		{
			window.EndEditingFor (null);
			var selectedRow = villainsTableView.SelectedRow;
			
			villains.Remove (villain);
			villainsTableView.ReloadData ();
			
			selectedRow = Math.Min (selectedRow, villains.Count - 1);
			if (selectedRow < 0)
				return;
			
			// deselect all rows to ensure that the table view see the selection
			// as "changed", even though it might still have the same row index
			villainsTableView.DeselectAll (null);
			villainsTableView.SelectRow (selectedRow, false);
			UpdateDetailViews ();
		}
		
		#endregion
	}
}
