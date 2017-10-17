using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eyeflow.Events;
using Eyeflow.Util;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Eyeflow.Dispatchers
{
    public class WindowDispatcher : Dispatcher
    {
        private event EventHandler<WindowEventArgs> WindowEvent;
        private static Logger log = Logger.get(typeof(WindowDispatcher));

        private WinEventDelegate dele = null;


        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;



        public void start()
        {
            dele = new WinEventDelegate(onWindowEvent);
            IntPtr m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
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

        public void onWindowEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            log.debug("EVENT!!");
        }
    }
}
