using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eyetrack.Dispatchers;

namespace Eyetrack
{
    public abstract class Runner
    {
        abstract public void start(GazeDispatcher gazeDispatcher);
        abstract public void stop();
    }
}
