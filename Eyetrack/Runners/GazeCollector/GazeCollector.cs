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

namespace Eyetrack.Runners.GazeCollector
{
    class GazeCollector
    {
        private static int GAZE_THRESHOLD = 1;

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(System.Drawing.Point p);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public event EventHandler GazeEvent;
        public event EventHandler QuitEvent;

        Host host;
        public int lastGazedProcessId;

        private int gazeCount = 0;
        public Dictionary<int, long> processTimestamps = new Dictionary<int, long>();

        public void start()
        {
            this.host = new Host();
            this.host.Streams.CreateGazePointDataStream().GazePoint(onGaze);
        }

        public void stop()
        {
            this.host.DisableConnection();
        }

        private void onGaze(double x, double y, double timestamp)
        {
            this.gazeCount++;
            Point point = new Point((int)x, (int)y);
            IntPtr window = WindowFromPoint(point);
            uint processIdUint;
            GetWindowThreadProcessId(window, out processIdUint);
            int processId = (int) processIdUint;
            if (this.gazeCount % GAZE_THRESHOLD == 0)
            {
                long stamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                this.processTimestamps[processId] = stamp;
                this.lastGazedProcessId = processId;
                GazeEvent(this, EventArgs.Empty);
            }
        }
    }
}
