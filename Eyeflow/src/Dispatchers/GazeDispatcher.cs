using System;

namespace Eyeflow.Dispatchers
{
    public interface GazeDispatcher
    {
        void start();
        void stop();
        void addEventHandler(EventHandler<GazeEventArgs> handler);
    }
}
