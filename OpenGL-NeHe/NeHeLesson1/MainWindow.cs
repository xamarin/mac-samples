using System;
using Foundation;
using AppKit;

namespace NeHeLesson1
{
        public partial class MainWindow : AppKit.NSWindow
        {
                public MainWindow (IntPtr handle) : base(handle)
                {
                }

                public MainWindow (NSCoder coder) : base(coder)
                {
                }
        }
}

