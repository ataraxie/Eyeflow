using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyetrack.Runners.TimerExample
{
    class GazeEventArgs : EventArgs
    {
        public double x;
        public double y;
        public double timestamp;

        public GazeEventArgs(double x, double y, double timestamp)
        {
            this.x = x;
            this.y = y;
            this.timestamp = timestamp;
        }
    }
}
