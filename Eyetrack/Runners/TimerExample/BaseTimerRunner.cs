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

namespace Eyetrack.Runners.TimerExample
{
    abstract class BaseTimerRunner : Runner
    {
        private static List<string> IGNORED_PROCESSES = new List<string>(new string[] {
            "explorer", "Eyetrack"
        });

        private static long TIMER_INTERVAL = 1000;
        private static int GAZE_THRESHOLD = 10;

        private GazeDispatcher gazeDispatcher;
        private int gazeCount = 0;

        public Dictionary<IntPtr, long> windowGazeTimestamps = new Dictionary<IntPtr, long>();

        protected IntPtr currentlyActiveWindow;
        protected HashSet<IntPtr> visibleWindows = new HashSet<IntPtr>();
        protected HashSet<IntPtr> hiddenWindows = new HashSet<IntPtr>();

        public override void run()
        {
            hideAllTopLevelWindows();
            this.gazeDispatcher = new GazeDispatcher();
            this.gazeDispatcher.GazeEvent += onGazeEvent;
            this.gazeDispatcher.start();
            initTimer();
            Console.ReadKey();
            this.gazeDispatcher.stop();
            showAllHiddenWindows();
        }

        protected abstract void onTimerTick(object source, ElapsedEventArgs e);
        protected abstract void onNewWindowGaze(IntPtr window);

        private void initTimer()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(onTimerTick);
            aTimer.Interval = TIMER_INTERVAL;
            aTimer.Enabled = true;
        }

        private void onGazeEvent(object sender, GazeEventArgs e)
        {
            this.gazeCount++;
            if (this.gazeCount % GAZE_THRESHOLD == 0)
            {
                Point point = new Point((int)e.x, (int)e.y);
                IntPtr windowAtGaze = WinLib.WindowFromPoint(point);
                this.currentlyActiveWindow = WinLib.getTopLevelWindow(windowAtGaze);
                long stamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                this.windowGazeTimestamps[this.currentlyActiveWindow] = stamp;
                onNewWindowGaze(this.currentlyActiveWindow);
            }
        }

        private void hideAllTopLevelWindows()
        {
            foreach (IntPtr window in WinLib.getAllTopLevelWindows()) {
                hideWindow(window);
            }
        }

        private void showAllHiddenWindows()
        {
            foreach (IntPtr window in this.hiddenWindows.ToList())
            {
                hideWindow(window);
            }
        }

        protected void showWindow(IntPtr window)
        {
            if (isTargetWindow(window) && !this.visibleWindows.Contains(window))
            {
                this.visibleWindows.Add(window);
                this.hiddenWindows.Remove(window);
                Console.WriteLine("Show process: " + WinLib.getProcess(window).ProcessName);
                WinLib.setTransparency(window, 255);
            }
        }

        private void hideWindow(IntPtr window)
        {
            if (isTargetWindow(window) && !this.hiddenWindows.Contains(window))
            {
                this.visibleWindows.Remove(window);
                this.hiddenWindows.Add(window);
                Console.WriteLine("Hiding process: " + WinLib.getProcess(window).ProcessName);
                WinLib.setTransparency(window, 50);
            }
        }

        private bool isTargetWindow(IntPtr window)
        {
            Process process = WinLib.getProcess(window);
            return !IGNORED_PROCESSES.Contains(process.ProcessName);
        }

    }
}