//
// generate-pdf.cs: This sample renders a red circle in a PDF file
//

using System;
using System.Drawing;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;

class DrawCircleInPDF {

	static void Main ()
	{
		var pdf = new CGContextPDF (NSUrl.FromFilename ("demo.pdf"), new RectangleF (0, 0, 617, 792));

		pdf.BeginPage (null);
		pdf.SetRGBFillColor (1, 0, 0, 1);
		pdf.AddArc (300, 300, 100, 0, (float) (2 * Math.PI), true);
		pdf.FillPath ();
		pdf.EndPage ();
		pdf.Flush ();
	}
}