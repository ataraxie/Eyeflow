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

namespace Eyetrack.Runners.GazeWindows
{
    public class GazeWindowsRunner : Runner
    {
        static int gazeCount = 0;
        static Dictionary<Process, Rectangle> rectangles;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool IsIconic(IntPtr handle);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out NativeRect nativeRect);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

        private const int HWND_TOPMOST = -1;
        private const int HWND_NOTOPMOST = -2;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public override void run()
        {
            rectangles = GetWindowRectangles();
            var host = new Host();
            var gazePointDataStream = host.Streams.CreateGazePointDataStream();
            gazePointDataStream.GazePoint(onGaze);
            Console.ReadKey();
            host.DisableConnection();
        }

        static void onGaze(double x, double y, double timestamp)
        {
            var coordString = x + "/" + y;
            gazeCount++;
            Point point = new Point((int)x, (int)y);
            if (gazeCount % 200 == 0)
            {
                Pen pen = new Pen(Color.Black, 2);
                IntPtr desktopPtr = GetDC(IntPtr.Zero);
                Graphics g = Graphics.FromHdc(desktopPtr);
                Rectangle rect2 = new Rectangle(point, new Size(100, 100));
                g.DrawRectangle(pen, rect2);
                //SolidBrush b = new SolidBrush(Color.White);
                //g.FillRectangle(b, new Rectangle(0, 0, 100, 100));


                foreach (Process process in rectangles.Keys)
                {
                    Rectangle rect = rectangles[process];
                    if (rect.Contains(point))
                    {
                        Console.WriteLine(process.ProcessName);
                    }
                }
            }

        }

        private static Dictionary<Process, Rectangle> GetWindowRectangles()
        {
            Dictionary<Process, Rectangle> rectangles = new Dictionary<Process, Rectangle>();
            foreach (Process process in System.Diagnostics.Process.GetProcesses())
            {
                var windowHandle = process.MainWindowHandle;
                NativeRect nativeRect = new NativeRect();
                if (windowHandle != IntPtr.Zero && !IsIconic(process.MainWindowHandle) && IsWindowVisible(process.MainWindowHandle)
                    && GetWindowRect(windowHandle, out nativeRect))
                {
                    Rectangle rect = new Rectangle
                    {
                        X = nativeRect.Left,
                        Y = nativeRect.Top,
                        Width = nativeRect.Right,
                        Height = nativeRect.Bottom
                    };
                    rectangles.Add(process, rect);
                }
            }
            return rectangles;
        }
    }
}