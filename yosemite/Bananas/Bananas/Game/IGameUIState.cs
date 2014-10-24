using System;
using CoreGraphics;

namespace Bananas
{
	public interface IGameUIState
	{
		int Score { get; set; }

		int CoinsCollected { get; set; }

		int BananasCollected  { get; set; }

		double SecondsRemaining  { get; set; }

		CGPoint ScoreLabelLocation  { get; set; }
	}
}

