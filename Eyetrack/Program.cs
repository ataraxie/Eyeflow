using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eyetrack.Runners;
using Eyetrack.Runners.TobiiWelcomeExample;
using Eyetrack.Runners.WebsocketExample;
using Eyetrack.Runners.GazeWindows;
using Eyetrack.Runners.RectangleDrawer;
using Eyetrack.Runners.WindowHighlight;
using Eyetrack.Runners.WindowIdentifier;

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
            runner = new WindowIdentifierRunner();
            runner.run();
        }
    }
}
