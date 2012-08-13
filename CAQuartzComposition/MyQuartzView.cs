using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.QuartzComposer;
using MonoMac.CoreGraphics;
using MonoMac.CoreImage;
using MonoMac.CoreAnimation;

namespace CAQuartzComposition
{
	public partial class MyQuartzView : MonoMac.AppKit.NSView
	{
		QCCompositionLayer cubeLayer;
		QCCompositionLayer quadLayer;
		bool useFilters = true;
		
		public MyQuartzView (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public MyQuartzView (NSCoder coder) : base(coder) {}
		
		public override bool AcceptsFirstResponder ()
		{
			return true;
		}
		
		public override void AwakeFromNib ()
		{
			WantsLayer = true;
			Layer.AddSublayer (CubeComposition);
		}
		
		private QCCompositionLayer SwitchLayers()
		{
			if (CubeComposition.SuperLayer == null) {
				QuadComposition.RemoveFromSuperLayer ();
				return CubeComposition;
			} else {
				CubeComposition.RemoveFromSuperLayer ();
				return QuadComposition;
			}
		}
		
		private QCCompositionLayer CubeComposition {
			get {
				if (cubeLayer == null){
					var path = NSBundle.MainBundle.PathForResource ("VideoCube","qtz");
					
					cubeLayer = new QCCompositionLayer (path) {
						Frame = this.Frame
					};
					cubeLayer.AddSublayer(HelloLayer);
					cubeLayer.Filters = CompositionFilters();						
				}
				return cubeLayer;
			}
		}
		
		private QCCompositionLayer QuadComposition
		{
			get {
				if (quadLayer == null) {
					var path = NSBundle.MainBundle.PathForResource("VideoQuad","qtz");
					
					quadLayer = new QCCompositionLayer (path) {
						Frame = Frame
					};
					quadLayer.AddSublayer(HelloLayer);
					quadLayer.Filters = CompositionFilters();						
				}
				return quadLayer;
			}
		}
		
		private CATextLayer HelloLayer {
			get {
				var text = new CATextLayer() {
					String = "Hello MonoMac",
					Frame = Frame
				};
				return text;
			}
		}
		
		private CIFilter[] CompositionFilters()
		{
			var blurFilter = CIFilter.FromName ("CIGaussianBlur");
					
			blurFilter.SetDefaults ();
			blurFilter.SetValueForKey ((NSNumber)2, (NSString)"inputRadius");

			return new CIFilter[] { blurFilter };
		}
		
		partial void switchView (NSMenuItem sender)
		{
			Layer.AddSublayer (SwitchLayers ());
		}
		
		partial void filterSwitch (NSMenuItem sender)
		{
			useFilters = !useFilters; 
			sender.State = (useFilters) ? NSCellStateValue.On : NSCellStateValue.Off;
			
			if (useFilters) {
				CubeComposition.Filters = CompositionFilters(); 
				QuadComposition.Filters = CompositionFilters();
			} else {
				CubeComposition.Filters = null;
				QuadComposition.Filters = null;
			}
		}
		
	}
}

