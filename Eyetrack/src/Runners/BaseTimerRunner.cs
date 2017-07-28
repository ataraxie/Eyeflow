using System;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Timers;
using Eyetrack.Util;

namespace Eyetrack.Runners
{
    abstract class BaseTimerRunner : Runner
    {
        private static Config config = Config.Instance;

        private GazeDispatcher gazeDispatcher;
        private int gazeCount = 0;

        public Dictionary<IntPtr, long> windowGazeTimestamps = new Dictionary<IntPtr, long>();

        protected IntPtr currentlyActiveWindow;
        protected HashSet<IntPtr> visibleWindows = new HashSet<IntPtr>();
        protected HashSet<IntPtr> hiddenWindows = new HashSet<IntPtr>();

        public override void start(GazeDispatcher gazeDispatcher)
        {
            hideAllTopLevelWindows();
            this.gazeDispatcher = gazeDispatcher;
            this.gazeDispatcher.addEventHandler(onGazeEvent);
            initTimer();
        }

        public override void stop()
        {
            showAllHiddenWindows();
        }

        protected abstract void onTimerTick(object source, ElapsedEventArgs e);
        protected abstract void onNewWindowGaze(IntPtr window);

        private void initTimer()
        {
            Timer timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(onTimerTick);
            timer.Interval = config.globalCheckTimerInterval;
            timer.Enabled = true;
        }

        private void onGazeEvent(object sender, GazeEventArgs e)
        {
            this.gazeCount++;
            if (this.gazeCount % config.runOnEveryXGazeDispatch == 0)
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
                showWindow(window);
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
            return !config.ignoredProcesses.Contains(process.ProcessName);
        }

    }
}