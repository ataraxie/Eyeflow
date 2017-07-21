using Eyetrack.Runners.TobiiWelcomeExample;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Tobii.Interaction;

namespace Eyetrack.Runners.TobiiWelcomeExample
{
    public class TobiiGreeterRunner : Runner
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out NativeRect nativeRect);

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
            var handle = Process.GetCurrentProcess().MainWindowHandle;
            var windowBounds = GetWindowBounds(handle);

            var greeter = new Greeter(
                handle.ToString(),
                windowBounds,
                () => Console.WriteLine("Welcome!"),
                () => Console.WriteLine("Goodbye!"));

            Console.ReadKey();
        }

        private static Rectangle GetWindowBounds(IntPtr windowHandle)
        {
            NativeRect nativeNativeRect;
            if (GetWindowRect(windowHandle, out nativeNativeRect))
                return new Rectangle
                {
                    X = nativeNativeRect.Left,
                    Y = nativeNativeRect.Top,
                    Width = nativeNativeRect.Right,
                    Height = nativeNativeRect.Bottom
                };

            return new Rectangle(0d, 0d, 1000d, 1000d);
        }
    }
}
