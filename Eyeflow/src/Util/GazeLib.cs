using System;
using System.Drawing;

namespace Eyeflow.Util
{
    class GazeLib
    {
        public static int getTransparencyByTimeInactive(long timeInactiveMs)
        {
            int transparency;
            int upperLimitTransparency = 255;
            int lowerLimitTransparency = 55;
            int upperLimitTime = 60000;
            int lowerLimitTime = 1000;
            if (timeInactiveMs < lowerLimitTime)
            {
                transparency = upperLimitTransparency;
            } else if (timeInactiveMs > upperLimitTime)
            {
                transparency = lowerLimitTransparency;
            } else
            {
                int gapTransparency = upperLimitTransparency - lowerLimitTransparency;
                int gapTime = upperLimitTime - lowerLimitTime;
                double ratioTime = (double) timeInactiveMs / (double) gapTime;
                double fragmentTransparency = ratioTime * gapTransparency;
                transparency = upperLimitTransparency - (int) fragmentTransparency;
                if (transparency < lowerLimitTransparency) // this should never happen but just to make sure
                {
                    transparency = lowerLimitTransparency;
                }
            }
            return transparency;
        }

        public static long getTimestamp()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var now = DateTimeOffset.Now.ToUniversalTime();
            var timestamp = (now - epoch).TotalMilliseconds;
            return (long) timestamp;
        }

        public static void drawRectangle(int x, int y)
        {
            Point point = new Point(x, y);
            Pen pen = new Pen(Config.Instance.gazePositionColor, 2);
            SolidBrush brush = new SolidBrush(Config.Instance.gazePositionColor);
            IntPtr desktopPtr = WinLib.GetDC(IntPtr.Zero);
            Graphics g = Graphics.FromHdc(desktopPtr);
            Rectangle rect = new Rectangle(point, new Size(30, 30));
            g.FillRectangle(brush, rect);
            g.DrawRectangle(pen, rect);
        }

        public void log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
