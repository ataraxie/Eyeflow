using System;
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
                new EyeflowConsoleApp().execute();
            } catch (Exception e)
            {
                log.error("Error occurred: {0}", e.ToString());
                if (!Config.Instance.dataCollectionMode && !Config.Instance.simulationMode)
                {
                    WinLib.setTransparency255ForAllWindows();
                }
            }
        }
    }
}
