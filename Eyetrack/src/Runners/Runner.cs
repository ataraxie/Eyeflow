using Eyetrack.Dispatchers;

namespace Eyetrack
{
    public abstract class Runner
    {
        abstract public void start(GazeDispatcher gazeDispatcher);
        abstract public void stop();
    }
}
