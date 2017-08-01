using System;

namespace Eyetrack.Dispatchers
{
    public interface GazeDispatcher
    {
        void start();
        void stop();
        void addEventHandler(EventHandler<GazeEventArgs> handler);
    }
}
