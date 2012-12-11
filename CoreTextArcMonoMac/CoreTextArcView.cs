using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.CoreText;

namespace CoreTextArcMonoMac
{
        public partial class CoreTextArcView : MonoMac.AppKit.NSView
        {
                NSFont _font;
                string _string;
                float _radius;
                const double PI_2 = Math.PI / 2;
                const string ARCVIEW_DEFAULT_FONT_NAME = "Didot";
                const float ARCVIEW_DEFAULT_FONT_SIZE = 64;
                const float ARCVIEW_DEFAULT_RADIUS = 150;

                struct GlyphArcInfo
                {
                        public float width;
                        public float angle;
                        // in radians
                        public GlyphArcInfo (float w, float a)
                        {
                                width = w;
                                angle = a;
                        }
                }

                [Export("initWithFrame:")]
                public CoreTextArcView (RectangleF frame) : base(frame)
                {
                        Font = NSFont.FromFontName (ARCVIEW_DEFAULT_FONT_NAME, ARCVIEW_DEFAULT_FONT_SIZE);
                        Title = "Curvaceous MonoMac";
                        Radius = ARCVIEW_DEFAULT_RADIUS;
                        
                }

                static void PrepareGlyphArcInfo (CTLine line, long glyphCount, GlyphArcInfo[] glyphArcInfo)
                {
                        var runArray = line.GetGlyphRuns ();
                        
                        // Examine each run in the line, updating glyphOffset to track how far along the run is 
                        // in terms of glyphCount.
                        long glyphOffset = 0;
                        float ascent = 0;
                        float descent = 0;
                        float leading = 0;
                        foreach (var run in runArray) {
                                var runGlyphCount = run.GlyphCount;
                                
                                // Ask for the width of each glyph in turn.
                                var runGlyphIndex = 0;
                                for (; runGlyphIndex < runGlyphCount; runGlyphIndex++) {
                                        glyphArcInfo[runGlyphIndex + glyphOffset].width = (float)run.GetTypographicBounds (new NSRange (runGlyphIndex, 1), out ascent, out descent, out leading);
                                        
                                }
                                
                                glyphOffset += runGlyphCount;
                                
                        }
                        
                        var lineLength = line.GetTypographicBounds (out ascent, out descent, out leading);
                        
                        var prevHalfWidth = glyphArcInfo[0].width / 2.0;
                        glyphArcInfo[0].angle = (float)((prevHalfWidth / lineLength) * Math.PI);
                        
                        var lineGlyphIndex = 1;
                        for (; lineGlyphIndex < glyphCount; lineGlyphIndex++) {
                                
                                var halfWidth = glyphArcInfo[lineGlyphIndex].width / 2.0;
                                var prevCenterToCenter = prevHalfWidth + halfWidth;
                                
                                glyphArcInfo[lineGlyphIndex].angle = (float)((prevCenterToCenter / lineLength) * Math.PI);
                                
                                prevHalfWidth = halfWidth;
                                
                        }
                }

                public override void DrawRect (RectangleF dirtyRect)
                {
                        // Don't draw if we don't have a font or a title.
                        if (Font == null || Title == string.Empty)
                                return;
                        
                        // Initialize the text matrix to a known value
                        CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
                        context.TextMatrix = CGAffineTransform.MakeIdentity ();
                        
                        // Draw a white background
                        NSColor.White.Set ();
                        context.FillRect (dirtyRect);
                        
                        //CTLineRef line = CTLineCreateWithAttributedString((CFAttributedStringRef)self.attributedString);
                        CTLine line = new CTLine (AttributedString);
                        
                        int glyphCount = line.GlyphCount;
                        if (glyphCount == 0)
                                return;
                        
                        GlyphArcInfo[] glyphArcInfo = new GlyphArcInfo[glyphCount];
                        PrepareGlyphArcInfo (line, glyphCount, glyphArcInfo);
                        
                        // Move the origin from the lower left of the view nearer to its center.
                        context.SaveState ();
                        context.TranslateCTM (dirtyRect.GetMidX (), dirtyRect.GetMidY () - Radius / 2);
                        
                        // Stroke the arc in red for verification.
                        context.BeginPath ();
                        context.AddArc (0, 0, Radius, (float)Math.PI, 0, true);
                        context.SetRGBStrokeColor (1, 0, 0, 1);
                        context.StrokePath ();
                        
                        // Rotate the context 90 degrees counterclockwise.
                        context.RotateCTM ((float)PI_2);
                        
                        // Now for the actual drawing. The angle offset for each glyph relative to the previous 
                        //      glyph has already been calculated; with that information in hand, draw those glyphs 
                        //      overstruck and centered over one another, making sure to rotate the context after each 
                        //      glyph so the glyphs are spread along a semicircular path.
                        PointF textPosition = new PointF (0, Radius);
                        context.TextPosition = textPosition;
                        
                        var runArray = line.GetGlyphRuns ();
                        var runCount = runArray.Count ();
                        
                        var glyphOffset = 0;
                        var runIndex = 0;
                        
                        for (; runIndex < runCount; runIndex++) {
                                var run = runArray[runIndex];
                                var runGlyphCount = run.GlyphCount;
                                bool drawSubstitutedGlyphsManually = false;
                                CTFont runFont = run.GetAttributes ().Font;
                                
                                // Determine if we need to draw substituted glyphs manually. Do so if the runFont is not 
                                //      the same as the overall font.
                                NSFont rrunFont = new NSFont (runFont.Handle);
                                // used for comparison
                                if (DimsSubstitutedGlyphs && Font != rrunFont) {
                                        drawSubstitutedGlyphsManually = true;
                                }
                                
                                var runGlyphIndex = 0;
                                for (; runGlyphIndex < runGlyphCount; runGlyphIndex++) {
                                        var glyphRange = new NSRange (runGlyphIndex, 1);
                                        context.RotateCTM (-(glyphArcInfo[runGlyphIndex + glyphOffset].angle));
                                        
                                        // Center this glyph by moving left by half its width.
                                        var glyphWidth = glyphArcInfo[runGlyphIndex + glyphOffset].width;
                                        var halfGlyphWidth = glyphWidth / 2.0;
                                        var positionForThisGlyph = new PointF (textPosition.X - (float)halfGlyphWidth, textPosition.Y);
                                        
                                        // Glyphs are positioned relative to the text position for the line, so offset text position leftwards by this glyph's 
                                        //      width in preparation for the next glyph.
                                        textPosition.X -= glyphWidth;
                                        
                                        CGAffineTransform textMatrix = run.TextMatrix;
                                        textMatrix.x0 = positionForThisGlyph.X;
                                        textMatrix.y0 = positionForThisGlyph.Y;
                                        context.TextMatrix = textMatrix;
                                        
                                        if (!drawSubstitutedGlyphsManually)
                                                run.Draw (context, glyphRange);
                                        else {
                                                
                                                // We need to draw the glyphs manually in this case because we are effectively applying a graphics operation by 
                                                //      setting the context fill color. Normally we would use kCTForegroundColorAttributeName, but this does not apply 
                                                // as we don't know the ranges for the colors in advance, and we wanted demonstrate how to manually draw.
                                                var cgFont = runFont.ToCGFont ();
                                                
                                                var glyph = run.GetGlyphs (glyphRange);
                                                var position = run.GetPositions (glyphRange);
                                                
                                                context.SetFont (cgFont);
                                                context.SetFontSize (runFont.Size);
                                                context.SetRGBFillColor (0.25f, 0.25f, 0.25f, 1);
                                                context.ShowGlyphsAtPositions (glyph, position, 1);
                                                
                                        }
                                        
                                        // Draw the glyph bounds 
                                        if (ShowsGlyphBounds) {
                                                
                                                var glyphBounds = run.GetImageBounds (context, glyphRange);
                                                context.SetRGBStrokeColor (0, 0, 1, 1);
                                                context.StrokeRect (glyphBounds);
                                        }
                                        
                                        // Draw the bounding boxes defined by the line metrics
                                        if (ShowsLineMetrics) {
                                                
                                                var lineMetrics = new RectangleF ();
                                                float ascent = 0;
                                                float descent = 0;
                                                float leading = 0;
                                                
                                                run.GetTypographicBounds (glyphRange, out ascent, out descent, out leading);
                                                
                                                // The glyph is centered around the y-axis
                                                lineMetrics.Location = new PointF (-(float)halfGlyphWidth, positionForThisGlyph.Y - descent);
                                                lineMetrics.Size = new SizeF (glyphWidth, ascent + descent);
                                                context.SetRGBStrokeColor (0, 1, 0, 1);
                                                context.StrokeRect (lineMetrics);
                                        }
                                        
                                        
                                }
                                
                                glyphOffset += runGlyphCount;
                        }
                        
                        context.RestoreState ();
                }

                public NSFont Font {
                        get { return _font; }
                        set { _font = value; }
                }

                public string Title {
                        get { return _string; }
                        set { _string = value; }
                }

                public float Radius {
                        get { return _radius; }
                        set { _radius = value; }
                }

                private NSAttributedString AttributedString {
                        get {
                                NSObject[] objects = new NSObject[] { Font, (NSNumber)0 };
                                NSObject[] keys = new NSObject[] { NSAttributedString.FontAttributeName, NSAttributedString.LigatureAttributeName };
                                NSDictionary attributes = NSDictionary.FromObjectsAndKeys (objects, keys);
                                
                                NSAttributedString attrString = new NSAttributedString (Title, attributes);
                                return attrString;
                        }
                }

                public bool ShowsGlyphBounds { get; set; }

                public bool ShowsLineMetrics { get; set; }

                public bool DimsSubstitutedGlyphs { get; set; }
                
        }
}

