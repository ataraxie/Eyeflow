using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eyeflow.Util
{
    public class Logger
    {
        public static int LEVEL_ERROR = 0;
        public static int LEVEL_WARN = 1;
        public static int LEVEL_INFO = 2;
        public static int LEVEL_DEBUG = 3;

        public static string ERROR = "error";
        public static string WARN = "warn";
        public static string INFO = "info";
        public static string DEBUG = "debug";

        public static List<string> LEVELS = new List<string>(new string[] {
            "error", "warn", "info", "debug"
        });

        private Type type;
        private string className;
        private static FileStream FS;
        private static int LOG_LEVEL;

        static Logger()
        {
            FS = File.Open(Config.Instance.logFilePath, FileMode.Append);
            LOG_LEVEL = LEVELS.IndexOf(Config.Instance.logLevel.ToLower());
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
            if (LOG_LEVEL >= LEVEL_DEBUG)
            {
                writeLog(DEBUG, msg, values);
            }
        }

        public void warn(string msg)
        {
            warn(msg, new object[0]);
        }

        public void warn(string msg, params object[] values)
        {
            if (LOG_LEVEL >= LEVEL_WARN)
            {
                writeLog(WARN, msg, values);
            }
        }

        public void info(string msg)
        {
            info(msg, new object[0]);
        }

        public void info(string msg, params object[] values)
        {
            if (LOG_LEVEL >= LEVEL_INFO)
            {
                writeLog(INFO, msg, values);
            }
        }

        public void error(string msg)
        {
            error(msg, new object[0]);
        }

        public void error(string msg, params object[] values)
        {
            if (LOG_LEVEL >= LEVEL_ERROR)
            {
                writeLog(ERROR, msg, values);
            }
        }

        private void writeLog(string level, string msg, params object[] values)
        {
            if (values.Length > 0)
            {
                msg = String.Format(msg, values);
            }
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string metaInfoPrefix = time + " [" + level + "] " + this.className;
            if (Config.Instance.logShowMetaInfo)
            {
                msg = time + " | " + msg;
            }
            writeToFile(msg);
            if (Config.Instance.logToConsole)
            {
                Console.WriteLine(msg);
            }
        }

        private void writeToFile(string value)
        {
            value += "\n";
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            FS.Write(info, 0, info.Length);
        }

    }
}
