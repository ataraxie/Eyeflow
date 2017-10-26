using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Eyeflow;
using Eyeflow.Runners;
using Eyeflow.Dispatchers;

using Eyeflow.Util;

namespace Eyeflow
{
    class EyeflowApp
    {
        private static Logger log = Logger.get(typeof(EyeflowApp));

        private Runner runner;
        private GazeDispatcher gazeDispatcher;
        private WindowDispatcher windowDispatcher;

        public void execute()
        {
            log.info("=== EYEFLOW STARTED - WELCOME! ===");
            log.info("CONFIG: " + Config.Instance.ToString());
            gazeDispatcher = new RealGazeDispatcher();
            windowDispatcher = new WindowDispatcher();
            if (Config.Instance.dataCollectionMode)
            {   
                runner = new DataCollectionRunner(gazeDispatcher, windowDispatcher);
                runner.start();
                windowDispatcher.start();
            }
            else
            {
                runner = new AnimatingTimerRunner(gazeDispatcher);
                runner.start();
            }

            if (Config.Instance.enableEyetracking)
            {
                gazeDispatcher.start();
            }
        }

        public void exit()
        {
            if (!Config.Instance.dataCollectionMode && ! Config.Instance.simulationMode)
            {
                WinLib.setTransparency255ForAllWindows();
            }
            runner.stop();
            if (Config.Instance.enableEyetracking)
            {
                gazeDispatcher.stop();
            }
        }

    }
}
