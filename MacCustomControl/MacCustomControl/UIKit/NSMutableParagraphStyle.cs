using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace UIKit
{
	public class NSMutableParagraphStyle : NSObject
	{
		#region Computed Variables
		public AppKit.NSMutableParagraphStyle ParagraphStyle { get; set; }

		public UITextAlignment Alignment {
			get {
				switch (ParagraphStyle.Alignment) {
				case NSTextAlignment.Center:
					return UITextAlignment.Center;
				case NSTextAlignment.Justified:
					return UITextAlignment.Justified;
				case NSTextAlignment.Left:
					return UITextAlignment.Left;
				case NSTextAlignment.Natural:
					return UITextAlignment.Natural;
				case NSTextAlignment.Right:
					return UITextAlignment.Right;
				default:
					return UITextAlignment.Left;
				}
			}
			set {
				switch (value) {
				case UITextAlignment.Center:
					ParagraphStyle.Alignment = NSTextAlignment.Center;
					break;
				case UITextAlignment.Justified:
					ParagraphStyle.Alignment = NSTextAlignment.Justified;
					break;
				case UITextAlignment.Left:
					ParagraphStyle.Alignment = NSTextAlignment.Left;
					break;
				case UITextAlignment.Natural:
					ParagraphStyle.Alignment = NSTextAlignment.Natural;
					break;
				case UITextAlignment.Right:
					ParagraphStyle.Alignment = NSTextAlignment.Right;
					break;
				}
			}
		}

		public UILineBreakMode LineBreakMode {
			get {
				switch (ParagraphStyle.LineBreakMode) {
				case NSLineBreakMode.CharWrapping:
					return UILineBreakMode.CharacterWrap;
				case NSLineBreakMode.Clipping:
					return UILineBreakMode.Clip;
				case NSLineBreakMode.TruncatingHead:
					return UILineBreakMode.HeadTruncation;
				case NSLineBreakMode.TruncatingMiddle:
					return UILineBreakMode.MiddleTruncation;
				case NSLineBreakMode.TruncatingTail:
					return UILineBreakMode.TailTruncation;
				case NSLineBreakMode.ByWordWrapping:
					return UILineBreakMode.WordWrap;
				default:
					return UILineBreakMode.CharacterWrap;
				}
			}
			set {
				switch (value) {
				case UILineBreakMode.CharacterWrap:
					ParagraphStyle.LineBreakMode = NSLineBreakMode.CharWrapping;
					break;
				case UILineBreakMode.Clip:
					ParagraphStyle.LineBreakMode = NSLineBreakMode.Clipping;
					break;
				case UILineBreakMode.HeadTruncation:
					ParagraphStyle.LineBreakMode = NSLineBreakMode.TruncatingHead;
					break;
				case UILineBreakMode.MiddleTruncation:
					ParagraphStyle.LineBreakMode = NSLineBreakMode.TruncatingMiddle;
					break;
				case UILineBreakMode.TailTruncation:
					ParagraphStyle.LineBreakMode = NSLineBreakMode.TruncatingTail;
					break;
				case UILineBreakMode.WordWrap:
					ParagraphStyle.LineBreakMode = NSLineBreakMode.ByWordWrapping;
					break;
				}
			}
		}
		#endregion

		#region Type Conversion
		public static implicit operator AppKit.NSMutableParagraphStyle(NSMutableParagraphStyle style) {
			return style.ParagraphStyle;
		}

		public static implicit operator NSMutableParagraphStyle(AppKit.NSMutableParagraphStyle style) {
			return new NSMutableParagraphStyle(style);
		}
		#endregion

		#region Constructors
		public NSMutableParagraphStyle() {
			// Initialize
			this.ParagraphStyle = new AppKit.NSMutableParagraphStyle();
		}

		public NSMutableParagraphStyle(AppKit.NSMutableParagraphStyle style) : base() {
			// Initialize
			this.ParagraphStyle = style;
		}

		public NSMutableParagraphStyle (NSObjectFlag x) : base(x) {
		}

		public NSMutableParagraphStyle (IntPtr handle) : base(handle) {
		}
		#endregion
	}
}

