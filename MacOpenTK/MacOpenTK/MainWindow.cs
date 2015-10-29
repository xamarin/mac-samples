using System;
using System.Drawing;

using Foundation;
using AppKit;

using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.MacOS;

namespace MacOpenTK
{
	public partial class MainWindow : NSWindow
	{
		#region Computed Properties

		public MonoMacGameView Game { get; set; }

		#endregion

		#region Constructors

		public MainWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
		}

		#endregion

		#region Override Methods

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Create new Game View and replace the window content with it
			Game = new MonoMacGameView (ContentView.Frame);
			ContentView = Game;

			// Wire-up any required Game events
			Game.Load += (sender, e) => {
				// Initialize settings, load textures and sounds here
			};

			// Adjust the GL view to be the same size as the window
			Game.Resize += (sender, e) => GL.Viewport (0, 0, Game.Size.Width, Game.Size.Height);

			Game.UpdateFrame += (sender, e) => {
				// Add any game logic or physics
			};

			Game.RenderFrame += (sender, e) => {
				// Setup buffer
				GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
				GL.MatrixMode (MatrixMode.Projection);

				// Draw a simple triangle
				GL.LoadIdentity ();
				GL.Ortho (-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
				GL.Begin (BeginMode.Triangles);
				GL.Color3 (Color.MidnightBlue);
				GL.Vertex2 (-1.0f, 1.0f);
				GL.Color3 (Color.SpringGreen);
				GL.Vertex2 (0.0f, -1.0f);
				GL.Color3 (Color.Ivory);
				GL.Vertex2 (1.0f, 1.0f);
				GL.End ();
			};

			// Run the game at 60 updates per second
			Game.Run (60.0);
		}

		#endregion
	}
}
