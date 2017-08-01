using System;
using System.Collections.Generic;
using System.Drawing;
using Eyeflow.Util;

namespace Eyeflow
{
    class Config
    {
        private static Config instance;

        // global
        public long globalCheckTimerInterval = 1000;
        public int runOnEveryXGazeDispatch = 1;

        public bool windowToForegroundOnGaze = false;
        public int windowInactiveThresholdMs = 5000;

        // animation
        public int fadeOutAnimationTimeIntervalMs = 20;
        public int fadeOutAnimationTransparencyInterval = 10;
        public int fadeOutAnimationMinTransparency = 40;

        // debugging
        public bool drawGazePositionRectangles = false; // ONLY FOR DEV!!
        public Color gazePositionColor = Color.Beige;

        // logging
        public string logFilePath = @"C:\Windows\Temp\eyeflow.log";
        public int logLevel = 2;

        // processes/windows
        public List<string> ignoredProcesses = new List<string>(new string[] {
            //"explorer", "Eyeflow"
        });

        private Config() {}

        internal static void init()
        {
            if (instance != null)
            {
                throw new Exception("Already instantiated. Cannot instantiate again.");
            }
            instance = new Config();
        }

        public static Config Instance
        {
            get
            {
                return instance;
            }
        }

    }
}
