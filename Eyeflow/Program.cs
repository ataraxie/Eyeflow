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
                EyeflowConsoleApp.execute();
            } catch (Exception e)
            {
                log.error("Error occurred: {0}", e.ToString());
                WinLib.setTransparency255ForAllWindows();
            }
        }
    }
}
