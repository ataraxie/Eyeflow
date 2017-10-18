using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyeflow.Dispatchers
{
    public interface Dispatcher
    {
        void start();
        void stop();
    }
}
