using System;
using System.Drawing;
using System.ComponentModel;
using System.Text;

namespace Eyeflow.Util
{
    class GazeLib
    {
        private static Logger log = Logger.get(typeof(GazeLib));


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

        public static bool isTargetWindow(IntPtr window)
        {
            long timestamp = getTimestamp();
            string processName = WinLib.getProcessName(window);
            string windowTitle = WinLib.getWindowTitle(window);
            bool ret = !windowTitle.Contains("Program Manager") && !String.IsNullOrEmpty(windowTitle)
                && processName != "dwm"
                && !(processName == "explorer" && windowTitle == "Start" && !Config.Instance.enableForTaskBar);
            GazeLib.logProf(timestamp, "isTargetWindow");
            return ret;
        }

        public static void logProf(long timestampBegin, string methodName)
        {
            log.trace("PROF:" + methodName + ":" + (GazeLib.getTimestamp() - timestampBegin).ToString());
        }

        public static string objectDump(Object someObject)
        {
            StringBuilder b = new StringBuilder();
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(someObject))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(someObject);
                b.Append(String.Format("{0}={1};", name, value));
            }
            return b.ToString();
        }
    }
}
