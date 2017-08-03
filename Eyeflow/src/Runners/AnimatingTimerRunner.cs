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
            this.hiddenWindows.Add(window);
            FadeOutAnimation animation = new FadeOutAnimation(window, processName, () => {
                log.debug("stop callback invoked for window owned by: {0}", processName);
                this.runningAnimations.Remove(window);
                return true;
            });
            this.runningAnimations[window] = animation;
            log.info("hiding window owned by process {0} with animation", processName);
            animation.start();
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
                        log.info("Not hiding window {0} because it's in recently queue", processName);
                    }

                }
            }
        }

        protected override void onNewWindowGaze(IntPtr window)
        {
            showWindow(this.currentlyActiveWindow);
            if (config.windowToForegroundOnGaze)
            {
                WinLib.SetForegroundWindow(this.currentlyActiveWindow);
            }
            if (this.runningAnimations.ContainsKey(window))
            {
                FadeOutAnimation animation = this.runningAnimations[window];
                if (animation != null)
                {
                    animation.stop();
                }
            }
        }


    }
}
