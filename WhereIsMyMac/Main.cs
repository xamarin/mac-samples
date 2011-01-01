using System;
using MonoMac.AppKit;

namespace WhereIsMyMac
{
        class MainClass
        {
                static void Main (string[] args)
                {
                        NSApplication.Init ();
                        NSApplication.Main (args);
                }
        }
}

