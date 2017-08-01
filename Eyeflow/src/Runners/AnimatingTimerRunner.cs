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
        private static Config config = Config.Instance;
        private Dictionary<IntPtr, FadeOutAnimation> runningAnimations = new Dictionary<IntPtr, FadeOutAnimation>();

        protected override void onTimerTick(object source, ElapsedEventArgs e)
        {
            long now = GazeLib.getTimestamp();
            foreach (IntPtr window in this.visibleWindows.ToList())
            {
                if (window != this.currentlyActiveWindow
                    && this.windowGazeTimestamps.ContainsKey(window))
                {
                    string processName = WinLib.getProcessName(window);
                    long timeInactiveMs = now - this.windowGazeTimestamps[window];
                    if (timeInactiveMs > config.windowInactiveThresholdMs)
                    {
                        this.visibleWindows.Remove(window);
                        this.hiddenWindows.Add(window);
                        FadeOutAnimation animation = new FadeOutAnimation(window, processName, () => {
                            Console.WriteLine(processName + " - stop callback invoked");
                            this.runningAnimations.Remove(window);
                            return true;
                        });
                        this.runningAnimations[window] = animation;
                        animation.start();
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
