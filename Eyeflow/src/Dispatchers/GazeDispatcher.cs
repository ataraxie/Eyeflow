using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eyeflow.Events;

namespace Eyeflow.Dispatchers
{
    public interface GazeDispatcher : Dispatcher
    {
        void addEventHandler(EventHandler<GazeEventArgs> handler);
    }
}
