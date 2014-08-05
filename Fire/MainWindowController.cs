using System;
using System.Drawing;

using Foundation;
using CoreAnimation;
using CoreGraphics;
using AppKit;

namespace Fire
{
	public partial class MainWindowController : AppKit.NSWindowController
	{
		// Root layer and the two paricle emitters
		CALayer rootLayer;
		CAEmitterLayer fireEmitter;
		CAEmitterLayer smokeEmitter;

		public MainWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}

		public MainWindowController () : base ("MainWindow")
		{
		}

		public override void AwakeFromNib ()
		{
			// center our fire horizontally on the view and 15px from the bottom
			var firePosition = new CGPoint (view.Bounds.Width / 2, 15);

			rootLayer = new CALayer {
				BackgroundColor = new CGColor (0, 0, 0)
			};

			fireEmitter = new CAEmitterLayer {
				Position = firePosition,
				Mode = CAEmitterLayer.ModeOutline,
				Shape = CAEmitterLayer.ShapeLine,
				RenderMode = CAEmitterLayer.RenderAdditive,
				Size = SizeF.Empty,
				Cells = new [] {
					new CAEmitterCell {
						Name = "fire", // name the cell so that it can be animated later using keypaths
						EmissionLongitude = (float)Math.PI,
						BirthRate = 0,
						Velocity = 80,
						VelocityRange = 30,
						EmissionRange = 1.1f,
						AccelerationY = 200,
						ScaleSpeed = 0.3f,
						Color = new CGColor (0.8f, 0.4f, 0.2f, 0.10f),
						Contents = NSImage.ImageNamed ("fire.png").CGImage
					}
				}
			};

			smokeEmitter = new CAEmitterLayer {
				Position = firePosition,
				Mode = CAEmitterLayer.ModePoints,
				Cells = new [] {
					new CAEmitterCell {
						Name = "smoke", // name the cell so that it can be animated later using keypaths
						BirthRate = 11,
						EmissionLongitude = (float)Math.PI / 2,
						LifeTime = 0,
						Velocity = 40,
						VelocityRange = 20,
						EmissionRange = (float)Math.PI / 4,
						Spin = 1,
						SpinRange = 6,
						AccelerationY = 160,
						Scale = 0.1f,
						AlphaSpeed = -0.12f,
						ScaleSpeed = 0.7f,
						Contents = NSImage.ImageNamed ("smoke.png").CGImage
					}
				}
			};

			rootLayer.AddSublayer (smokeEmitter);
			rootLayer.AddSublayer (fireEmitter);

			view.Layer = rootLayer;
			view.WantsLayer = true;

			// Set the fire simulation to reflect the intial slider postion
			slidersChanged (this);

			// Force the view to update
			view.NeedsDisplay = true;
		}

		partial void slidersChanged (NSObject sender)
		{
			var gas = gasSlider.IntValue / 100.0f;

			// Update the fire properties
			fireEmitter.SetValueForKeyPath ((NSNumber)(gas * 1000), (NSString)"emitterCells.fire.birthRate");
			fireEmitter.SetValueForKeyPath ((NSNumber)gas, (NSString)"emitterCells.fire.lifetime");
			fireEmitter.SetValueForKeyPath ((NSNumber)(gas * 0.35), (NSString)"emitterCells.fire.lifetimeRange");
			fireEmitter.Size = new SizeF (50 * gas, 0);

			var color = new CGColor (1, 1, 1, gas * 0.3f);
			smokeEmitter.SetValueForKeyPath ((NSNumber)(gas * 4), (NSString)"emitterCells.smoke.lifetime");
			smokeEmitter.SetValueForKeyPath(color.Handle, (NSString)"emitterCells.smoke.color");
		}
	}
}
