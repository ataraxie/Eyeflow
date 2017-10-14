using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eyeflow.Events;

namespace Eyeflow.Dispatchers
{
    public class WindowDispatcher : Dispatcher
    {
        private event EventHandler<WindowEventArgs> WindowEvent;

        public void start()
        {
            
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
