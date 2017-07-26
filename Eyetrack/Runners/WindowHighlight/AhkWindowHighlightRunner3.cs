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
    public class AhkWindowHighlightRunner3 : Runner
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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

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


        private static int GAZE_THRESHOLD = 10;
        private static String PATH = "Resources\\ahk\\functions.ahk"; // must be in bin/debug
        private static AutoHotkeyEngine AHK = AutoHotkeyEngine.Instance;

        private static List<string> IGNORED_PROCESSES = new List<string>(new string[] {
            /*"explorer",*/ "Eyetrack"
        });

        private int gazeCount = 0;
        private uint currentProcessId;
        private Process currentProcess;
        private List<Process> allProcesses = new List<Process>();
        private List<Process> nonActiveProcesses = new List<Process>();
        private HashSet<uint> currentlyBlurredProcesses = new HashSet<uint>();

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
            foreach (Process process in this.nonActiveProcesses)
            {
                AHK.ExecFunction("ShowWindow", process.Id.ToString());
            }
        }

        private void redraw(IntPtr targetWindow)
        {
            Console.WriteLine("Active: " + this.currentProcess.ProcessName);
            //this.currentlyBlurredProcesses.Remove(this.currentProcessId);
            SetForegroundWindow(targetWindow);
            //setTransparency(targetWindow);
            //AHK.ExecFunction("ShowActiveWindow");

            Console.WriteLine("Non-Active: " + getProcessNames(this.nonActiveProcesses));
            //foreach (Process process in this.nonActiveProcesses)
            //{
            //    uint processId = (uint) process.Id;
            //    if (!this.currentlyBlurredProcesses.Contains(processId)) {
            //        AHK.ExecFunction("HideWindow", process.Id.ToString());
            //        this.currentlyBlurredProcesses.Add(processId);
            //    }
            //}
        }

        private void setTransparency(IntPtr handle)
        {
            SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) | WS_EX_LAYERED);
            SetLayeredWindowAttributes(handle, 0, 200, 0x02);
        }

        private bool isTargetProcess(Process process)
        {
            bool ret = false;
            IntPtr handle = process.MainWindowHandle;
            if (handle != IntPtr.Zero && !IsIconic(handle) && IsWindowVisible(handle)
                && !IGNORED_PROCESSES.Contains(process.ProcessName))
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
            if (this.gazeCount % GAZE_THRESHOLD == 0)
            {
                if (this.currentProcessId != processId) {
                    setupProcesses(processId);
                    redraw(window);
                }
            }
        }

        private void setupProcesses(uint processId)
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
    }
}