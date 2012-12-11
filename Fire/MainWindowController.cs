using System;
using System.Collections.Generic;
using System.Drawing;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

namespace Fire
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		// Root layer and the two paricle emitters
		CALayer rootLayer;
		CAEmitterLayer fireEmitter;
		CAEmitterLayer smokeEmitter;

		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}

		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
		}

		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			// Create the root layer
			rootLayer = new CALayer ();

			// Set the root layer;s background color to black
			rootLayer.BackgroundColor = new CGColor (0,0,0);

			// Create the fire emitter layer
			fireEmitter = new CAEmitterLayer ();
			fireEmitter.Position = new System.Drawing.PointF (225,50);
			fireEmitter.Mode = CAEmitterLayer.ModeOutline;
			fireEmitter.Shape = CAEmitterLayer.ShapeLine;
			fireEmitter.RenderMode = CAEmitterLayer.RenderAdditive;
			fireEmitter.Size = new SizeF (0,0);

			// Create the smoke emitter layer
			smokeEmitter = new CAEmitterLayer ();
			smokeEmitter.Position = new PointF (225,50);
			smokeEmitter.Mode = CAEmitterLayer.ModePoints;

			// Create the fire emitter cell
			CAEmitterCell fire = CAEmitterCell.EmitterCell ();
			fire.EmissionLongitude = (float)Math.PI;
			fire.BirthRate = 0;
			fire.Velocity = 80;
			fire.VelocityRange = 30;
			fire.EmissionRange = 1.1f;
			fire.AccelerationY = 200;
			fire.ScaleSpeed = 0.3f;

			CGColor color = new CGColor (0.8f,0.4f,0.2f,0.10f);
			fire.Color = color;
			fire.Contents = NSImage.ImageNamed ("fire.png").AsCGImage (RectangleF.Empty, null, null);

			// Name the cell so that it can be animated later using keypaths
			fire.Name = "fire";

			// Add the fire emitter cell to the fire emitter layer
			fireEmitter.Cells = new CAEmitterCell[] { fire };

			//Create the smoke emitter cell
			CAEmitterCell smoke = CAEmitterCell.EmitterCell ();
			smoke.BirthRate = 11;
			smoke.EmissionLongitude = (float)Math.PI / 2;
			smoke.LifeTime = 0;
			smoke.Velocity = 40;
			smoke.VelocityRange = 20;
			smoke.EmissionRange = (float)Math.PI / 4;
			smoke.Spin = 1;
			smoke.SpinRange = 6;
			smoke.AccelerationY = 160;
			smoke.Scale = 0.1f;
			smoke.AlphaSpeed = -0.12f;
			smoke.ScaleSpeed = 0.7f;
			smoke.Contents = NSImage.ImageNamed ("smoke.png").AsCGImage (RectangleF.Empty, null, null);
			//Name the cell so that it can be animated later using keypaths
			smoke.Name = "smoke";

			// Add the smoke emitter cell to the smoke emitter layer
			smokeEmitter.Cells = new CAEmitterCell[] { smoke };

			// Add the two emitter layers to the root layer
			rootLayer.AddSublayer (smokeEmitter);
			rootLayer.AddSublayer (fireEmitter);

			// Set the view's layer to the base layer
			view.Layer = rootLayer;
			view.WantsLayer = true;

			// Set the fire simulation to reflect the intial slider postion
			slidersChanged (this);

			// Force the view to update
			view.NeedsDisplay = true;

		}

		partial void slidersChanged (NSObject sender)
		{
			// Query the gasSlider's value
			float gas = gasSlider.IntValue / 100.0f;

			// Update the fire properties
			fireEmitter.SetValueForKeyPath ((NSNumber)(gas * 1000), (NSString)"emitterCells.fire.birthRate");
			fireEmitter.SetValueForKeyPath ((NSNumber)gas, (NSString)"emitterCells.fire.lifetime");
			fireEmitter.SetValueForKeyPath ((NSNumber)(gas * 0.35), (NSString)"emitterCells.fire.lifetimeRange");
			fireEmitter.Size = new SizeF (50 * gas, 0);
			
			// Update the smoke properties
			smokeEmitter.SetValueForKeyPath ((NSNumber)(gas * 4), (NSString)"emitterCells.smoke.lifetime");
			CGColor color = new CGColor (1,1,1,gas * 0.3f);
			smokeEmitter.SetValueForKeyPath(color.Handle, (NSString)"emitterCells.smoke.color");
		}


	}
}

