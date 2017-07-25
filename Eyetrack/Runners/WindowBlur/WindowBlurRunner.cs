using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using System.Drawing;
using Tobii.Interaction;

using System.Collections.Generic;

using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using Eyetrack.Runners;
using System.Threading;

//using Overlay.NET.Common;
//using Overlay.NET.Directx;


namespace Eyetrack.Runners.WindowBlur
{
    class WindowBlurRunner : Runner
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);


        private void setTransparency(IntPtr handle, byte value)
        {
            SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) ^ WS_EX_LAYERED);
            SetLayeredWindowAttributes(handle, 0, value, LWA_ALPHA);
            //DirectXOverlayWindow window = new DirectXOverlayWindow(handle, false);
            //int brush = window.Graphics.CreateBrush(0x7FFF0000);
            //window.Graphics.BeginScene();
            //window.Graphics.ClearScene();
            //window.Graphics.DrawCircle(50, 50, 35, 2, brush);
        }

        public override void run()
        {
            string input = null;
            while (true)
            {
                Console.WriteLine("Enter process name:");
                input = Console.ReadLine();
                if (input == "q")
                {
                    break;
                }
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(input);
                if (processes.Length == 0)
                {
                    Console.WriteLine("No such process");
                }
                else
                {
                    Console.WriteLine("Enter value:");
                    string input2 = Console.ReadLine();
                    setTransparency(processes[0].MainWindowHandle, (byte) Int32.Parse(input2));
                }
            }
        }
    }
}
