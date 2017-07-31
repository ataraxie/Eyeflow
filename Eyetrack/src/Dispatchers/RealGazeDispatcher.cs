using System;
using Tobii.Interaction;

namespace Eyetrack.Dispatchers
{
    public class RealGazeDispatcher : GazeDispatcher
    {
        private event EventHandler<GazeEventArgs> GazeEvent;

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
