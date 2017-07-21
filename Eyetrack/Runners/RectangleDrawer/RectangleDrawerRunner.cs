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

namespace Eyetrack.Runners.RectangleDrawer
{
    public class RectangleDrawerRunner : Runner
    {
        private int gazeCount;

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        public override void run()
        {
            this.gazeCount = 0;
            var host = new Host();
            var gazePointDataStream = host.Streams.CreateGazePointDataStream();
            gazePointDataStream.GazePoint(onGaze);
            Console.ReadKey();
            host.DisableConnection();
        }

        private void onGaze(double x, double y, double timestamp)
        {

            this.gazeCount++;
            if (this.gazeCount % 50 == 0)
            {
                Point point = new Point((int)x, (int)y);
                Pen pen = new Pen(Color.Black, 2);
                IntPtr desktopPtr = GetDC(IntPtr.Zero);
                Graphics g = Graphics.FromHdc(desktopPtr);
                Rectangle rect2 = new Rectangle(point, new Size(100, 100));
                g.DrawRectangle(pen, rect2);
            }
        }
    }
}