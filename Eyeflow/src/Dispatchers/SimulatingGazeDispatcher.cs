using System;
using System.Timers;
using Eyeflow.Util;

namespace Eyeflow.Dispatchers
{
    class SimulatingGazeDispatcher : GazeDispatcher
    {
        private event EventHandler<GazeEventArgs> GazeEvent;
        private Timer timer;
        private int tickCount = 0;
        private int timesCount = 0;
        private int x = 100;
        private int y = 0;

        public void start()
        {
            this.timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(onTimerTick);
            timer.Interval = 1000;
            timer.Enabled = true;
        }

        private void onTimerTick(object source, ElapsedEventArgs e)
        {
            this.tickCount++;
            this.timesCount++;
            if (this.tickCount < 10)
            {
                this.y += 100;
            }
            else if (this.tickCount < 25)
            {
                this.x += 100;
            }
            else if (this.tickCount < 33)
            {
                this.y -= 100;
            }
            else if (this.tickCount < 48)
            {
                this.x -= 100;
            }

            GazeEvent(this, new GazeEventArgs(x, y, GazeLib.getTimestamp()));
        }

        public void stop()
        {
            this.timer.Stop();
            this.timer.Dispose();
        }
        public void addEventHandler(EventHandler<GazeEventArgs> handler)
        {
            GazeEvent += handler;
        }

        private void onGaze(double x, double y, double timestamp)
        {
            GazeEvent(this, new GazeEventArgs(x, y, timestamp));
        }
    }
}
