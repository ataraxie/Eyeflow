using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Eyeflow;
using Eyeflow.Runners;
using Eyeflow.Dispatchers;

using Eyeflow.Util;

namespace Eyeflow.Lifecycle
{
    class EyeflowConsoleApp : EyeflowApp
    {
        private static Logger log = Logger.get(typeof(EyeflowConsoleApp));

        private static WinLib.HandlerRoutine consoleHandler;

        public override void execute()
        {
            handleConsole();
            base.execute();
            Console.ReadKey();
            log.info("=== SHUTDOWN BY KEYPRESS ===");
            base.exit();
        }

        private void handleConsole()
        {
            consoleHandler = new WinLib.HandlerRoutine(ConsoleCtrlCheck);
            WinLib.SetConsoleCtrlHandler(consoleHandler, true);
        }

        private bool ConsoleCtrlCheck(WinLib.CtrlTypes ctrlType)
        {
            if (ctrlType == WinLib.CtrlTypes.CTRL_CLOSE_EVENT)
            {
                log.info("=== SHUTDOWN BY CTRL_CLOSE_EVENT ===");
                base.exit();
            }
            return false;
        }
    }
}
