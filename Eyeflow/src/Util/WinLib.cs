using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using System.Collections.Generic;

using Rectangle = System.Drawing.Rectangle;

namespace Eyeflow.Util
{
    class WinLib
    {
        private static Logger log = Logger.get(typeof(WinLib));

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(System.Drawing.Point p);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        public static IntPtr getTopLevelWindow(IntPtr window)
        {
            if (GetParent(window) != IntPtr.Zero)
            {
                do
                {
                    window = GetParent(window);
                } while (GetParent(window) != IntPtr.Zero);
            }

            return window;
        }

        public static Process getProcess(IntPtr window)
        {
            uint processIdUint;
            GetWindowThreadProcessId(window, out processIdUint);
            int processId = (int)processIdUint;
            Process process = Process.GetProcessById(processId);
            return process;
        }

        public static string getProcessName(IntPtr window)
        {
            return getProcess(window).ProcessName;
        }

        public static HashSet<IntPtr> getAllTopLevelWindows()
        {
            return new GetTopLevelWindowsAction(true).exec();
        }

        public static HashSet<IntPtr> getAllWindows()
        {
            return new GetTopLevelWindowsAction(false).exec();
        }

        private class GetTopLevelWindowsAction
        {
            private HashSet<IntPtr> windows = new HashSet<IntPtr>();
            private bool onlyTopLevel;

            public GetTopLevelWindowsAction(bool onlyTopLevel)
            {
                this.onlyTopLevel = onlyTopLevel;
            }

            public HashSet<IntPtr> exec()
            {
                EnumWindowsProc callback = new EnumWindowsProc(EnumWindowsCallback);
                EnumWindows(callback, IntPtr.Zero);
                return this.windows;
            }

            private bool EnumWindowsCallback(IntPtr window, IntPtr lParam)
            {
                if (!this.onlyTopLevel || WinLib.isTopLevelWindow(window))
                {
                    this.windows.Add(window);
                }
                return true;
            }
        }

        public static bool isTopLevelWindow(IntPtr window)
        {
            bool ret = false;
            Process process = WinLib.getProcess(window);
            if (window != IntPtr.Zero && !IsIconic(window) && IsWindowVisible(window))
            {
                Rectangle rect = new Rectangle();
                GetWindowRect(window, out rect);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    ret = true;
                }
            }
            return ret;
        }

        public static void setTransparency(IntPtr handle, byte value)
        {
            SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) | WS_EX_LAYERED);
            SetLayeredWindowAttributes(handle, 0, value, LWA_ALPHA);
        }

        public static void setTransparency255ForAllWindows()
        {
            log.debug("setTransparency255ForAllWindows()");
            foreach (IntPtr handle in getAllWindows()) {
                if (Logger.DEBUG_ENABLED)
                {
                    log.debug("Setting transparency to 255 for window {0}", getProcessName(handle));
                }
                setTransparency(handle, 255);
            }
        }

        public static string getWindowTitle(IntPtr handle)
        {
            const int numChars = 256;
            StringBuilder sb = new StringBuilder(numChars);
            GetWindowText(handle, sb, numChars);
            return sb.ToString();
        }

    }
}
