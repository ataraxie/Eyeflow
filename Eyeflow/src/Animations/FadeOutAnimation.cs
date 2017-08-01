using System;
using System.Collections.Generic;
using System.Timers;
using Eyeflow.Util;

namespace Eyeflow.Animations
{
    public class FadeOutAnimation
    {
        private static Config config = Config.Instance;

        private IntPtr window;
        private string processName;
        Func<bool> onStop;
        private int currentTransparency = 255;
        private Timer timer;

        private Dictionary<IntPtr, Timer> timers = new Dictionary<IntPtr, Timer>();

        public FadeOutAnimation(IntPtr window, string processName, Func<bool> onStop)
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
            this.timer.Interval = Config.Instance.fadeOutAnimationTimeIntervalMs;
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
            this.currentTransparency -= config.fadeOutAnimationTransparencyInterval;
            if (this.currentTransparency < config.fadeOutAnimationMinTransparency)
            {
                this.currentTransparency = config.fadeOutAnimationMinTransparency;
            }
            WinLib.setTransparency(this.window, (byte)this.currentTransparency);
            if (this.currentTransparency == config.fadeOutAnimationMinTransparency)
            {
                this.stop();
            }
        }
    }
}
