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

namespace Eyetrack.Runners.WindowHighlight
{
    public class WindowHighlightRunner : Runner
    {
        private int gazeCount = 0;
        private Dictionary<Process, Rectangle> rectangles;

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

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(System.Drawing.Point p);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

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
            if (this.gazeCount % 100 == 0)
            {
                if (this.currentProcessId != processId)
                {
                    this.currentProcessId = processId;
                    Process processInView = Process.GetProcessById((int)processId);
                    Console.WriteLine(processInView.ProcessName);
                    SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) ^ WS_EX_LAYERED);
                    SetLayeredWindowAttributes(handle, 0, 255, LWA_ALPHA);

                    foreach (Process process in Process.GetProcesses())
                    {
                        if (process != processInView)
                        {
                            SetWindowLong(process.MainWindowHandle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) ^ WS_EX_LAYERED);
                            SetLayeredWindowAttributes(process.MainWindowHandle, 0, 100, LWA_ALPHA);
                        }
                    }
                }
            }

        }
    }
}