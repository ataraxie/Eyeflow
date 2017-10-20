using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using Eyeflow.Util;
using System.Configuration;
using System.Linq;

namespace Eyeflow
{
    class Config
    {
        private static Logger log = Logger.get(typeof(Config));
        private static Config instance;

        // global
        public bool dataCollectionMode;
        public bool simulationMode;
        public long globalCheckTimerInterval; // 1000
        public int runOnEveryXGazeDispatch; // 1
        public int gazeTimeRequiredForHighlightMs;
        public int howManyHighlightedConcurrentWindows = 1;
        public bool enableForTaskBar = true;

        public int windowToForegroundOnGazeAfterMs;
        public int windowInactiveThresholdMs; // 5000;

        // animation
        public int fadeOutAnimationTimeIntervalMs; // 20;
        public int fadeOutAnimationTransparencyInterval; // 10;
        public int fadeOutAnimationMinTransparency; // 40;

        // debugging
        public bool drawGazePositionRectangles; // false; // ONLY FOR DEV!!
        public Color gazePositionColor; // blue;

        // logging
        public string logFilePath; // "C:\Windows\Temp\eyeflow.log";
        public string logLevel;
        public bool logToConsole; // true;
        public bool logShowMetaInfo; // false;

        // sqlite
        public string databaseFilePath;

        static Config()
        {
            instance = new Config();
        }

        private Config() {
            readConfigProperties();
        }

        public static Config Instance
        {
            get
            {
                return instance;
            }
        }

        private void readConfigProperties()
        {
            this.enableForTaskBar = boolFromConfig("enableForTaskBar");
            this.dataCollectionMode = boolFromConfig("dataCollectionMode");
            this.simulationMode = boolFromConfig("simulationMode");
            this.globalCheckTimerInterval = longFromConfig("globalCheckTimerInterval");
            this.runOnEveryXGazeDispatch = intFromConfig("runOnEveryXGazeDispatch ");
            this.gazeTimeRequiredForHighlightMs = intFromConfig("gazeTimeRequiredForHighlightMs");
            this.howManyHighlightedConcurrentWindows = intFromConfig("howManyHighlightedConcurrentWindows");
            this.windowToForegroundOnGazeAfterMs = intFromConfig("windowToForegroundOnGazeAfterMs");
            this.windowInactiveThresholdMs = intFromConfig("windowInactiveThresholdMs");
            this.fadeOutAnimationTimeIntervalMs = intFromConfig("fadeOutAnimationTimeIntervalMs");
            this.fadeOutAnimationTransparencyInterval = intFromConfig("fadeOutAnimationTransparencyInterval");
            this.fadeOutAnimationMinTransparency = intFromConfig("fadeOutAnimationMinTransparency");
            this.drawGazePositionRectangles = boolFromConfig("drawGazePositionRectangles");
            this.gazePositionColor = colorFromConfig("gazePositionColor");
            this.logFilePath = stringFromConfig("logFilePath");
            this.logLevel = stringFromConfig("logLevel");
            this.logToConsole = boolFromConfig("logToConsole");
            this.logShowMetaInfo = boolFromConfig("logShowMetaInfo");
            this.databaseFilePath = stringFromConfig("databaseFilePath");
        }

        public override string ToString()
        {
            Dictionary<string, object> configFields = new Dictionary<string, object>();
            foreach (FieldInfo field in this.GetType().GetFields())
            {
                if (field.IsPublic)
                {
                    object fieldValue = field.GetValue(this);
                    configFields.Add(field.Name, fieldValue);
                }
            }
            return "{" + string.Join(",", configFields.Select(x => x.Key + "=" + x.Value)) + "}";
        }

        private long longFromConfig(string key)
        {
            return Convert.ToInt64(stringFromConfig(key));
        }

        private int intFromConfig(string key)
        {
            return Convert.ToInt32(stringFromConfig(key));
        }

        private string stringFromConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        private bool boolFromConfig(string key)
        {
            return Convert.ToBoolean(stringFromConfig(key));
        }

        private Color colorFromConfig(string key) {
            return Color.FromName(stringFromConfig(key));
        }

    }
}
