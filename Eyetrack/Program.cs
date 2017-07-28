using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Eyetrack;
using Eyetrack.Runners;

namespace Eyetrack
{
    class Program
    {
        public static void Main(string[] args)
        {
            Config.init();
            GazeDispatcher gazeDispatcher = new GazeDispatcher();
            Runner timerRunner = new AnimatingTimerRunner();
            timerRunner.start(gazeDispatcher);
            gazeDispatcher.start();
            Console.ReadKey();
            timerRunner.stop();
            gazeDispatcher.stop();
        }
    }
}
