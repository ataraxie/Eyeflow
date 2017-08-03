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
        protected long potentialNewWindowFirstGazeTime = 0;
        protected IntPtr currentlyActiveWindow;
        protected long timeActivated = 0;

        protected HashSet<IntPtr> visibleWindows = new HashSet<IntPtr>();
        protected HashSet<IntPtr> hiddenWindows = new HashSet<IntPtr>();

        protected abstract void onTimerTick(object source, ElapsedEventArgs e);
        protected abstract void onNewWindowGaze(IntPtr window);
        protected abstract void onSameWindowGaze(IntPtr window, long currentTimestamp);
        protected abstract void onStop();

        public override void start(GazeDispatcher gazeDispatcher)
        {
            log.debug("BaseTimerRunner started");
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
                long stamp = GazeLib.getTimestamp();
                this.windowGazeTimestamps[this.currentlyActiveWindow] = stamp;
                Point point = new Point((int)e.x, (int)e.y);
                IntPtr windowAtGaze = WinLib.WindowFromPoint(point);
                IntPtr topLevelWindowAtGaze = WinLib.getTopLevelWindow(windowAtGaze);
                if (isTargetWindow(topLevelWindowAtGaze) )
                {
                    if (topLevelWindowAtGaze != this.currentlyActiveWindow)
                    {
                        forwardGazeIfGazedLongEnough(topLevelWindowAtGaze, stamp);
                    }
                    else
                    {
                        onSameWindowGaze(topLevelWindowAtGaze, stamp);
                    }
                }
            }
        }

        private void forwardGazeIfGazedLongEnough(IntPtr topLevelWindowAtGaze, long timestamp)
        {
            if (this.potentialNewWindow == topLevelWindowAtGaze)
            {
                if (timestamp - this.potentialNewWindowFirstGazeTime > config.gazeTimeRequiredForHighlightMs)
                {
                    this.currentlyActiveWindow = topLevelWindowAtGaze;
                    this.timeActivated = timestamp;
                    this.recentlyActiveWindows.Enqueue(this.currentlyActiveWindow);
                    onNewWindowGaze(this.currentlyActiveWindow);
                }
            }
            else
            {
                this.potentialNewWindowFirstGazeTime = timestamp;
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
            if (isTargetWindow(window))
            {
                if (!this.visibleWindows.Contains(window))
                {
                    if (!config.simulationMode)
                    {
                        this.visibleWindows.Add(window);
                        this.hiddenWindows.Remove(window);
                        WinLib.setTransparency(window, 255);
                    }
                    log.info("SHOW_HIDDEN:::" + createWindowKey(window));
                } else
                {
                    log.info("SHOW_VISIBLE:::" + createWindowKey(window));
                }
            }
        }

        private void hideWindow(IntPtr window)
        {
            if (isTargetWindow(window) && !this.hiddenWindows.Contains(window))
            {
                if (!config.simulationMode)
                {
                    this.visibleWindows.Remove(window);
                    this.hiddenWindows.Add(window);
                    WinLib.setTransparency(window, 50);
                }
                log.debug("HIDE:::" + createWindowKey(window));
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