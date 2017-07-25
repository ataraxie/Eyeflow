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

namespace Eyetrack.Runners.WindowHighlight
{
    public class WindowHighlightRunner : Runner
    {
        private int gazeCount = 0;
        private Dictionary<System.Diagnostics.Process, Rectangle> rectangles;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool IsIconic(IntPtr handle);

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

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

        private uint currentProcessId;
        private Process currentProcess;
        private List<Process> allProcesses = new List<Process>();
        private List<Process> nonActiveProcesses = new List<Process>();


        public override void run()
        {
            var host = new Host();
            var gazePointDataStream = host.Streams.CreateGazePointDataStream();
            gazePointDataStream.GazePoint(onGaze);
            Console.ReadKey();
            host.DisableConnection();
        }

        private void redraw()
        {
            Console.WriteLine("Active: " + this.currentProcess.ProcessName);
            Console.WriteLine("Non-Active: " + getProcessNames(this.nonActiveProcesses));
            setTransparency(this.currentProcess.MainWindowHandle, 255);
            foreach (Process process in this.nonActiveProcesses)
            {
                setTransparency(process.MainWindowHandle, 255);
            }
        }

        private string getProcessNames(List<Process> processes)
        {
            List<string> names = new List<string>();
            foreach (Process process in this.nonActiveProcesses)
            {
                names.Add(process.ProcessName);
            }
            return String.Join(", ", names.ToArray());
        }

        private void setTransparency(IntPtr handle, byte value)
        {
            SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) ^ WS_EX_LAYERED);
            SetLayeredWindowAttributes(handle, 0, value, LWA_ALPHA);
        }

        private bool isTargetProcess(Process process)
        {
            bool ret = false;
            IntPtr handle = process.MainWindowHandle;
            if (handle != IntPtr.Zero && !IsIconic(handle) && IsWindowVisible(handle)
                && process.ProcessName != "explorer")
            {
                Rectangle rect = new Rectangle();
                GetWindowRect(handle, out rect);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    ret = true;
                }
            }
            return ret;
        }

        private void onGaze(double x, double y, double timestamp)
        {
            this.gazeCount++;
            Point point = new Point((int)x, (int)y);
            IntPtr window = WindowFromPoint(point);
            uint processId;
            GetWindowThreadProcessId(window, out processId);
            if (this.gazeCount % 10 == 0)
            {
                if (this.currentProcessId != processId)
                {
                    this.currentProcessId = processId;
                    this.currentProcess = Process.GetProcessById((int)processId);
                    this.nonActiveProcesses.Clear();
                    foreach (Process process in Process.GetProcesses())
                    {
                        IntPtr handle = process.MainWindowHandle;
                        if (this.currentProcessId != process.Id && isTargetProcess(process))
                        {
                            this.nonActiveProcesses.Add(process);
                        }
                    }

                    redraw();
                }
            }
            //if (this.gazeCount % 10 == 0)
            //{
            //    if (this.activeWindow != window)
            //    {
            //        this.activeWindow = window;
            //        foreach (Process process in Process.GetProcesses())
            //        {
            //            IntPtr handle = process.MainWindowHandle;
            //            this.allWindows.Add(handle);
            //            if (this.activeWindow != handle)
            //            {
            //                this.nonActiveWindows.Add(handle);
            //            } else
            //            {
            //                this.activeProcess = process;
            //            }
            //        }
            //        Console.WriteLine("Active: " + this.activeProcess.ProcessName);
            //    }
            //}
        }
    }
}