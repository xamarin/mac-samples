using System;
using System.Collections.Generic;
using MonoMac.AppKit;

namespace VillainTracker
{
	/// <summary>
	/// Keeps villain data
	/// </summary>
	public class Villain
	{
		public string Name { get; set; }
		public string LastKnownLocation { get; set; }
		public DateTime LastSeenDate { get; set; }
		public string SwornEnemy { get; set; }
		public string PrimaryMotivation { get; set; }
		public IList<string> Powers { get; set; }
		public string PowerSource { get; set; }
		public int Evilness { get; set; }
		public NSImage Mugshot { get; set; }
		public string Notes { get; set; }

		public Villain ()
		{
			Name = "";
			LastKnownLocation = "";
			LastSeenDate = DateTime.Now;
			SwornEnemy = "";
			PrimaryMotivation = "";
			Powers = new[] { "" };
			PowerSource = "";
			Evilness = 0;
			Mugshot = NSImage.ImageNamed ("NSUser");
			Notes = "";
		}
	}
}

