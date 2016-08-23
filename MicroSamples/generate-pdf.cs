//
// generate-pdf.cs: This sample renders a red circle in a PDF file
//

using System;
using System.Drawing;
using System.IO;
using AppKit;
using CoreGraphics;
using Foundation;

class DrawCircleInPDF {

	static void Main ()
	{
		NSApplication.Init ();
		NSUrl path = NSUrl.FromFilename (Path.Combine ("../../..", "demo.pdf")); //Escape out of generate-pdf.app/Contents/Resources
		var pdf = new CGContextPDF (path, new RectangleF (0, 0, 617, 792));

		pdf.BeginPage (null);
		pdf.SetFillColor (1, 0, 0, 1);
		pdf.AddArc (300, 300, 100, 0, (float) (2 * Math.PI), true);
		pdf.FillPath ();
		pdf.EndPage ();
		pdf.Flush ();
	}
}
