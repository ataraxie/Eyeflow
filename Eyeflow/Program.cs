﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Eyeflow;
using Eyeflow.Runners;
using Eyeflow.Dispatchers;
using Eyeflow.Util;

namespace Eyeflow
{
    class Program
    {
        private static Logger log = Logger.get(typeof(Program));

        public static void Main(string[] args)
        {
            try
            {
                log.info("=== Eyeflow started ===");
                //GazeDispatcher gazeDispatcher = new SimulatingGazeDispatcher();
                GazeDispatcher gazeDispatcher = new RealGazeDispatcher();
                //Runner runner = new RectangleDrawerRunner();
                Runner runner = new AnimatingTimerRunner();
                runner.start(gazeDispatcher);
                gazeDispatcher.start();
                Console.ReadKey();
                runner.stop();
                gazeDispatcher.stop();
            } catch (Exception e)
            {
                log.error("Error occurred: {0}", e.ToString());
                WinLib.setTransparency255ForAllWindows();
            }

        }
    }
}
