using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Eyetrack.Runners;
//using Eyetrack.Runners.TobiiWelcomeExample;
using Eyetrack.Runners.WebsocketExample;
//using Eyetrack.Runners.GazeWindows;
//using Eyetrack.Runners.RectangleDrawer;
using Eyetrack.Runners.WindowHighlight;
using Eyetrack.Runners.WindowIdentifier;
using Eyetrack.Runners.WindowBlur;
//using Eyetrack.Runners.RectangleRemover;
//using Eyetrack.Runners.SocketExample;
using Eyetrack.Runners.AutoHotkeyTest;

namespace Eyetrack
{
    class Program
    {
        public static void Main(string[] args)
        {
            Runner runner;
            //runner = new TobiiGreeterRunner();
            //runner = new WebsocketRunner();
            //runner = new GazeWindowsRunner();
            //runner = new RectangleDrawerRunner();
            //runner = new WindowHighlightRunner();
            //runner = new WindowIdentifierRunner();
            //runner = new WindowBlurRunner();
            //runner = new RectangleRemover();
            //runner = new SocketRunner();
            //runner = new AutoHotkeyTestRunner();
            runner = new AhkWindowHighlightRunner();
            //runner = new AhkWindowHighlightRunner3();
            runner.run();
        }
    }
}
