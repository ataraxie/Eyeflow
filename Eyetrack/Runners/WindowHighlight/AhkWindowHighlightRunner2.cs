using System;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using Tobii.Interaction;
using System.Runtime.InteropServices;

using System.Collections.Generic;

using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using Eyetrack.Runners;
using System.Threading;
using AutoHotkey.Interop;

namespace Eyetrack.Runners.WindowHighlight
{
    public class AhkWindowHighlightRunner2 : Runner
    {
        [DllImport("user32.dll")][return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool IsIconic(IntPtr handle);

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(System.Drawing.Point p);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

        private static int GAZE_THRESHOLD = 10;
        private static String PATH = "Resources\\ahk\\functions.ahk"; // must be in bin/debug
        private static AutoHotkeyEngine AHK = AutoHotkeyEngine.Instance;

        private int gazeCount = 0;

        public override void run()
        {
            AHK.LoadFile(PATH);
            Host host = new Host();
            host.Streams.CreateGazePointDataStream().GazePoint(onGaze);
            Console.ReadKey();
            restoreWindows();
            host.DisableConnection();
        }

        private void restoreWindows()
        {
            foreach(Process process in Process.GetProcesses())
            {
                AHK.ExecFunction("ShowWindow", process.Id.ToString());
            }
        }

        private void onGaze(double x, double y, double timestamp)
        {
            this.gazeCount++;
            if (this.gazeCount % GAZE_THRESHOLD == 0)
            {
                Console.WriteLine("Gaze: ("+x+"/"+y+")");
                AHK.ExecFunction("HandleWindowsByCoords", x.ToString(), y.ToString());
            }
        }

    }
}