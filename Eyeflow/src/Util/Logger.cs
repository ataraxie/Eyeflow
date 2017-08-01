using System;
using System.IO;
using System.Text;

namespace Eyeflow.Util
{
    public class Logger
    {
        public static int LEVEL_ERROR = 1;
        public static int LEVEL_INFO = 2;
        public static int LEVEL_DEBUG = 3;
        
        private Type type;
        private string className;
        private static FileStream FS;

        static Logger()
        {
            string path = Config.Instance.logFilePath;
            FS = File.Open(path, FileMode.Append);
        }

        public static Logger get(Type type)
        {
            return new Logger(type);
        }

        private Logger(Type type)
        {
            this.type = type;
            this.className = this.type.Name;
        }

        public void debug(string msg)
        {
            debug(msg, new object[0]);
        }

        public void debug(string msg, params object[] values)
        {
            if (Config.Instance.logLevel >= LEVEL_DEBUG)
            {
                writeLog("debug", msg, values);
            }
        }

        public void info(string msg)
        {
            info(msg, new object[0]);
        }

        public void info(string msg, params object[] values)
        {
            if (Config.Instance.logLevel >= LEVEL_INFO)
            {
                writeLog("info", msg, values);
            }
        }

        public void error(string msg)
        {
            error(msg, new object[0]);
        }

        public void error(string msg, params object[] values)
        {
            if (Config.Instance.logLevel >= LEVEL_ERROR)
            {
                writeLog("error", msg, values);
            }
        }

        private void writeLog(string level, string msg, params object[] values)
        {
            if (values.Length > 0)
            {
                msg = String.Format(msg, values);
            }
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logString = time + " [" + level + "] " + this.className + " | " + msg;
            writeToFile(logString);
        }

        private void writeToFile(string value)
        {
            value += "\n";
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            FS.Write(info, 0, info.Length);
        }

    }
}
