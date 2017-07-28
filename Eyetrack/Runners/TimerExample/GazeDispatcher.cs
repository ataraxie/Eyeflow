using System;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using Tobii.Interaction;
using System.Runtime.InteropServices;

using System.Collections.Generic;

using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using Eyetrack.Runners;

namespace Eyetrack.Runners.TimerExample
{
    class GazeDispatcher
    {
        public event EventHandler<GazeEventArgs> GazeEvent;

        Host host;

        public void start()
        {
            this.host = new Host();
            this.host.Streams.CreateGazePointDataStream().GazePoint(onGaze);
        }

        public void stop()
        {
            this.host.DisableConnection();
        }

        private void onGaze(double x, double y, double timestamp)
        {
            GazeEvent(this, new GazeEventArgs(x, y, timestamp));
        }
    }
}
