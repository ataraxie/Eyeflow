using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Eyetrack.Runners.TimerExample
{
    class AnimatingTimerRunner : BaseTimerRunner
    {
        private static long TIME_THRESHOLD_MS = 5000;
        private Dictionary<IntPtr, Animation> runningAnimations = new Dictionary<IntPtr, Animation>();

        protected override void onTimerTick(object source, ElapsedEventArgs e)
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            foreach (IntPtr window in this.visibleWindows.ToList())
            {
                if (window != this.currentlyActiveWindow)
                {
                    string processName = WinLib.getProcessName(window);
                    long timeInactiveMs = now - this.windowGazeTimestamps[window];
                    if (timeInactiveMs > TIME_THRESHOLD_MS)
                    {
                        this.visibleWindows.Remove(window);
                        this.hiddenWindows.Add(window);
                        Animation animation = new Animation(window, processName, () => {
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
            if (this.runningAnimations.ContainsKey(window))
            {
                Animation animation = this.runningAnimations[window];
                if (animation != null)
                {
                    animation.stop();
                }
            }
        }

        private class Animation
        {
            private static int TIME_INTERVAL = 20;
            private static int TRANSPARENCY_INTERVAL = 10;
            private static int MIN_TRANSPARENCY = 50;

            private IntPtr window;
            private string processName;
            Func<bool> onStop;
            private int currentTransparency = 255;
            private Timer timer;

            private Dictionary<IntPtr, Timer> timers = new Dictionary<IntPtr, Timer>();

            public Animation(IntPtr window, string processName, Func<bool> onStop)
            {
                this.window = window;
                this.processName = processName;
                this.onStop = onStop;
                this.timer = new Timer();
            }

            public void start()
            {
                Console.WriteLine(this.processName + " - starting animation");
                this.timer.Elapsed += new ElapsedEventHandler(onFadeOutTimer);
                this.timer.Interval = TIME_INTERVAL;
                this.timer.Enabled = true;
            }

            public void stop()
            {
                Console.WriteLine(this.processName + " - stopping animation");
                this.onStop();
                this.timer.Stop();
                this.timer.Dispose();
            }

            private void onFadeOutTimer(object source, ElapsedEventArgs e)
            {
                this.currentTransparency -= TRANSPARENCY_INTERVAL;
                if (this.currentTransparency < MIN_TRANSPARENCY)
                {
                    this.currentTransparency = MIN_TRANSPARENCY;
                }
                WinLib.setTransparency(this.window, (byte)this.currentTransparency);
                if (this.currentTransparency == MIN_TRANSPARENCY)
                {
                    this.stop();
                }
            }
        }
    }
}
