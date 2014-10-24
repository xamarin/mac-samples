using System;
using Foundation;
using CoreFoundation;
using SceneKit;

namespace Bananas
{
	public class SharedAppDelegate : NSObject
	{
		public static SharedAppDelegate AppDelegate { get; set; }

		public SharedSceneView Scene { get; set; }

		public SharedAppDelegate (SharedSceneView scene)
		{
			Scene = scene;
		}

		public void TogglePaused ()
		{
			GameState currentState = GameSimulation.Sim.CurrentGameState;

			if (currentState == GameState.Paused)
				GameSimulation.Sim.SetGameState (GameState.InGame);
			else if (currentState == GameState.InGame)
				GameSimulation.Sim.SetGameState (GameState.Paused);
		}

		public void CommonApplicationDidFinishLaunching (Action completionHandler)
		{
			Scene.ShowsStatistics = true;
			Scene.BackgroundColor = AppKit.NSColor.Black;

			NSProgress progress = NSProgress.FromTotalUnitCount (10);

			DispatchQueue queue = DispatchQueue.GetGlobalQueue (DispatchQueuePriority.Default);
			queue.DispatchSync (() => {
				progress.BecomeCurrent (2);
				var ui = new InGameScene (Scene.Bounds.Size);

				DispatchQueue.MainQueue.DispatchAsync (() =>
					Scene.OverlayScene = ui
				);

				progress.ResignCurrent ();
				progress.BecomeCurrent (3);

				GameSimulation gameSim = GameSimulation.Sim;
				gameSim.GameUIScene = ui;

				progress.ResignCurrent ();
				progress.BecomeCurrent (3);

				SCNTransaction.Flush ();

				// Preload
				Scene.Prepare (gameSim, new Func<bool> (() =>
					true
				));
				progress.ResignCurrent ();
				progress.BecomeCurrent (1);

				// Game Play Specific Code
				gameSim.GameUIScene.GameStateDelegate = gameSim.GameLevel;
				gameSim.GameLevel.ResetLevel ();
				gameSim.SetGameState (GameState.PreGame);

				progress.ResignCurrent ();
				progress.BecomeCurrent (1);

				DispatchQueue.MainQueue.DispatchAsync (() => {
					Scene.Scene = gameSim;
					Scene.WeakSceneRendererDelegate = gameSim;

					if (completionHandler != null)
						completionHandler ();
				});

				progress.ResignCurrent ();
			});
		}
	}
}

