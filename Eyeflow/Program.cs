using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Eyeflow;
using Eyeflow.Runners;
using Eyeflow.Dispatchers;

namespace Eyeflow
{
    class Program
    {
        public static void Main(string[] args)
        {
            Config.init();
            //GazeDispatcher gazeDispatcher = new SimulatingGazeDispatcher();
            GazeDispatcher gazeDispatcher = new RealGazeDispatcher();
            //Runner runner = new RectangleDrawerRunner();
            Runner runner = new AnimatingTimerRunner();
            runner.start(gazeDispatcher);
            gazeDispatcher.start();
            Console.ReadKey();
            runner.stop();
            gazeDispatcher.stop();
        }
    }
}
