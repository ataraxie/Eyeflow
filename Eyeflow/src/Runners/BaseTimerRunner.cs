using System;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Timers;
using Eyeflow.Util;
using Eyeflow.Dispatchers;

namespace Eyeflow.Runners
{
    abstract class BaseTimerRunner : Runner
    {
        private static Logger log = Logger.get(typeof(BaseTimerRunner));
        private static Config config = Config.Instance;

        private Timer globalTimer;

        private GazeDispatcher gazeDispatcher;
        private int gazeCount = 0;

        protected Dictionary<IntPtr, long> windowGazeTimestamps;
        protected FixedSizeQueue<IntPtr> recentlyActiveWindows;

        protected IntPtr potentialNewWindow = IntPtr.Zero;
        protected int potentialNewWindowGazeTimes = 0;
        protected IntPtr currentlyActiveWindow;

        protected HashSet<IntPtr> visibleWindows = new HashSet<IntPtr>();
        protected HashSet<IntPtr> hiddenWindows = new HashSet<IntPtr>();

        public override void start(GazeDispatcher gazeDispatcher)
        {
            log.info("BaseTimerRunner started");
            this.windowGazeTimestamps = new Dictionary<IntPtr, long>();
            this.recentlyActiveWindows = new FixedSizeQueue<IntPtr>(Config.Instance.howManyActiveConcurrentWindows);
            hideAllTopLevelWindows();
            this.gazeDispatcher = gazeDispatcher;
            this.gazeDispatcher.addEventHandler(onGazeEvent);
            initTimer();
        }

        public override void stop()
        {
            this.globalTimer.Stop();
            onStop();
            showAllHiddenWindows();
        }

        protected abstract void onTimerTick(object source, ElapsedEventArgs e);
        protected abstract void onNewWindowGaze(IntPtr window);
        protected abstract void onStop();

        private void initTimer()
        {
            this.globalTimer = new Timer();
            this.globalTimer.Elapsed += new ElapsedEventHandler(onTimerTick);
            this.globalTimer.Interval = config.globalCheckTimerInterval;
            this.globalTimer.Enabled = true;
        }

        private void onGazeEvent(object sender, GazeEventArgs e)
        {
            this.gazeCount++;
            if (config.drawGazePositionRectangles)
            {
                GazeLib.drawRectangle((int)e.x, (int)e.y);
            }
            if (this.gazeCount % config.runOnEveryXGazeDispatch == 0)
            {
                Point point = new Point((int)e.x, (int)e.y);
                IntPtr windowAtGaze = WinLib.WindowFromPoint(point);
                IntPtr topLevelWindowAtGaze = WinLib.getTopLevelWindow(windowAtGaze);
                if (isTargetWindow(topLevelWindowAtGaze)
                    && topLevelWindowAtGaze != this.currentlyActiveWindow)
                {
                    forwardGazeIfGazedLongEnough(topLevelWindowAtGaze);
                }

            }
        }

        private void forwardGazeIfGazedLongEnough(IntPtr topLevelWindowAtGaze)
        {
            if (this.potentialNewWindow == topLevelWindowAtGaze)
            {
                this.potentialNewWindowGazeTimes++;
                if (this.potentialNewWindowGazeTimes > Config.Instance.howManyGazesRequiredForHighlight)
                {
                    this.currentlyActiveWindow = topLevelWindowAtGaze;
                    long stamp = GazeLib.getTimestamp();
                    this.windowGazeTimestamps[this.currentlyActiveWindow] = stamp;
                    this.recentlyActiveWindows.Enqueue(this.currentlyActiveWindow);
                    onNewWindowGaze(this.currentlyActiveWindow);
                }
            }
            else
            {
                this.potentialNewWindowGazeTimes = 0;
                this.potentialNewWindow = topLevelWindowAtGaze;
            }
        }

        private void logRecentlyQueue() // niu => remove later
        {
            if (this.recentlyActiveWindows.Count == 2)
            {
                IntPtr handle1 = this.recentlyActiveWindows.ToArray()[0];
                IntPtr handle2 = this.recentlyActiveWindows.ToArray()[1];
                log.warn(WinLib.getProcessName(handle1) + " - " + WinLib.getProcessName(handle2));
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

                log.info("Showing window::" + createWindowKey(window));
                WinLib.setTransparency(window, 255);
            }
        }

        private void hideWindow(IntPtr window)
        {
            if (isTargetWindow(window) && !this.hiddenWindows.Contains(window))
            {
                this.visibleWindows.Remove(window);
                this.hiddenWindows.Add(window);
                string processName = WinLib.getProcess(window).ProcessName;
                log.info("Hiding window::" + createWindowKey(window));
                WinLib.setTransparency(window, 50);
            }
        }

        private string createWindowKey(IntPtr window)
        {
            string processName = WinLib.getProcess(window).ProcessName;
            string windowTitle = WinLib.getWindowTitle(window);
            return processName + ":::" + windowTitle;
        }

        private bool isTargetWindow(IntPtr window)
        {
            string windowTitle = WinLib.getWindowTitle(window);
            return !windowTitle.Contains("Program Manager");
        }

    }
}