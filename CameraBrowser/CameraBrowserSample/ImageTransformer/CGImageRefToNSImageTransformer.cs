using Foundation;
using ObjCRuntime;
using AppKit;
using CoreGraphics;

namespace CameraBrowserSample
{
    [Register("CGImageRefToNSImageTransformer")]
    public class CGImageRefToNSImageTransformer: NSValueTransformer
	{
        [Export("transformedValueClass")]
        public static new Class TransformedValueClass => new Class(typeof(NSImage));

        [Export("allowsReverseTransformation")]
        public static new bool AllowsReverseTransformation => false;

        public override NSObject TransformedValue(NSObject value)
        {
            if (value == null)
                return null;

            var img = Runtime.GetINativeObject<CGImage>(value.Handle, false);
            return new NSImage(img, CGSize.Empty);
        }
    }
}

