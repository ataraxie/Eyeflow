using System;
using Eyetrack.Dispatchers;
using Eyetrack.Util;
using System.Drawing;

namespace Eyetrack.Runners
{
    class RectangleDrawerRunner : Runner
    {
        private GazeDispatcher gazeDispatcher;

        public override void start(GazeDispatcher gazeDispatcher)
        {
            this.gazeDispatcher = gazeDispatcher;
            this.gazeDispatcher.addEventHandler(onGazeEvent);
        }

        public override void stop()
        {
            
        }

        private void onGazeEvent(object sender, GazeEventArgs e)
        {
            Console.WriteLine("NEW GAZE 2");
            Point point = new Point((int) e.x, (int) e.y);
            Pen pen = new Pen(Color.Beige, 2);
            IntPtr desktopPtr = WinLib.GetDC(IntPtr.Zero);
            Graphics g = Graphics.FromHdc(desktopPtr);
            Rectangle rect2 = new Rectangle(point, new Size(50, 50));
            g.DrawRectangle(pen, rect2);
        }
    }
}
