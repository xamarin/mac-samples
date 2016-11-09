using System;
using AppKit;
using Foundation;

namespace TouchBarExample
{
	public class CanidateBarDelegate : TouchBarExampleDelegate, INSCandidateListTouchBarItemDelegate
	{
		public override int Count => 1;

		NSCandidateListTouchBarItem canidateItem;
		public override NSTouchBarItem MakeItem (NSTouchBar touchBar, string identifier)
		{
			switch (ParseId (identifier)) {
				case 0: {
					canidateItem = new NSCandidateListTouchBarItem (identifier);
					canidateItem.Delegate = this;
					canidateItem.SetCandidates (new NSString [] { (NSString)"Hello", (NSString)"World", (NSString)"Touch" },
							new NSRange (0, 3), "");
					return canidateItem;
				}
			}
			return null;
		}

		[Export ("candidateListTouchBarItem:beginSelectingCandidateAtIndex:")]
		public void BeginSelectingCandidate (NSCandidateListTouchBarItem anItem, nint index)
		{
			if (index == nint.MaxValue)
				Console.WriteLine ("BeginSelectingCandidate: None");
			else
				Console.WriteLine ("BeginSelectingCandidate {0} at {1}", anItem.Candidates [index], index);
		}

		[Export ("candidateListTouchBarItem:changeSelectionFromCandidateAtIndex:toIndex:")]
		public void ChangeSelectionFromCandidate (NSCandidateListTouchBarItem anItem, nint previousIndex, nint index)
		{
			// previousIndex and index always appear to return NSNotFound (nint.MaxValue) in this use case
			Console.WriteLine ("ChangeSelectionFromCandidate");
		}

		[Export ("candidateListTouchBarItem:changedCandidateListVisibility:")]
		public void ChangedCandidateListVisibility (NSCandidateListTouchBarItem anItem, bool isVisible)
		{
			Console.WriteLine ("ChangedCandidateListVisibility {0}", isVisible);
		}

		[Export ("candidateListTouchBarItem:endSelectingCandidateAtIndex:")]
		public void EndSelectingCandidate (NSCandidateListTouchBarItem anItem, nint index)
		{
			Console.WriteLine ("EndSelectingCandidate {0} at {1}", anItem.Candidates [index], index);
		}
	}
}
