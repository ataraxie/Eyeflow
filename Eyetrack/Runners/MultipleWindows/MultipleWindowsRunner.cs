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
using System.Timers;
using AutoHotkey.Interop;
using System.Threading;
using System.Timers;


namespace Eyetrack.Runners.MultipleWindows
{
    class MultipleWindowsRunner : Runner
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool IsIconic(IntPtr handle);

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(System.Drawing.Point p);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);


        private readonly static String PATH = "Resources\\ahk\\functions.ahk"; // must be in bin/debug
        private readonly static AutoHotkeyEngine AHK = AutoHotkeyEngine.Instance;

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        public override void run()
        {
            AHK.LoadFile(PATH);
            int value = 50;
            int exstyle;
            Point point = new Point(1200, 300);
            IntPtr win = WindowFromPoint(point);
            win = getTopLevelWindow(win);
            //IntPtr win = Process.GetProcessesByName("notepad").FirstOrDefault().MainWindowHandle;

            Process process = getProcessForWindow(win);
            Console.WriteLine(process.ProcessName);
            //exstyle = GetWindowLong(window, GWL_EXSTYLE);

            //SetWindowLong(window, GWL_EXSTYLE, GetWindowLong(window, GWL_EXSTYLE) ^ WS_EX_LAYERED);
            //SetLayeredWindowAttributes(window, 0, 50, LWA_ALPHA);

            //SetWindowLong(window, GWL_EXSTYLE, exstyle ^ WS_EX_LAYERED);
            //SetLayeredWindowAttributes(window, 0, 50, LWA_ALPHA);
            //SetWindowLong(window, GWL_EXSTYLE, GetWindowLong(window, GWL_EXSTYLE) ^ WS_EX_LAYERED);
            //SetLayeredWindowAttributes(window, 0, 50, LWA_ALPHA);

            //AHK.ExecFunction("ShowWindow", process.Id.ToString());
            setTransparency(win, 50);
            Thread.Sleep(1000);
            //Console.ReadKey();
            setTransparency(win, 255);
            Thread.Sleep(1000);
            //Console.ReadKey();
            setTransparency(win, 50);
            Thread.Sleep(1000);
            //Console.ReadKey();
            setTransparency(win, 255);
            Thread.Sleep(1000);

            Animation animation = new Animation(win);
            animation.start();
            Console.ReadKey();
        }

        private void setTransparency(IntPtr handle, byte value)
        {
            SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) |  WS_EX_LAYERED);
            SetLayeredWindowAttributes(handle, 0, value, LWA_ALPHA);
        }

        private Process getProcessForWindow(IntPtr window)
        {
            uint processIdUint;
            GetWindowThreadProcessId(window, out processIdUint);
            int processId = (int)processIdUint;
            Process process = Process.GetProcessById(processId);
            return process;
        }

        private IntPtr getTopLevelWindow(IntPtr window)
        {
            do
            {
                window = GetParent(window);
            } while (GetParent(window) != IntPtr.Zero);

            return window;
        }
    }
}
