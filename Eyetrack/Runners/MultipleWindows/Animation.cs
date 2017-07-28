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
    class Animation
    {
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;


        private int currentTransparency = 255;
        private IntPtr window;

        public Animation(IntPtr window)
        {
            this.window = window;
        }


        public void start()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(onFadeOutTimer);
            aTimer.Interval = 100;
            aTimer.Enabled = true;
        }

        private void onFadeOutTimer(object source, ElapsedEventArgs e)
        {
            this.currentTransparency -= 30;
            setTransparency(this.window, (byte)this.currentTransparency);
        }

        private void setTransparency(IntPtr handle, byte value)
        {
            SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) | WS_EX_LAYERED);
            SetLayeredWindowAttributes(handle, 0, value, LWA_ALPHA);
        }


    }
}
