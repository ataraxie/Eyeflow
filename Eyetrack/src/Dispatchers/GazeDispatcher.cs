using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyetrack.Dispatchers
{
    public interface GazeDispatcher
    {
        void start();
        void stop();
        void addEventHandler(EventHandler<GazeEventArgs> handler);
    }
}
