using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eyeflow.Events;
using Eyeflow.Util;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Eyeflow.Dispatchers
{
    public class WindowDispatcher : Dispatcher
    {
        private event EventHandler<WindowEventArgs> WindowEvent;
        private static Logger log = Logger.get(typeof(WindowDispatcher));

        public void start()
        {
            WindowEvent(this, new WindowEventArgs());
        }

        public void stop()
        {
            
        }

        public void addEventHandler(EventHandler<WindowEventArgs> handler)
        {
            WindowEvent += handler;
        }

        private void onWindowEvent()
        {
            WindowEvent(this, new WindowEventArgs());
        }
    }
}
