using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using System.Drawing;
using Tobii.Interaction;
using System.Runtime.InteropServices;

using System.Collections.Generic;

using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using Eyetrack.Runners;
using System.Threading;

namespace Eyetrack.Runners.WindowIdentifier
{
    public class WindowIdentifierRunner : Runner
    {
        private int gazeCount = 0;

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(System.Drawing.Point p);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private uint currentProcessId;

        public override void run()
        {
            var host = new Host();
            var gazePointDataStream = host.Streams.CreateGazePointDataStream();
            gazePointDataStream.GazePoint(onGaze);
            Console.ReadKey();
            host.DisableConnection();
        }

        private void onGaze(double x, double y, double timestamp)
        {
            this.gazeCount++;
            Point point = new Point((int)x, (int)y);
            IntPtr handle = WindowFromPoint(point);
            uint processId;
            GetWindowThreadProcessId(handle, out processId);
            if (this.gazeCount % 10 == 0)
            {
                if (this.currentProcessId != processId)
                {
                    this.currentProcessId = processId;
                    Process processInView = Process.GetProcessById((int)processId);
                    Console.WriteLine(processInView.ProcessName);
                }
            }

        }
    }
}