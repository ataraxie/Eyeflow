using Eyeflow.Dispatchers;

namespace Eyeflow
{
    public abstract class Runner
    {
        abstract public void start(GazeDispatcher gazeDispatcher);
        abstract public void stop();
    }
}
