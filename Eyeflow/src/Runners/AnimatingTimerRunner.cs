using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Eyeflow.Animations;
using Eyeflow.Util;

namespace Eyeflow.Runners
{
    class AnimatingTimerRunner : BaseTimerRunner
    {
        private static Logger log = Logger.get(typeof(AnimatingTimerRunner));
        private static Config config = Config.Instance;
        private Dictionary<IntPtr, FadeOutAnimation> runningAnimations = new Dictionary<IntPtr, FadeOutAnimation>();

        private void startAnimation(IntPtr window, string processName)
        {
            this.visibleWindows.Remove(window);
            FadeOutAnimation animation = new FadeOutAnimation(window, processName, () => {
                log.debug("stop callback invoked for window owned by: {0}", processName);
                this.runningAnimations.Remove(window);
                this.visibleWindows.Remove(window);
                return true;
            });
            this.runningAnimations[window] = animation;
            log.debug("hiding window owned by process {0} with animation", processName);
            animation.start();
        }

        private void stopAllAnimations()
        {
            foreach(FadeOutAnimation animation in runningAnimations.Values)
            {
                animation.stop();
            }
        }

        protected override void onTimerTick(object source, ElapsedEventArgs e)
        {
            long now = GazeLib.getTimestamp();
            foreach (IntPtr window in this.visibleWindows.ToList())
            {
                if (window != this.currentlyActiveWindow
                    && this.windowGazeTimestamps.ContainsKey(window))
                {
                    string processName = WinLib.getProcessName(window);
                    if (!this.recentlyActiveWindows.Contains(window))
                    {
                        long timeInactiveMs = now - this.windowGazeTimestamps[window];
                        if (timeInactiveMs > config.windowInactiveThresholdMs)
                        {
                            startAnimation(window, processName);
                        }
                    }
                    else
                    {
                        log.debug("Not hiding window {0} because it's in recently queue", processName);
                    }

                }
            }
        }

        protected override void onNewWindowGaze(IntPtr window)
        {
            showWindow(this.currentlyActiveWindow);
            if (this.runningAnimations.ContainsKey(window))
            {
                FadeOutAnimation animation = this.runningAnimations[window];
                if (animation != null)
                {
                    animation.stop();
                }
            }
        }

        protected override void onStop()
        {
            stopAllAnimations();
        }

        protected override void onSameWindowGaze(IntPtr window, long currentTimestamp)
        {
            if (!config.simulationMode && config.windowToForegroundOnGazeAfterMs >= 0)
            {
                long timeActiveMs = currentTimestamp - this.timeActivated;
                if (timeActiveMs > config.windowToForegroundOnGazeAfterMs)
                {
                    log.debug("window has been active for {0}ms => bringing to foreground");
                    WinLib.SetForegroundWindow(this.currentlyActiveWindow);
                }
            }
        }
    }
}
