using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using Tobii.Interaction;

using System.Collections.Generic;

using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using Eyetrack.Runners;

using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Overlay.NET.Common;
using Process.NET;
using Process.NET.Memory;

namespace Eyetrack.Runners.RectangleRemover
{
    class RectangleRemover : Runner
    {
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        public override void run()
        {
            Log.Debug(@"Please type the process name of the window you want to attach to, e.g 'notepad.");
            Log.Debug("Note: If there is more than one process found, the first will be used.");

            // Set up objects/overlay
            var processName = Console.ReadLine();
            var process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
            if (process == null)
            {
                Log.Warn($"No process by the name of {processName} was found.");
                Log.Warn("Please open one or use a different name and restart the demo.");
                Console.ReadLine();
                return;
            }
        }
    }
}
