using System;
using Foundation;
using AppKit;

namespace UIKit
{
	public class UIStringAttributes : NSObject
	{
		#region Computed Properties
		public UIColor BackgroundColor { get; set;}

		public float? BaselineOffset { get; set;}

		public float? Expansion { get; set;}

		public UIFont Font { get; set;}

		public UIColor ForegroundColor { get; set;}

		public float? KerningAdjustment { get; set;}

		public NSLigatureType? Ligature { get; set;}

		public NSUrl Link { get; set;}

		public float? Obliqueness { get; set;}

		public NSParagraphStyle ParagraphStyle { get; set;}

		public NSShadow Shadow { get; set;}

		public UIColor StrikethroughColor { get; set;}

		public NSUnderlineStyle? StrikethroughStyle { get; set;}

		public UIColor StrokeColor { get; set;}

		public float? StrokeWidth { get; set;}

		public NSTextAttachment TextAttachment { get; set;}

		public NSTextEffect TextEffect { get; set;}

		public UIColor UnderlineColor { get; set;}

		public NSUnderlineStyle? UnderlineStyle { get; set;}

		public NSString WeakTextEffect { get; set;}

		public NSNumber[] WritingDirectionInt { get; set;}
		#endregion

		#region Constructors
		public UIStringAttributes (){

		}

		public UIStringAttributes (NSDictionary dictionary){

		}
		#endregion
	}
}

