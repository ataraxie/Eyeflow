using System;
using System.Collections.Generic;
using System.Drawing;
using Eyetrack.Util;

namespace Eyetrack
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
        public string logFilePath = @"C:\Windows\Temp\eyetrack.log";
        public int logLevel = 2;

        // processes/windows
        public List<string> ignoredProcesses = new List<string>(new string[] {
            //"explorer", "Eyetrack"
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
