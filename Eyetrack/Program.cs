using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Eyetrack;
using Eyetrack.Runners;
using Eyetrack.Dispatchers;

namespace Eyetrack
{
    class Program
    {
        public static void Main(string[] args)
        {
            Config.init();
            GazeDispatcher gazeDispatcher = new SimulatingGazeDispatcher();
            Runner timerRunner = new RectangleDrawerRunner();
            timerRunner.start(gazeDispatcher);
            gazeDispatcher.start();
            Console.ReadKey();
            timerRunner.stop();
            gazeDispatcher.stop();
        }
    }
}
