using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Eyetrack.Runners.TimerExample
{
    class CalculatingTimerRunner : BaseTimerRunner
    {
        protected override void onNewWindowGaze(IntPtr window)
        {
            showWindow(this.currentlyActiveWindow);
        }

        protected override void onTimerTick(object source, ElapsedEventArgs e)
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            foreach (IntPtr window in this.visibleWindows.ToList())
            {
                if (window != this.currentlyActiveWindow)
                {
                    long timeInactive = now - this.windowGazeTimestamps[window];
                    int transparency = GazeLib.getTransparencyByTimeInactive(timeInactive);
                    Console.WriteLine("Setting transparency " + WinLib.getProcessName(window) + ": timeInactive=" + timeInactive + ", transparency=" + transparency);
                    WinLib.setTransparency(window, (byte)transparency);
                }
            }
        }
    }
}
