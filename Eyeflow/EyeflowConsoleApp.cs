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
    class EyeflowConsoleApp
    {
        private static Logger log = Logger.get(typeof(EyeflowConsoleApp));

        private static Runner runner;
        private static GazeDispatcher dispatcher;
        private static WinLib.HandlerRoutine consoleHandler;

        public static void execute()
        {
            handleConsole();
            log.info("=== EYEFLOW STARTED - WELCOME! ===");
            log.info("CONFIG: " + Config.Instance.ToString());
            //GazeDispatcher gazeDispatcher = new SimulatingGazeDispatcher();
            dispatcher = new RealGazeDispatcher();
            runner = new AnimatingTimerRunner();
            runner.start(dispatcher);
            dispatcher.start();
            Console.ReadKey();
            log.info("=== SHUTDOWN BY KEYPRESS ===");
            exit();
        }

        private static void exit()
        {
            WinLib.setTransparency255ForAllWindows();
            runner.stop();
            dispatcher.stop();
        }

        private static void handleConsole()
        {
            consoleHandler = new WinLib.HandlerRoutine(ConsoleCtrlCheck);
            WinLib.SetConsoleCtrlHandler(consoleHandler, true);
        }

        private static bool ConsoleCtrlCheck(WinLib.CtrlTypes ctrlType)
        {
            if (ctrlType == WinLib.CtrlTypes.CTRL_CLOSE_EVENT)
            {
                log.info("=== SHUTDOWN BY CTRL_CLOSE_EVENT ===");
                exit();
            }
            return false;
        }
    }
}
